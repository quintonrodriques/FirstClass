using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helpscreentext : MonoBehaviour
{

    private AudioSource source;
    // Start is called before the first frame update
    void Start()
    {

        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            source.Play();
            gameObject.SetActive(false);
        }

        if (Input.GetMouseButtonDown(0))
        {
            source.Play();
            gameObject.SetActive(false);
        }
    }
}
