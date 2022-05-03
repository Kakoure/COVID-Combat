using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressButtons : MonoBehaviour
{
    private const float _maxDistance = 4000;
    private GameObject _gazedAtObject = null;
    public LayerMask interactsWith;
    public string pressButton;

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    public void Update()
    {
        // Casts ray towards camera's forward direction, to detect if a GameObject is being gazed
        // at.
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, _maxDistance, interactsWith))
        {
            // GameObject detected in front of the camera.
            if (_gazedAtObject != hit.transform.gameObject)
            {
                // New GameObject.
                _gazedAtObject?.SendMessage("OnPointerExit");
                _gazedAtObject = hit.transform.gameObject;
                _gazedAtObject.SendMessage("OnPointerEnter");
            }
        }
        else
        {
            // No GameObject detected in front of the camera.
            _gazedAtObject?.SendMessage("OnPointerExit");
            _gazedAtObject = null;
        }

        if (Input.GetButtonDown(pressButton))
        {
            _gazedAtObject?.SendMessage("OnPointerClick");
        }
    }
}
