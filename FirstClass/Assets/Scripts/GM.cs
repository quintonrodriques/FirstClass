using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM : MonoBehaviour
{
	public GameObject plane;
	public GameObject waypoints;
	public int planesPerMinute = 0;    
	
	private Vector3[] spawnPoints;                      //Holds spawn points for all spawn points
	private Vector3[] spawnVectors = new Vector3[4];    //Holds spawn vector for all 4 walls
	private bool gameRunning = true;

	private List<PlaneController> airplanePool;

	void Start()
	{
		airplanePool = new List<PlaneController>();

		setSpawnWalls();

		float spawnDelay = 60f / planesPerMinute;
		StartCoroutine(SpawnAirplane(spawnDelay));    

		Debug.Log("Time in seconds between every plane: " + spawnDelay);
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
			airplanePool.Add(Instantiate(plane, Vector3.zero, Quaternion.identity).GetComponent<PlaneController>());
			index = airplanePool.Count - 1;
		}
		
		airplanePool[index].transform.position = spawnPosition;
		airplanePool[index].SetTarget(GetTargetAirport(spawnPosition));
		airplanePool[index].BeginJourney();
	}

	Vector3 GetTargetAirport(Vector3 spawnPosition)
	{
		Vector3 targetPoint;
		do
			targetPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
		while (targetPoint.x == spawnPosition.x || targetPoint.z == spawnPosition.z);

		return targetPoint;
	}

	int GetAvailableAirplaneIndex()
	{
		for (int i = 0; i < airplanePool.Count; i++)
		{
			if (airplanePool[i].isAvailable)
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
