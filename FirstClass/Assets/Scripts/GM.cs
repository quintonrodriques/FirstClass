using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM : MonoBehaviour
{

    public Camera camera;
    public GameObject plane;
    public GameObject weatherEffect;

    public GameObject waypoints;
    private Vector3[] spawnPoints = new Vector3[4];               //Holds spawn points for all 4 walls
    private Vector3[] spawnVectors = new Vector3[4];               //Holds spawn vector for all 4 walls

    public int planesPerMinute = 0;    
    float spawnDelay = 0f;                //Delay time in-between each plane, in seconds

    float elapsedTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        Vector3[] spawnPoints = new Vector3[4];
        Vector3[] spawnVectors = new Vector3[4];
        setSpawnWalls();

        spawnDelay = 60f / (float) planesPerMinute;
        //Debug.Log("Time in seconds between every plane: " + spawnDelay);
    }

    void setSpawnWalls()
    {
        spawnPoints[0] = waypoints.transform.GetChild(0).transform.position;
        spawnPoints[1] = waypoints.transform.GetChild(1).transform.position;
        spawnPoints[2] = waypoints.transform.GetChild(2).transform.position;
        spawnPoints[3] = waypoints.transform.GetChild(3).transform.position;



        spawnVectors[0] = spawnPoints[1] - spawnPoints[0];
        spawnVectors[1] = spawnPoints[3] - spawnPoints[1];
        spawnVectors[2] = spawnPoints[3] - spawnPoints[2];
        spawnVectors[3] = spawnPoints[0] - spawnPoints[2];
    }

    void spawn()
    {
        int selection = Random.Range(0, 4);
        float positionAlongSpawnVector = Random.Range(0f, 1f);

        Vector3 spawnVector = spawnPoints[selection] + (positionAlongSpawnVector * spawnVectors[selection]);

        Instantiate(plane, spawnVector, Quaternion.identity);
    }


    void OnMouseDown()
    {
        Vector3 clickPosition = -Vector3.one;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit))
        {
            clickPosition = hit.point;
            Debug.Log(clickPosition);
            Instantiate(weatherEffect, clickPosition, Quaternion.identity);
        }
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


    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= spawnDelay)
        {
            elapsedTime = elapsedTime % 1f;
            SpawnAirplane();
        }
    }
    void SpawnAirplane()
    {
        //Debug.Log("Plane spawned!" + elapsedTime);
        spawn();
    }
}
