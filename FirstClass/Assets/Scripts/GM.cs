using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class GM : MonoBehaviour
{
	private static GM _intstance;

	public GameObject plane;
	public GameObject waypoints;
	public TextMeshProUGUI scoreText;
	public Slider braverySlider;
	public TextMeshProUGUI braveryValueText;
	public string[] braveryValues;

	private Boid selectedBoid;

	public int planesPerMinute = 0;    
	
	private Vector3[] spawnPoints;                      //Holds spawn points for all spawn points
	private Vector3[] spawnVectors = new Vector3[4];    //Holds spawn vector for all 4 walls
	private bool gameRunning = true;

	public static int totalScore = 0;

	private List<Boid> airplanePool;

	public GameObject weatherEffect;

	public static bool mouseOverButton = false;
	
	public static void addScoreToTotal(int score)
<<<<<<< HEAD
	{
		Debug.Log("+" + score + " added!");
=======
    {

		//Debug.Log("+" + score + " added!");
>>>>>>> eac996441c026eb0e92954fd0f7231ee3697987f
		GM.totalScore += score;
		_intstance.scoreText.text = GM.totalScore.ToString();
	}

	public void addScores(int s)
	{
		scoreText.text = s.ToString();
	}

	void OnMouseDown()
	{
		/*
		RaycastHit hit;
		Ray ray = camera.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out hit))
		{
			Transform objectHit = hit.transform;
			Instantiate(weatherEffect, objectHit.position, Quaternion.identity);
		}
		*/
	}
	

	void Start()
	{
		airplanePool = new List<Boid>();

		setSpawnWalls();

		float spawnDelay = 60f / planesPerMinute;
		StartCoroutine(SpawnAirplane(spawnDelay));

		_intstance = this;
		//Debug.Log("Time in seconds between every plane: " + spawnDelay);
	}

	void Update()
	{
		if (selectedBoid == null)
			braverySlider.onValueChanged.RemoveAllListeners();

		braveryValueText.text = braveryValues[(int)Mathf.Clamp(braverySlider.value * braveryValues.Length, 0, braveryValues.Length - 1)];

		if (Input.GetMouseButtonDown(0) && !mouseOverButton)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out RaycastHit hit))
			{
				selectedBoid = hit.transform.GetComponent<Boid>();
				if (selectedBoid != null)
				{
					braverySlider.value = selectedBoid.bravery;
					braverySlider.onValueChanged.AddListener(selectedBoid.OnBraveryChanged);
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

	IEnumerator SpawnAirplane(float timeBetweenSpawns)
	{
		while (gameRunning)
		{
			spawn();
			yield return new WaitForSeconds(timeBetweenSpawns);
		}
	}
}
