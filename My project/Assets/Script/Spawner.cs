using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour
{
        [SerializeField]
        private float timeToSpawn = 5f;
        private ObjectPool objectPool;
        [SerializeField]
        public List<GameObject> prefab;
        public Transform max;
        public Transform min;
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
            {   int random=Random.Range(0, prefab.Count);
                yield return new WaitForSeconds(timeToSpawn);
                GameObject newCritter = objectPool.GetObject(prefab[random]);
            if (newCritter != null) {
                newCritter.transform.SetParent(transform);
                newCritter.transform.localPosition = new Vector3(Random.Range(min.position.x, max.position.x), 0, Random.Range(min.position.z, max.position.z)); ;
               
            }
            if (timeToSpawn > minSpawnInterval)
                timeToSpawn -= spawnIntervalDecrease;

        }
        }
}
