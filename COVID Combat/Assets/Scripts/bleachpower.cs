using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bleachpower : MonoBehaviour
{
    [SerializeField] FlashImage _flashImage = null;
    [SerializeField] Color _newColor = Color.red;
    public float dis = 1000f;

    // Start is called before the first frame update
    public void unleashbleach()
    {
        GameObject[] covidgroup;
        
        covidgroup = GameObject.FindGameObjectsWithTag("virus");
        Vector3 position= transform.position;
        _flashImage.startflash(.25f, .5f, _newColor);
        foreach (GameObject covid in covidgroup)
        {
            Vector3 diss = covid.transform.position - position;
            float curdiss = diss.sqrMagnitude;
            if (curdiss < dis)
            {
                covid.GetComponent<Explosion>().explode();
            }
        }
    }



}
