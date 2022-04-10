using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Image))]
public class FlashImage : MonoBehaviour
{
    Image _image = null;
    Coroutine _currentFlashRoutine = null;
    private void Awake()
    {
        _image = GetComponent<Image>();
    }
    public void startflash(float secondsforflash, float maxAlpha, Color newColor)
    {
        _image.color = newColor;
        maxAlpha = Mathf.Clamp(maxAlpha, 0, 1);
        if (_currentFlashRoutine != null)
            StopCoroutine(_currentFlashRoutine);
        _currentFlashRoutine = StartCoroutine(Flash(secondsforflash, maxAlpha));

    }

    IEnumerator Flash(float secondsforoneflash, float maxAlpha)
    {
        float flashinduration = secondsforoneflash / 2;
        for (float t = 0; t<=flashinduration; t+=Time.deltaTime)
        {
            Color colorthisframe = _image.color;
            colorthisframe.a = Mathf.Lerp(0, maxAlpha, t / flashinduration);
            _image.color = colorthisframe;
            yield return null;
        }
        float flashoutduration= secondsforoneflash / 2;
        for (float t = 0; t <= flashinduration; t+=Time.deltaTime)
        {
            Color colorthisframe = _image.color;
            colorthisframe.a = Mathf.Lerp( maxAlpha, 0,t / flashinduration);
            _image.color = colorthisframe;
            yield return null;
        }
        _image.color = new Color32(0, 0, 0, 0);


    }
}

/*https://www.youtube.com/watch?v=Yw3EoV5I_PE */