using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneController : MonoBehaviour
{
    Vector3 position;
    Vector3 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(new Vector3(0,0,0));
        transform.eulerAngles = new Vector3(90f, transform.eulerAngles.y, transform.eulerAngles.z);
    }
}
