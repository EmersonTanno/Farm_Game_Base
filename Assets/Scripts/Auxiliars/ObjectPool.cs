using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dustopia.RewardSystem
{
    public class ObjectPool : MonoBehaviour
    {
        public static ObjectPool Instance { get; private set; }
        private Dictionary<string, Queue<GameObject>> pool = new Dictionary<string, Queue<GameObject>>();
        private Dictionary<string, Transform> poolParents = new Dictionary<string, Transform>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void InstantiatePool(GameObject prefab, int quantity)
        {
            string key = prefab.name;

            if (!pool.ContainsKey(key))
            {
                pool[key] = new Queue<GameObject>();

                GameObject parent = new GameObject($"{key}_Pool");
                parent.transform.parent = transform;
                poolParents[key] = parent.transform;
            }

            for (int i = 0; i < quantity; i++)
            {
                GameObject obj = Instantiate(prefab, poolParents[key]);
                obj.name = prefab.name;
                obj.SetActive(false);
                pool[key].Enqueue(obj);
            }
        }

        public GameObject GetObject(GameObject prefab)
        {
            string key = prefab.name;

            if (!pool.ContainsKey(key))
                InstantiatePool(prefab, 1);

            if (pool[key].Count == 0)
                InstantiatePool(prefab, 1);

            GameObject obj = pool[key].Dequeue();
            obj.SetActive(true);
            return obj;
        }


        public void ReturnObject(GameObject obj)
        {
            string key = obj.name;

            obj.SetActive(false);
            if (!pool.ContainsKey(key))
                pool[key] = new Queue<GameObject>();

            obj.transform.SetParent(poolParents[key]);
            pool[key].Enqueue(obj);
        }
    }
}