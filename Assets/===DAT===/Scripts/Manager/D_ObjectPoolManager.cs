using UnityEngine;
using System.Collections.Generic;
using System;
public class D_ObjectPoolManager : MonoBehaviour
{
#region Singleton
    public static D_ObjectPoolManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
#region Preferences
    [System.Serializable]
    public class Pool
    {
        public string key;
        public GameObject prefab;
        public int initialSize; 
    }
    [SerializeField] private Pool[] pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;
#endregion 

// Start is called before the first frame update
/// <summary>
/// Khởi tạo các pool theo cấu hình trong inspector, tạo sẵn một số lượng object nhất định để tránh việc Instantiate
/// </summary>
    void Start()
    {
        InitializePools();
    }
    private void InitializePools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        foreach(Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for(int i = 0; i< pool.initialSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj); // Enqueue là thêm vào cuối hàng đợi
            }
            poolDictionary[pool.key] = objectPool;
        }
        
    }
    public GameObject GetObjectPool(string key,Transform parent = null)
    {
        if(!poolDictionary.ContainsKey(key))
        {
            Debug.LogWarning($"Pool with key {key} doesn't exist");
            return null;
        }
        Queue<GameObject> objectPool = poolDictionary[key];
        GameObject obj = objectPool.Count > 0 ? objectPool.Dequeue() : Instantiate(System.Array.Find(pools, p => p.key == key)?.prefab);
        obj.SetActive(true);
        if(parent != null)
        {
            obj.transform.SetParent(parent);
        }
        return obj;
    }
    public void ReturnObjectToPool(string key, GameObject obj)
    {
        obj.SetActive(false);
        poolDictionary[key].Enqueue(obj); // Enqueue là thêm vào cuối hàng đợi
    }
}
