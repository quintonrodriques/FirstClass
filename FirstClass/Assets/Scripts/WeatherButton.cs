using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherButton : MonoBehaviour
{

    public GameObject effect;
    public GameObject gM;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButton(0)){
            //Debug.Log("Something was pressed!");
            gM.GetComponent<GM>().weatherEffect = effect;
        }
    }
}
