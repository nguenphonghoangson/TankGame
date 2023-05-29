using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour
{
    [SerializeField] private float timeToSpawn = 5f;
    private ObjectPool objectPool;
    [SerializeField] public List<GameObject> prefab;
    public float minSpawnInterval = 1f;
    public float spawnIntervalDecrease = 0.1f;

    private void Start()
    {
        objectPool = FindObjectOfType<ObjectPool>();
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            int random = Random.Range(0, prefab.Count);
            yield return new WaitForSeconds(timeToSpawn);
            GameObject newCritter = objectPool.GetObject(prefab[random]);
            if (newCritter != null)
            {
                newCritter.transform.SetParent(transform);
                float randomAngle = Random.Range(0f, 360f);
                float distance = 100;
                Quaternion rotation = Quaternion.Euler(0f, randomAngle, 0f);
                Vector3 newPosition = transform.position + rotation * (Vector3.forward * distance);
                Debug.DrawRay(newPosition, Vector3.up, Color.red, 10);
                newCritter.transform.localPosition = newPosition;
                if (timeToSpawn > minSpawnInterval)
                    timeToSpawn -= spawnIntervalDecrease;

            }
        }
    }
}
