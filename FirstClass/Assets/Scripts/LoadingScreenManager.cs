using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenManager : MonoBehaviour
{
    public GameObject plane;
    public GameObject spawnPoint;
    float time = 0;
    public int planesPerMinute;
    float planeDelay;


    public bool menuActive;
    public Camera mainCam;

    // Start is called before the first frame update
    void Start()
    {
        menuActive = false;
        planeDelay = 60.0f / planesPerMinute;
    }

    public void setPlane()
    {
        GameObject[] planes;
        planes = GameObject.FindGameObjectsWithTag("Plane");

        List<GameObject> planesInView = new List<GameObject>();
        foreach (GameObject planeX in planes)
        {
            if (planeX.transform.position.y < -11.8f)
            {
                planesInView.Add(planeX);
            }
        }

        GameObject closestPlane = planesInView[0];
        foreach (GameObject planeY in planesInView)
        {
            if (planeY.transform.position.y < closestPlane.transform.position.y)
            {
                closestPlane = planeY;
            }
        }

        closestPlane.GetComponent<Plane>().changeTrajectory(mainCam.transform);
        closestPlane.GetComponent<Plane>().speed = 20f;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > planeDelay)
        {
            //Debug.Log("Making Plane");
            Instantiate(plane, spawnPoint.transform.position, Quaternion.Euler(-82, 83, 81));
            time = 0f;
        }
    }
}
