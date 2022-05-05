using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CellSpawner : NetworkBehaviour
{
    public float spawnDistanceMin;
    public float spawnDistanceMax;
    public float spawnRateOverTime;
    public float spawnRateOverDistance;
    public float spawnOffset;
    public GameObject playerObj;
    public float despawnDistance;
    public LayerMask spawnMask;
    public RoleManager roleManager;


    public bool distSpawnEnabled;
    public bool timeSpawnEnabled;

    float spawnDistanceTracker;
    float spawnTimeTracker;

    Vector3 lastPos;
    [Serializable]
    public struct ObjectPool
    {
        public GameObject objPrefab;
        public GameObject containerObj;
        public string origintag;
        public Queue<GameObject> pooledObjects;
        public int amountToPool;
        public float spawnWeight;
    }

    public List<ObjectPool> spawnableItems;

    void Start()
    {
        if (!isServer)
        {
            return;
        }
        GenerateObjects();
        spawnDistanceTracker = 0f;
        spawnTimeTracker = 0f;
        lastPos = playerObj.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isServer)
        {
            return;
        }
        //Dont spawn if game hasnt started
        if (!roleManager.PilotJoined())
        {
            return;
        }

        var deltaPosition = playerObj.transform.position - lastPos;
        transform.position = playerObj.transform.position;

        if (distSpawnEnabled)
        {
            spawnDistanceTracker += deltaPosition.magnitude;
        }
        

        if (timeSpawnEnabled)
        {
            spawnTimeTracker += Time.deltaTime;

        }
        
        while(spawnDistanceTracker > spawnRateOverDistance)
        {
            Debug.Log("Distance Spawn");
            SpawnObject();
            spawnDistanceTracker -= spawnRateOverDistance;
        }

        while(spawnTimeTracker > spawnRateOverTime)
        {
            Debug.Log("Time Spawn");
            SpawnObject();
            spawnTimeTracker -= spawnRateOverTime;
        }

        lastPos = playerObj.transform.position;
    }

    void GenerateObjects()  
    {
        for(int i = 0; i < spawnableItems.Count; i++) {
            var pool = spawnableItems[i];    
            pool.pooledObjects = new Queue<GameObject>();
            spawnableItems[i] = pool;
        }
    }

    GameObject SpawnPoolObj(ObjectPool pool)
    {
        var newObj = Instantiate(pool.objPrefab, pool.containerObj.transform);
        newObj.SetActive(true);
        newObj.tag=pool.origintag;
        var moveNet = newObj.GetComponent<CellMoveNetwork>();
        moveNet.despawnDistance = despawnDistance;
        moveNet.playerObj = playerObj;
        moveNet.SetPool(pool);
        NetworkServer.Spawn(newObj);
        return newObj;
    }

    ObjectPool SelectRandomPool()
    {
        float sumWeights = 0f;
        foreach(ObjectPool pool in spawnableItems)
        {
            sumWeights += pool.spawnWeight;
        }

        float selection = UnityEngine.Random.Range(0f, sumWeights);

        for(int i = 0; i < spawnableItems.Count; i++)
        {
            selection -= spawnableItems[i].spawnWeight;
            if (selection <= 0)
            {
                return spawnableItems[i];
            }
        }

        Debug.Log("ErrorGeneratingPool");
        return spawnableItems[0];
    }

    GameObject GetNextObject()
    {
        var pool = SelectRandomPool();
        if(pool.pooledObjects.Count > 0)
        {
            return pool.pooledObjects.Dequeue();
        } else
        {
            return SpawnPoolObj(pool);           
        }
    }
    void SpawnObject()
    {
        int spawnIters = 0;
        bool validSpawn = false;
        Vector3 spawnPos = Vector3.zero;
        var objToSpawn = GetNextObject();
        var objBounds = objToSpawn.GetComponent<Collider>().bounds.extents;
        while (!validSpawn && spawnIters < 1000)
        {
            var randAngle = Quaternion.Euler(UnityEngine.Random.Range(-180f, 180f), UnityEngine.Random.Range(-180f, 180f), UnityEngine.Random.Range(-180f, 180f));
            var spawnDistance = UnityEngine.Random.Range(spawnDistanceMin, spawnDistanceMax);
            var posVect = Vector3.right;
            
            if (objToSpawn == null)
            {
                return;
            }
            spawnPos = playerObj.transform.position + (randAngle * posVect * spawnDistance) + (playerObj.transform.forward * spawnOffset);
            if(Physics.OverlapSphere(spawnPos, Mathf.Max(objBounds.x, objBounds.y, objBounds.z) + 1f, spawnMask, QueryTriggerInteraction.Collide).Length <= 0)
            {
                validSpawn = false; 
            } else
            {
                validSpawn = true;
            }
        }

        if(spawnIters >= 1000)
        {
            Debug.LogError("Failed to find valid spawn location!");
        }
        


        objToSpawn.transform.position = spawnPos;
        objToSpawn.SetActive(true);
        var cellCntrl = objToSpawn.GetComponent<CellMoveNetwork>();
        cellCntrl.RpcShowObject();
        cellCntrl.BloodCellMove();
    }


}
