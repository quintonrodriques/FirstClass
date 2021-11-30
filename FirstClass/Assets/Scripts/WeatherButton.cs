using UnityEngine;

public class WeatherButton : MonoBehaviour
{
    public GameObject effect;
    public GameObject gM;

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0)){
            //Debug.Log("Something was pressed!");
            gM.GetComponent<GM>().weatherEffect = effect;
        }
    }
}
