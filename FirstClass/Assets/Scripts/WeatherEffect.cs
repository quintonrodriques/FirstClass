using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherEffect : MonoBehaviour
{
    public float timeOfEffect = 0f;
    public float radiusGrowth = 0f;

    public ParticleSystem storm1;
    public ParticleSystem storm2;

    private AudioSource source;

    float elapsedTime = 0f;
    float opacity = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        BoidManager.AddObsticle(GetComponent<Obsticle>());
        source.Play();
    }

    // Update is called once per frame
    void Update()
    {
        opacity = 1.0f - (elapsedTime / timeOfEffect);
        this.GetComponent<MeshRenderer>().material.color = new Color(95.0f, 190.0f, 255.0f, opacity);

        elapsedTime += Time.deltaTime;
        //transform.localScale = new Vector3(1.0f + elapsedTime, 1.1f + elapsedTime, 1.1f + elapsedTime);

        float percentComplete = ((timeOfEffect - (timeOfEffect - elapsedTime)) / timeOfEffect);
        float radiusGrowFactor = percentComplete * radiusGrowth;

        transform.localScale = new Vector3(1.0f + radiusGrowFactor, 1.1f + radiusGrowFactor, 1.1f + radiusGrowFactor);

        if (elapsedTime >= timeOfEffect)
        {
            BoidManager.RemoveObsticle(GetComponent<Obsticle>());
            Destroy(gameObject);
        }
    }
}
