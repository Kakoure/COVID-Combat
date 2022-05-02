using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    public void SetMaxPower(int power)
    {
        slider.maxValue = power;
        slider.value = 0;
        fill.color = gradient.Evaluate(1f);
    }

    public void SetPower(int power)
    {
        slider.value = power;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
