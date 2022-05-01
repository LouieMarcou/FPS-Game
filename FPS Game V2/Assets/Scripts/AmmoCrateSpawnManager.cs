using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoCrateSpawnManager : MonoBehaviour
{
    public List<GameObject> pooledObjects;
    public GameObject objectToPool;
    public GameObject[] spawnPoints;
    public int amountToPool;
    // Start is called before the first frame update
    void Start()
    {
        pooledObjects = new List<GameObject>();
        GameObject tmp;

        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(objectToPool, gameObject.transform);
            //tmp.SetActive(false);
            pooledObjects.Add(tmp);
            tmp.transform.position = spawnPoints[i].transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
