using UnityEngine;

public class UI_Blocker : MonoBehaviour
{
    private void OnMouseEnter()
    {
        GM.mouseOverButton = true;
    }

    private void OnMouseExit()
    {
        GM.mouseOverButton = false;
    }
}
