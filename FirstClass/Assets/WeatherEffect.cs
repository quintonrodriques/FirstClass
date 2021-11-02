using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherEffect : MonoBehaviour
{
    public float timeOfEffect = 0f;
    public float radiusGrowth = 0f;
    float elapsedTime = 0f;
    float opacity = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        opacity = 1.0f - (elapsedTime / timeOfEffect);
        this.GetComponent<MeshRenderer>().material.color = new Color(95.0f, 190.0f, 255.0f, opacity);
        //Debug.Log(opacity);

        elapsedTime += Time.deltaTime;
        transform.localScale = new Vector3(1.0f + elapsedTime, 1.1f + elapsedTime, 1.1f + elapsedTime);

        if (elapsedTime >= timeOfEffect)
        {
            Destroy(gameObject);
        }
    }
}
