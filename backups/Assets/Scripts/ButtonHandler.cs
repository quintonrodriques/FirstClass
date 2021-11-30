using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    public void SetText(string message)
    {
        Text txt = transform.Find("Text").GetComponent<Text>();
        txt.text = message;
        Debug.Log("BUTTON PRESSED");
    }


}
