using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderButtonNew : MonoBehaviour
{
    public GM gM;
    public GameObject effect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnClick()
    {
        gM.GetComponent<GM>().weatherEffect = effect;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
