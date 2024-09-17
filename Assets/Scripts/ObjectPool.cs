using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance; // Singleton instance

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Set the instance
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instance
        }
    }

    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>(); // Dictionary to hold pools

    // Create a pool of objects
    public void CreatePool(GameObject prefab, int poolSize)
    {
        string key = prefab.name;
        if (!poolDictionary.ContainsKey(key))
        {
            poolDictionary[key] = new Queue<GameObject>();

            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(prefab); // Create object
                obj.SetActive(false); // Deactivate it
                obj.transform.SetParent(this.transform); // Set as child of ObjectPool GameObject
                poolDictionary[key].Enqueue(obj); // Add to pool
            }
        }
    }

    // Get an object from the pool
    public GameObject GetFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        string key = prefab.name;
        if (poolDictionary.ContainsKey(key) && poolDictionary[key].Count > 0)
        {
            GameObject obj = poolDictionary[key].Dequeue(); // Get object from pool
            obj.transform.position = position; // Set position
            obj.transform.rotation = rotation; // Set rotation
            obj.SetActive(true); // Activate it
            return obj;
        }
        else
        {
            GameObject obj = Instantiate(prefab, position, rotation); // Create new object if pool is empty
            obj.transform.SetParent(this.transform); // Set as child of ObjectPool GameObject
            return obj;
        }
    }

    // Return an object to the pool
    public void ReturnToPool(GameObject obj)
    {
        if(obj == null) return; // Check if object is null
        obj.SetActive(false); // Deactivate it
        obj.transform.SetParent(this.transform); // Set as child of ObjectPool GameObject
        string key = obj.name.Replace("(Clone)", "").Trim(); // Get original name
        if (poolDictionary.ContainsKey(key))
        {
            poolDictionary[key].Enqueue(obj); // Add back to pool
        }
        else
        {
            Destroy(obj); // Destroy if no pool exists
        }
    }
}
