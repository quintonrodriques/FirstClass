using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HelpButtonHandler : MonoBehaviour
{
    public LoadingScreenManager gM;
    public GameObject helpText;

    private void Start()
    {
        ///gM = GameObject.Find("LoadingScreenManager");
    }


    IEnumerator HelpButtonAnim()
    {
        gM.helpActive = true;
        //gM.setPlane();

        yield return new WaitForSeconds(2.0f);

        helpText.SetActive(true);

    }


    public void HelpButton()
    {
        gM.GetComponent<LoadingScreenManager>().setPlane();
        helpText.SetActive(true);
    }
}
