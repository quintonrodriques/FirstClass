using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GM : MonoBehaviour
{
	private static GM _intstance;
	private AudioSource explosion1;

	public GameObject[] lifeGraphics;
	
	public GameObject plane;
	public GameObject explosion;
	public GameObject waypoints;
	public TextMeshProUGUI scoreText;
	public Slider braverySlider;
	public TextMeshProUGUI braveryValueText;
	public string[] braveryValues;

	private Boid selectedBoid;

	public int planesPerMinute = 0;
	public int maxPlanesPerMinute = 120;
	public int timeToRamp = 15;

	private int lives = 3;
	private float invincibilityCooldown = 0.5f;
	private float timeOfLastKill = 0f;

	private float timeSincelastSpawn;

	private Vector3[] spawnPoints;                      //Holds spawn points for all spawn points
	private Vector3[] spawnVectors = new Vector3[4];    //Holds spawn vector for all 4 walls
	private bool gameRunning = true;

	public static int totalScore = 0;

	private List<Boid> airplanePool;

	public GameObject weatherEffect;

	public static bool mouseOverButton = false;

	public GameObject sliderUI;

	public static void explosionAt(Transform t)
	{
		GM.totalScore += 0;
		_intstance.explode(t);
	}

	public void explode(Transform t)
    {
		GameObject explosionOne = Instantiate(explosion, t.position, Quaternion.identity);
		DownLife();
		StartCoroutine(DeleteExplosion(explosionOne));
	}

	public static void addScoreToTotal(int score)
    {
		GM.totalScore += score;
		_intstance.scoreText.text = GM.totalScore.ToString();
	}

	public void addScores(int s)
	{
		scoreText.text = s.ToString();
	}

	void Start()
	{
		lives = 3;
		totalScore = 0;

		airplanePool = new List<Boid>();
		timeSincelastSpawn = Time.time;

		explosion1 = GetComponent<AudioSource>();

		setSpawnWalls();

		float spawnDelay = 60f / planesPerMinute;
		StartCoroutine(SpawnAirplane(spawnDelay));

		_intstance = this;

		EnableRiskLevelUI(false);
	}

	void Update()
	{
		if (selectedBoid != null && selectedBoid.isLanding)
			ResetSelectedAirplane();
		
		braveryValueText.text = braveryValues[(int)Mathf.Clamp(braverySlider.value * braveryValues.Length, 0, braveryValues.Length - 1)];

		if (Input.GetMouseButtonDown(0) && !mouseOverButton)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out RaycastHit hit))
			{
				braverySlider.onValueChanged.RemoveAllListeners();

				if (selectedBoid != null)
					selectedBoid.SetOutline(false);

				selectedBoid = hit.transform.GetComponent<Boid>();
				if (selectedBoid != null)
				{
					selectedBoid.SetOutline(true);

					braverySlider.value = selectedBoid.bravery;
					braverySlider.onValueChanged.AddListener(selectedBoid.OnBraveryChanged);
					
					EnableRiskLevelUI(true);
				}
			}
			else
			{
				if (!mouseOverButton)
				{
					Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					clickPosition.y = 0;
					Instantiate(weatherEffect, clickPosition, Quaternion.identity);
				}
			}
		}
	}

	void ResetSelectedAirplane()
    {
		braverySlider.onValueChanged.RemoveAllListeners();
		selectedBoid = null;
		EnableRiskLevelUI(false);
	}

	void EnableRiskLevelUI(bool enable)
    {
		sliderUI.SetActive(enable);
    }

	void setSpawnWalls()
	{
		spawnPoints = new Vector3[waypoints.transform.childCount];
		for (int i = 0; i < spawnPoints.Length; i++)
			spawnPoints[i] = waypoints.transform.GetChild(i).position;

		// This assumes the first four spawn points are points along the wall;
		for (int i = 0; i < 4; i++)
			spawnVectors[i] = (spawnPoints[(i + 1) % 4] - spawnPoints[i]);
	}

	void spawn()
	{
		int selection = Random.Range(0, spawnPoints.Length);
		Vector3 spawnPosition = spawnPoints[selection];

		if (selection < 4) 
		{
			float positionAlongSpawnVector = Random.Range(0f, 1f);
			spawnPosition += (positionAlongSpawnVector * spawnVectors[selection]);
		}

		int index = GetAvailableAirplaneIndex();
		if (index < 0)
		{
			airplanePool.Add(Instantiate(plane, Vector3.zero, Quaternion.identity).GetComponent<Boid>());
			index = airplanePool.Count - 1;
		}
		
		airplanePool[index].transform.position = spawnPosition;
		airplanePool[index].SetTarget(GetTargetAirport(spawnPosition));
		airplanePool[index].gameObject.SetActive(true);
		airplanePool[index].Init();
	}

	Vector3 GetTargetAirport(Vector3 spawnPosition)
	{
		return -spawnPosition;
	}

	int GetAvailableAirplaneIndex()
	{
		for (int i = 0; i < airplanePool.Count; i++)
		{
			if (!airplanePool[i].gameObject.activeSelf)
				return i;
		}
		
		return -1;
	}


	IEnumerator DeleteExplosion(GameObject g)
    {
		yield return new WaitForSeconds(5.0f);
		Destroy(g);
	}

	IEnumerator SpawnAirplane(float timeBetweenSpawns)
	{
		while (gameRunning)
		{
			spawn();
			yield return new WaitForSeconds(timeBetweenSpawns);
		}
	}

	public void DownLife()
    {
        if (Time.time > timeOfLastKill)
        {
			timeOfLastKill = Time.time + invincibilityCooldown;
			lives--;
			explosion1.Play();

			lifeGraphics[lives].SetActive(false);
			if (lives <= 0)
				GameOver();
		
			ResetSelectedAirplane();
		}
    }

	public static void GameOver()
    {
		if (_intstance.executingGameOver != null)
			return;

		_intstance.executingGameOver = _intstance.StartCoroutine(_intstance.GameOverSceneChange());
    }

	Coroutine executingGameOver = null;
	IEnumerator GameOverSceneChange()
    {
		yield return new WaitForSeconds(1.0f);
		BoidManager.Reset();
		SceneManager.LoadSceneAsync("IntroScreen");
    }
}
