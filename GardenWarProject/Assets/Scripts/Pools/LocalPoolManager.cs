using System;
using System.Collections.Generic;
using UnityEngine;

public class LocalPoolManager : MonoBehaviour
{
    [Serializable]
    public class BehaviourData
    {
        public Component prefab;
        public uint amount;
    }
    
    [Serializable]
    public class GameObjectData
    {
        public GameObject prefab;
        public uint amount;
    }

    private static LocalPoolManager Instance;

    [SerializeField] private uint defaultPoolSize = 16;
    private static uint defaultAmount => Instance.defaultPoolSize;
    private static Transform defaultParent => Instance.transform;

    [SerializeField] private List<BehaviourData> componentPrefabs = new List<BehaviourData>();
    [SerializeField] private List<GameObjectData> gameObjectsPrefabs = new List<GameObjectData>();

    private static readonly Dictionary<Component, Queue<Component>> componentDict = new Dictionary<Component, Queue<Component>>();
    private static readonly Dictionary<GameObject, Queue<GameObject>> gameObjectDic = new Dictionary<GameObject, Queue<GameObject>>();

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
        componentDict.Clear();
        gameObjectDic.Clear();
        
        foreach (var data in componentPrefabs)
        {
            CreateQueue(data.prefab,data.amount);
        }

        foreach (var data in gameObjectsPrefabs)
        {
            CreateQueue(data.prefab,data.amount);
        }
    }

    private static void CreateQueue(Component component,uint amount = 0)
    {
        if (componentDict.ContainsKey(component))
        {
            Debug.LogWarning($"Duplicate key for {component.gameObject.name} (component)");
            return;
        }
        
        var newQueue = new Queue<Component>();

        amount = amount == 0 ? defaultAmount : amount;
        for (int i = 0; i < amount; i++)
        {
            var comp = Instantiate(component, defaultParent);
            comp.gameObject.SetActive(false);
            newQueue.Enqueue(comp);
        }
            
        componentDict.Add(component, newQueue);
    }
    
    private static void CreateQueue(GameObject prefab,uint amount = 0)
    {
        if (gameObjectDic.ContainsKey(prefab))
        {
            Debug.LogWarning($"Duplicate key for {prefab.gameObject.name} (gameObject)");
            return;
        }
        
        var newQueue = new Queue<GameObject>();

        amount = amount == 0 ? defaultAmount : amount;
        for (int i = 0; i < amount; i++)
        {
            var go = Instantiate(prefab, defaultParent);
            go.SetActive(false);
            newQueue.Enqueue(go);
        }
            
        gameObjectDic.Add(prefab, newQueue);
    }
    
    public static T PoolInstantiate<T>(T prefab, Transform parent = null,bool requeue = true) where T : Component
    {
        var savedAsTransform = typeof(T) != typeof(Transform) && componentDict.ContainsKey(prefab.transform) ;
        if (!componentDict.ContainsKey(prefab) && !savedAsTransform)
        {
            CreateQueue(prefab);
        }

        var component = savedAsTransform ? componentDict[prefab.transform].Dequeue().GetComponent<T>() : componentDict[prefab].Dequeue();

        component.transform.SetParent(parent);
        component.transform.localScale = Vector3.one;

        component.gameObject.SetActive(true);

        if (!requeue) return (T)component;
        
        if(savedAsTransform) 
        {
            componentDict[prefab.transform].Enqueue(component.transform);
        }
        else
        {
            componentDict[prefab].Enqueue(component);
        }

        return (T)component;
    }
    
    public static T PoolInstantiate<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null,bool requeue = true) where T : Component
    {
        var savedAsTransform = typeof(T) != typeof(Transform) && componentDict.ContainsKey(prefab.transform) ;
        if (!componentDict.ContainsKey(prefab) && !savedAsTransform)
        {
            CreateQueue(prefab);
        }

        var component = savedAsTransform ? componentDict[prefab.transform].Dequeue().GetComponent<T>() : componentDict[prefab].Dequeue();
        var tr = component.transform;
        var go = component.gameObject;

        tr.position = position;
        tr.rotation = rotation;
        tr.localScale = Vector3.one;

        tr.SetParent(parent);
        
        go.SetActive(false);
        
        go.SetActive(true);

        if (!requeue) return (T)component;
        
        if(savedAsTransform) 
        {
            componentDict[prefab.transform].Enqueue(tr);
        }
        else
        {
            componentDict[prefab].Enqueue(component);
        }

        return (T)component;
    }
    
    public static GameObject PoolInstantiate(GameObject prefab, Transform parent = null,bool requeue = true)
    {
        if (!gameObjectDic.ContainsKey(prefab)) CreateQueue(prefab);

        var go = gameObjectDic[prefab].Dequeue();

        go.transform.SetParent(parent);
        go.transform.localScale = Vector3.one;

        go.SetActive(true);
        
        if(requeue) gameObjectDic[prefab].Enqueue(go);
        
        return go;
    }
    
    public static GameObject PoolInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null,bool requeue = true)
    {
        if (!gameObjectDic.ContainsKey(prefab)) CreateQueue(prefab);

        var go = gameObjectDic[prefab].Dequeue();
        var tr = go.transform;

        tr.position = position;
        tr.rotation = rotation;
        tr.localScale = Vector3.one;
        
        tr.SetParent(parent);
        
        go.SetActive(true);
        
        if(requeue) gameObjectDic[prefab].Enqueue(go);
        
        return go;
    }
}
