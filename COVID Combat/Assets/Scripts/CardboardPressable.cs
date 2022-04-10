using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardboardPressable : MonoBehaviour
{
    
    public void OnPointerEnter()
    {

    }
    public void OnPointerExit()
    {

    }
    public void OnPointerClick()
    {
        GetComponent<Button>().onClick.Invoke();
    }
}
