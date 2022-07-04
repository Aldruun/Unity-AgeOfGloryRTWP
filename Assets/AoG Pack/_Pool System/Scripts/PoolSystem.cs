using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public enum ObjectPoolingCategory
{
    DEFAULT,
    VFX,
    SFX,

    //UI,
    CHARACTER,
    //GAMELOGIC,
    INDICATOR,
    MISC
}

public static class PoolSystem
{
    ///////////////////////////////////////////////////
    // Resources
    ///////////////////////////////////////////////////

    //static List<Item> _items = new List<Item>();

    private static readonly List<ObjectPool> _vfxLibrary = new List<ObjectPool>();

    private static readonly List<ObjectPool> _sfxLibrary = new List<ObjectPool>();

    //static List<ObjectPool> _uiLibrary = new List<ObjectPool>();
    private static readonly List<ObjectPool> _gameLogicLibrary = new List<ObjectPool>();
    private static readonly List<ObjectPool> _indicatorLibrary = new List<ObjectPool>();

    public static GameObject[] actorRefs;

    public static void LoadPoolObjects()
    {
        Transform parent = new GameObject("PoolingParent").transform;
        // ParticleSystems
        /*
         * 0 - vfx_generic_hit
         * 1 - vfx_generic_slash
         * 2 - vfx_holy_fireball
         * 3 - vfx_notify_levelup
         * 4 - vfx_notify_drinkhealthpotion
         */
        //vfx_hit_default = Resources.Load<GameObject>("Prefabs/ParticleSystems/vfx_hit_default");

        if(_vfxLibrary.Count > 0)
        {
            _vfxLibrary.Clear();
        }

        if(_sfxLibrary.Count > 0)
        {
            _sfxLibrary.Clear();
        }

        var vfxPool = new GameObject("Repositiory_VFX").transform;
        var vfxObjects = Resources.LoadAll<GameObject>("Prefabs/Particle Systems");
        for (var i = 0; i < vfxObjects.Length; i++) _vfxLibrary.Add(new ObjectPool(vfxObjects[i], vfxPool, 2));
        var sfxPool = new GameObject("Repositiory_SFX").transform;
        var sfxObject = Resources.Load<GameObject>("Prefabs/Audio/SFXObject"); // a gameobject with audiosource attached
        _sfxLibrary.Add(new ObjectPool(sfxObject, sfxPool, 3));

        vfxPool.SetParent(parent);
        sfxPool.SetParent(parent);
        ////Transform uiPool = new GameObject("Repositiory_UI").transform;
        ////GameObject[] uiObjects = Resources.LoadAll<GameObject>("Prefabs/UI");
        ////for(int i = 0; i < uiObjects.Length; i++)
        ////{
        ////    _uiLibrary.Add(new ObjectPool(uiObjects[i], uiPool, 3));
        ////}

        //var gameLogicPool = new GameObject("Repositiory_GAMELOGIC").transform;
        //var glObjects = Resources.LoadAll<GameObject>("Prefabs/Game Logic/");
        //for (var i = 0; i < glObjects.Length; i++)
        //    _gameLogicLibrary.Add(new ObjectPool(glObjects[i], gameLogicPool, 2));

        //var indicatorPool = new GameObject("Repositiory_INDICATORS").transform;
        //var indicatorObjects = Resources.LoadAll<GameObject>("Prefabs/Indicators/");
        //for (var i = 0; i < indicatorObjects.Length; i++) _indicatorLibrary.Add(new ObjectPool(indicatorObjects[i], indicatorPool, 10));
    }

    public static void StorePoolObject(Transform poolObject, ObjectPoolingCategory resourceType)
    {
        switch (resourceType)
        {
            case ObjectPoolingCategory.DEFAULT:
                break;
            case ObjectPoolingCategory.VFX:
                for (var i = 0; i < _vfxLibrary.Count; i++)
                    if (_vfxLibrary[i].identifier == poolObject.name)
                        _vfxLibrary[i].AddObject(poolObject);
                break;
            case ObjectPoolingCategory.SFX:
                for (var i = 0; i < _sfxLibrary.Count; i++)
                    if (_sfxLibrary[i].identifier == poolObject.name)
                        _sfxLibrary[i].AddObject(poolObject);
                break;
            //case ObjectPoolingCategory.UI:
            //    for(int i = 0; i < _uiLibrary.Count; i++)
            //    {
            //        if(_uiLibrary[i].identifier == identifier)
            //        {
            //            return _uiLibrary[i].GetAvailableObject();
            //        }
            //    }
            //    break;
            //case ObjectPoolingCategory.CHARACTER:
                //for(int i = 0; i < actorRefs.Length; i++)
                //{
                //    if(actorRefs[i].name == poolObject.name)
                //    {
                //        actorRefs[i].AddObject(poolObject);
                //    }
                //}
                //break;
            //case ObjectPoolingCategory.GAMELOGIC:
            //    for (var i = 0; i < _gameLogicLibrary.Count; i++)
            //        if (_gameLogicLibrary[i].identifier == poolObject.name)
            //            _gameLogicLibrary[i].AddObject(poolObject);
            //    break;
            case ObjectPoolingCategory.INDICATOR:
                for (var i = 0; i < _indicatorLibrary.Count; i++)
                    if (_indicatorLibrary[i].identifier == poolObject.name)
                        _indicatorLibrary[i].AddObject(poolObject);
                break;
        }
    }

    public static GameObject GetPoolObject(string identifier,
        ObjectPoolingCategory resourceType /*, float duration = 0*/)
    {
        GameObject found = null;

        //GameObject obj = null;
        switch (resourceType)
        {
            case ObjectPoolingCategory.DEFAULT:
                break;
            case ObjectPoolingCategory.VFX:
                for (var i = 0; i < _vfxLibrary.Count; i++)
                    if (_vfxLibrary[i].identifier == identifier)
                        found = _vfxLibrary[i].GetAvailableObject();
                break;
            case ObjectPoolingCategory.SFX:
                for (var i = 0; i < _sfxLibrary.Count; i++)
                    if (_sfxLibrary[i].identifier == identifier)
                        found = _sfxLibrary[i].GetAvailableObject();
                break;
            //case ObjectPoolingCategory.UI:
            //    for(int i = 0; i < _uiLibrary.Count; i++)
            //    {
            //        if(_uiLibrary[i].identifier == identifier)
            //        {
            //            return _uiLibrary[i].GetAvailableObject();
            //        }
            //    }
            //    break;
            case ObjectPoolingCategory.CHARACTER:
                for(var i = 0; i < actorRefs.Length; i++)
                    if(actorRefs[i].name == identifier)
                        found = actorRefs[i];
                break;
            //case ObjectPoolingCategory.GAMELOGIC:
            //    for (var i = 0; i < _gameLogicLibrary.Count; i++)
            //        if (_gameLogicLibrary[i].identifier == identifier)
            //            found = _gameLogicLibrary[i].GetAvailableObject();
            //    break;
            case ObjectPoolingCategory.INDICATOR:
                for (var i = 0; i < _indicatorLibrary.Count; i++)
                    if (_indicatorLibrary[i].identifier == identifier)
                        found = _indicatorLibrary[i].GetAvailableObject();
                break;
        }

        if (found == null) Debug.LogError("Pool object <color=white>'" + identifier + "'</color> not found");

        return found;
    }

    public static void ClearPools()
    {
        _vfxLibrary.Clear();
        _sfxLibrary.Clear();
    }
}

public class ObjectPool
{
    public int initialNumObjects;
    public int maxNumObjects;

    private Action OnActivate;
    private readonly List<GameObject> pool;
    private readonly GameObject prefab;
    private readonly Transform repository;

    public ObjectPool(GameObject prefab, Transform repository, int numInitialObjects, /*int maxNumObjects,*/
        Action OnActivate = null)
    {
        identifier = prefab.name;
        this.prefab = prefab;
        this.repository = repository;
        pool = new List<GameObject>();

        for (var i = 0; i < numInitialObjects; i++)
        {
            //if(initialNumObjects <= maxNumObjects) {
            var obj = Object.Instantiate(prefab, repository);
            obj.name = identifier;
            obj.SetActive(false);

            pool.Add(obj);
            //}
            //else {

            //    break;
            //}
        }

        initialNumObjects = numInitialObjects;
        //this.maxNumObjects = maxNumObjects;

        this.OnActivate = OnActivate;
    }

    public string identifier { get; }

    public GameObject GetAvailableObject()
    {
        //GameObject available = null;

        for (var i = 0; i < pool.Count; i++)
        {
            if (pool[i] == null)
            {
                pool.RemoveAt(i);

                if (i + 1 >= pool.Count) break;
            }

            if (pool[i].activeInHierarchy == false)
            {
                var available = pool[i];

                if (available.transform.parent != repository) available.transform.SetParent(repository);
                available.SetActive(true);
                return available;
            }
        }

        var obj = Object.Instantiate(prefab, repository);
        //obj.name = identifier;
        pool.Add(obj);
        return obj;
    }

    public void AddObject(Transform obj)
    {
        //available = pool[i];
        obj.gameObject.SetActive(false);
        obj.SetParent(repository);
    }
}