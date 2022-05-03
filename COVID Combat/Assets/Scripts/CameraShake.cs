using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float shakeDamper;
    public AnimationCurve shakeCurve;

    float t = 1f;
    void Update()
    {
        Shake();
    }

    void Shake()
    {
        t += Time.deltaTime;
        transform.localPosition = new Vector2(Random.insideUnitSphere.x, Random.insideUnitSphere.y) / shakeDamper * shakeCurve.Evaluate(Mathf.Clamp(t,-5f, 1f));
    }

    public void SetShake(float amount)
    {
        t = Mathf.Min(t, 1-amount);
    }
}

