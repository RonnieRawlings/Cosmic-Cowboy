using System.Collections;
using UnityEngine;

public class ChangeColour : MonoBehaviour
{
    public Color startColor;
    public Color mouseOverColor;
    bool mouseOver = false;

    private void OnMouseEnter()
    {
        mouseOver = true;
        GetComponent<Renderer>().material.SetColor("_Color", mouseOverColor);
    }

    private void OnMouseExit()
    {
        mouseOver = false;
        GetComponent<Renderer>().material.SetColor("_Color", startColor);
    }





}
