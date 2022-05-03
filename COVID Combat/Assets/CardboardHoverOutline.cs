using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardboardHoverOutline : MonoBehaviour
{
    Outline outline;

    private void Start()
    {
        outline = GetComponent<Outline>();
    }

    public void OnPointerEnter()
    {
        outline.enabled = true;
    }
    public void OnPointerExit()
    {
        outline.enabled = false;
    }
    public void OnPointerClick()
    {
       
    }
}