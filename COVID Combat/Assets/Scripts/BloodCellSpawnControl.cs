using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodCellSpawnControl : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject[] bloodcells;
    int randomSpawnPoint, randomBloodcell;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnABloodcell", 0f, 1f);
    }

    // Update is called once per frame
    void SpawnABloodcell()
    {
        randomSpawnPoint = Random.Range(0, spawnPoints.Length);
        randomBloodcell = Random.Range(0, bloodcells.Length);
        Instantiate(bloodcells[randomBloodcell], spawnPoints[randomSpawnPoint].position, Quaternion.identity);
    }
}
