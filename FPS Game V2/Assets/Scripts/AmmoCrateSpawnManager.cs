using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoCrateSpawnManager : MonoBehaviour
{
    public List<GameObject> pooledObjects;
    public GameObject objectToPool;
    public int amountToPool;
    public GameObject[] spawnPoints;
    // Start is called before the first frame update
    void Start()
    {
        pooledObjects = new List<GameObject>();
        GameObject tmp;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            tmp = Instantiate(objectToPool, spawnPoints[i].transform);
            tmp.transform.position = spawnPoints[i].transform.position;
            tmp.transform.rotation = spawnPoints[i].transform.rotation;
            //tmp.SetActive(false);
            pooledObjects.Add(tmp);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }
}
