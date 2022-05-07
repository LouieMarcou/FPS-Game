using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoCrateSpawnManager : MonoBehaviour
{
    public List<GameObject> pooledObjects;
    public GameObject objectToPool;
    public int amountToPool;
    public GameObject[] spawnPoints;

    public float secondsToSpawn = 30.0f;
    private WaitForSeconds spawnTime;
    private Coroutine startSpawnTime;
    void Awake()
    {
        spawnTime = new WaitForSeconds(secondsToSpawn);
    }

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
            tmp.SetActive(false);
            pooledObjects.Add(tmp);
            Spawning(tmp);
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
    
    public void Spawning(GameObject crate)
    {
        startSpawnTime = StartCoroutine(Wait(crate));
    }

    IEnumerator Wait(GameObject crate)
    {
        //Debug.Log("Starting");
        yield return spawnTime;
        crate.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        crate.SetActive(true);
        //Debug.Log("Finished");
    }
}
