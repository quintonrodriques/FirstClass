using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour
{

    GameObject spawnPoint;
    GameObject endPoint;
    GameObject trueEnd;
    public float speed;
    Vector3 dir;
    Vector3 randomness;

    // Start is called before the first frame update
    void Start()
    {

        spawnPoint = GameObject.Find("StartPath");
        endPoint = GameObject.Find("EndPath");

        trueEnd = new GameObject(); 

        

        randomness = new Vector3(Random.Range(-0.5f, 0.5f), 0f, Random.Range(-0.9f, 0.9f));
        trueEnd.transform.position = endPoint.transform.position + randomness;

        dir = spawnPoint.gameObject.transform.position - trueEnd.transform.position;
        dir = dir.normalized * -1;

    }

    public void changeTrajectory(Transform t)
    {
        dir = spawnPoint.gameObject.transform.position - t.position;
        dir = dir.normalized * -1;
    }

    private void FixedUpdate()
    {
        if (transform.position.y > 11.0f)
        {
            //Debug.Log("Despawn Plane");
            //Destroy(spawnPoint);
            //Destroy(endPoint);
            Destroy(trueEnd);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float distan = Vector3.Distance(transform.position, trueEnd.transform.position);
        if (distan > 5.0f)
        {
            transform.position += dir * speed * Time.deltaTime;
        }
    }
}
