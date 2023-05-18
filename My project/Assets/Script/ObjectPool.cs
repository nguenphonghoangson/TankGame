using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>();
    [SerializeField] private int maxPoolSize = 10;
    private int currentObjectCount = 0;
    public GameObject GetObject(GameObject gameObject)
    {
        if (objectPool.TryGetValue(gameObject.name, out Queue<GameObject> objectList))
        {
            if (objectList.Count == 0)
                return CreateNewObject(gameObject);
            else
            {
                GameObject _object = objectList.Dequeue();
                _object.SetActive(true);
                return _object;
            }
        }
        else
            return CreateNewObject(gameObject);
    }

    private GameObject CreateNewObject(GameObject gameObject)
    {
        if (currentObjectCount < maxPoolSize)
        {
            GameObject newGO = Instantiate(gameObject);
            newGO.name = gameObject.name;
            currentObjectCount++;
            return newGO;
        }
        else
        {
            return null;
        }
    }

    public void ReturnGameObject(GameObject gameObject)
    {
        if (objectPool.TryGetValue(gameObject.name, out Queue<GameObject> objectList))
        {
            objectList.Enqueue(gameObject);
        }
        else
        {
            Queue<GameObject> newObjectQueue = new Queue<GameObject>();
            newObjectQueue.Enqueue(gameObject);
            objectPool.Add(gameObject.name, newObjectQueue);
        }
        gameObject.SetActive(false);

    }
}
