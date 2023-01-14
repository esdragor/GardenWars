using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class NetworkPoolManager : MonoBehaviour
{
    public static bool isOffline => !PhotonNetwork.IsConnected;
    public static bool isMaster => isOffline || PhotonNetwork.IsMasterClient;

    [Serializable]
    public class PhotonViewData
    {
        public PhotonView entity;
        public uint amount;
    }

    private static NetworkPoolManager Instance;


    [SerializeField] private Vector3 defaultPosition = Vector3.down * 50;
    [SerializeField] private uint defaultPoolSize = 16;
    private static uint defaultAmount => Instance.defaultPoolSize;
    private static Transform defaultParent => Instance.transform;

    [SerializeField] private List<PhotonViewData> entityPrefabs = new List<PhotonViewData>();

    private static readonly Dictionary<string, Queue<GameObject>> stringDict = new Dictionary<string, Queue<GameObject>>();
    private static readonly Dictionary<string, Queue<GameObject>> offlineDict = new Dictionary<string, Queue<GameObject>>();


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }

        Instance = this;
    }

    public static void Init()
    {
        Instance.SetupDictionary();
    }

    private void SetupDictionary()
    {
        stringDict.Clear();
        offlineDict.Clear();

        if (isOffline)
        {
            foreach (var data in entityPrefabs)
            {
                CreateQueueOffline(data.entity.gameObject,data.amount);
            }
            return;
        }

        foreach (var data in entityPrefabs)
        {
            CreateQueue(data.entity.gameObject.name,data.amount);
        }
    }

    private static void CreateQueue(string name,uint amount = 0)
    {
        if (isOffline) return;

        if (stringDict.ContainsKey(name))
        {
            Debug.LogWarning($"Duplicate key for {name} (network)");
            return;
        }
        
        var newQueue = new Queue<GameObject>();

        amount = amount == 0 ? defaultAmount : amount;
        for (int i = 0; i < amount; i++)
        {
            newQueue.Enqueue(null);
        }
            
        stringDict.Add(name, newQueue);
    }
    
    private static void CreateQueueOffline(GameObject prefab,uint amount = 0)
    {
        if (offlineDict.ContainsKey(prefab.name))
        {
            Debug.LogWarning($"Duplicate key for {prefab} (offline)");
            return;
        }
        
        var newQueue = new Queue<GameObject>();

        amount = amount == 0 ? defaultAmount : amount;
        for (int i = 0; i < amount; i++)
        {
            newQueue.Enqueue(null);
        }
            
        stringDict.Add(prefab.name, newQueue);
    }
    
    public static GameObject PoolInstantiate(string name, Vector3 position, Quaternion rotation,bool requeue = true)
    {
        if (isOffline) return PoolInstantiateOffline(name, position, rotation, requeue);

        if (!stringDict.ContainsKey(name)) CreateQueue(name);

        var go = stringDict[name].Dequeue();

        if (go == null)
        {
            go = PhotonNetwork.Instantiate(name, position, rotation);
        }
        else
        {
            var tr = go.transform;
            tr.position = position;
            tr.rotation = rotation;
        }

        if(requeue) stringDict[name].Enqueue(go);
        
        go.SetActive(true);

        return go;
    }
    
    private static GameObject PoolInstantiateOffline(string name, Vector3 position, Quaternion rotation,bool requeue = true)
    {
        if (!offlineDict.ContainsKey(name)) CreateQueue(name);

        var go = offlineDict[name].Dequeue();
        var tr = go.transform;

        tr.position = position;
        tr.rotation = rotation;
        
        if(requeue) offlineDict[name].Enqueue(go);
        
        go.SetActive(true);
        
        return go;
    }

    public new static GameObject Instantiate(string name, Vector3 position, Quaternion rotation,bool requeue = true)
    {
        return PoolInstantiate(name, position, rotation, requeue);
    }
    
    
    
    
}