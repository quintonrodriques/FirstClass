using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButtonHandler : MonoBehaviour
{
    public GameObject gM;

    private void Start()
    {
        ///gM = GameObject.Find("LoadingScreenManager");
    }

    public void PlayButton()
    {

        //GameObject gM = GameObject.Find("LoadingScreenManager");
        if (gM.GetComponent<LoadingScreenManager>().menuActive == false)
        {
            Debug.Log("Plane incoming");
            gM.GetComponent<LoadingScreenManager>().menuActive = true;
            gM.GetComponent<LoadingScreenManager>().setPlane();
        }
        else
        {
            Debug.Log("Plane reset");
            gM.GetComponent<LoadingScreenManager>().menuActive = false;
        }
    }
}
