using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunObjectPool : MonoBehaviour
{
    public List<GameObject> pooledObjects;
    public GameObject[] objectsToPool;
    public GameObject[] spawnPoints;
    private List<Transform> alreadySpawnedAt;

    private int size;
    public int amountToPool;
    // Start is called before the first frame update
    void Start()
    {
        size = objectsToPool.Length;
        int random = Random.Range(0, size);
        alreadySpawnedAt = new List<Transform>();

        pooledObjects = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < size; i++)
        {
            Transform position = spawnPoints[random].transform;
            if(!alreadySpawnedAt.Contains(position))
                alreadySpawnedAt.Add(position);
            tmp = Instantiate(objectsToPool[i], spawnPoints[random].transform);
            tmp.SetActive(false);
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