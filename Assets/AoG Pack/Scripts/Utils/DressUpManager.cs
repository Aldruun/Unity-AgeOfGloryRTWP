using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public enum EquipmentSlot
{
    Head,
    Hands,
    Body,
    Bottom,
    Feet
}
public enum ClothType
{
    None,
    Head,
    Hands,
    FullBody,
    Bottom,
    Feet
}

public class DressUpManager : MonoBehaviour
{
    public bool debug;
    public List<Dress> dresses;
    public SkinnedMeshRenderer[] m_RendererSlots;

    private SkinnedMeshRenderer _headSlot;
    private SkinnedMeshRenderer _handsSlot;
    private SkinnedMeshRenderer _bodySlot;
    private SkinnedMeshRenderer _bottomSlot;
    private SkinnedMeshRenderer _feetSlot;

    public Transform skeletonRoot;
    private List<Transform> _skeletonBones;
    private Transform dressContainer;

    public Texture2D occlusionTex;
    private static Texture2D _occlusionTex;

    private void Awake()
    {
        _occlusionTex = occlusionTex;
        //foreach(Transform t in skeletonRoot)
        //{
        //    if(t.name == "slot_body")
        //    {
        //        _bodySlot = t.GetComponent<SkinnedMeshRenderer>();
        //    }
        //    else if(t.name == "slot_head")
        //    {
        //        _headSlot = t.GetComponent<SkinnedMeshRenderer>();
        //    }
        //    else if(t.name == "slot_hands")
        //    {
        //        _handsSlot = t.GetComponent<SkinnedMeshRenderer>();
        //    }
        //    else if(t.name == "slot_bottom")
        //    {
        //        _bottomSlot = t.GetComponent<SkinnedMeshRenderer>();
        //    }
        //    else if(t.name == "slot_feet")
        //    {
        //        _feetSlot = t.GetComponent<SkinnedMeshRenderer>();
        //    }
        //}

        dressContainer = transform.Find("Dresses");
        if(dressContainer == null)
            dressContainer = new GameObject("Dresses").transform;
        dressContainer.SetParent(transform);
        dressContainer.localPosition = Vector3.zero;
        dressContainer.localRotation = Quaternion.identity;

        //_skeletonBones = new List<Transform>();

        //_skeletonBones.AddRange(skeletonRoot.GetComponentsInChildren<Transform>());

        m_RendererSlots = GetComponentsInChildren<SkinnedMeshRenderer>();
        //m_characterSMRenderer.transform.localRotation = Quaternion.identity;
        //m_characterSMRenderer.sharedMesh = dresses[0].dressSlot.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;
        //DressUp();
        foreach(Dress dress in dresses)
        {
            AttachBipedObject(transform, Instantiate(dress.dressSlot.gameObject, dressContainer));

            //EnableBodyParts(dress, false);
        }
    }

    public static GameObject AttachBipedObject(Transform transform, GameObject armorObject)
    {
        Transform dressContainer = transform.Find("Dresses");
        if(dressContainer == null)
        {
            dressContainer = new GameObject("Dresses").transform;
            //Debug.Log($"<color=grey>Created cloth holder on actor '{transform.gameObject.name}'</color>");
        }
        dressContainer.SetParent(transform);
        dressContainer.localPosition = Vector3.zero;
        dressContainer.localRotation = Quaternion.identity;
        armorObject.transform.SetParent(dressContainer);
        armorObject.transform.localPosition = Vector3.zero;
        armorObject.transform.localRotation = Quaternion.identity;
        //Stitcher.Stitch(armorObject, agent.skeletonRoot.gameObject);
        SkinnedMeshRenderer armorSMR = armorObject.GetComponentInChildren<SkinnedMeshRenderer>();
        SkinnedMeshRenderer charSMR = transform.GetComponentInChildren<SkinnedMeshRenderer>();

        List<Transform> matchingBones = new List<Transform>(100);
        Profiler.BeginSample("DRESSUP Set bones");
        for(int a = 0; a < armorSMR.bones.Length; a++)
        {
            for(int c = 0; c < charSMR.bones.Length; c++)
            {
                if(armorSMR.bones[a] != null && armorSMR.bones[a].name == charSMR.bones[c].name)
                    matchingBones.Add(charSMR.bones[c]);
            }
        }
        Profiler.EndSample();

        armorSMR.bones = matchingBones.ToArray();
        armorSMR.rootBone = charSMR.rootBone;

        //Texture2D mainTexture = BodyTextureOcclusion.RebuildTexture((Texture2D)originRenderer.material.mainTexture, cullingTextures, 10);

        //originRenderer.material.mainTexture = mainTexture;
        //Profiler.BeginSample("DRESSUP RebuildTexture");
        //Texture2D occludedMainTex = RebuildTexture(charSMR.material.mainTexture as Texture2D, new Texture2D[] { _occlusionTex });
        //Profiler.EndSample();
        //charSMR.material.mainTexture = occludedMainTex;
        return armorObject;
    }

    private static void EnableBodyParts(Dress dress, bool enable)
    {
        //foreach(EquipmentSlot slot in dress.equipmentSlots)
        //{
        //    switch(slot)
        //    {
        //        case EquipmentSlot.Head:

        //            if(_headSlot != null)
        //            {
        //                _headSlot.enabled = enable;
        //            }

        //            break;
        //        case EquipmentSlot.Hands:

        //            if(_handsSlot != null)
        //            {
        //                _handsSlot.enabled = enable;
        //            }

        //            break;
        //        case EquipmentSlot.Body:

        //            if(_bodySlot != null)
        //            {
        //                _bodySlot.enabled = enable;
        //            }

        //            break;
        //        case EquipmentSlot.Bottom:

        //            if(_bottomSlot != null)
        //            {
        //                _bottomSlot.enabled = enable;
        //            }

        //            break;
        //        case EquipmentSlot.Feet:

        //            if(_feetSlot != null)
        //            {
        //                _feetSlot.enabled = enable;
        //            }

        //            break;
        //    }
        //}
    }

    public void DressUp()
    {
        // Create an object as a parent for all dresses, to keep it tidy


        foreach(Dress dress in dresses)
        {
            if(dress.equipped)
                continue;

            if(debug)
                Debug.Log(gameObject.name + ": Instantiating cloth '<color=orange>" + dress.dressSlot.name + "</color>'");

            Transform dressObj = Instantiate(dress.dressSlot, dressContainer);
            dress.CollectObject(dressObj.gameObject);

            if(dress.isProp)
            {
                if(dress.propParentBone != null)
                {
                    dressObj.transform.SetParent(dress.propParentBone);
                    dressObj.localPosition = Vector3.zero;
                    if(dress.inheritParentRotation)
                        dressObj.localRotation = Quaternion.identity;
                    else
                    {
                        //dressObj.
                    }
                }
            }
            else
            {
                SkinnedMeshRenderer[] dressSkinnedMeshRenderers = dressObj.GetComponentsInChildren<SkinnedMeshRenderer>();

                dressObj.transform.SetParent(dressContainer);

                //foreach(SkinnedMeshRenderer sRenderer in dressSkinnedMeshRenderers)
                //{
                //    if(debug)
                //        Debug.Log(gameObject.name + ": Synchronizing cloth bones '<color=orange>" + sRenderer.gameObject.name + "</color>'");

                //    switch(dress.equipmentSlot)
                //    {
                //        case EquipmentSlot.Head:
                //            break;
                //        case EquipmentSlot.Hands:
                //            break;
                //        case EquipmentSlot.Body:
                //            break;
                //        case EquipmentSlot.Bottom:
                //            break;
                //        case EquipmentSlot.Feet:
                //            break;
                //    }

                //    List<Transform> matchingBones = new List<Transform>();

                //    //for(int d = 0; d < sRenderer.bones.Length; d++)
                //    //    for(int c = 0; c < m_characterSMRenderer.bones.Length; c++)
                //    //        if(sRenderer.bones[d].name == m_characterSMRenderer.bones[c].name)
                //    //            matchingBones.Add(m_characterSMRenderer.bones[c]);

                //    // At this point we have all matching bones in the right order, so we can simply assign them to the dress bones
                //    sRenderer.bones = matchingBones.ToArray();

                //    if(debug)
                //        Debug.Log(gameObject.name + ": Found <color=orange>" + matchingBones.Count + "</color> bones");
                //}
            }

            dress.equipped = true;
        }
    }

    public void ToggleDress()
    {
        foreach(Dress dress in dresses)
            for(int i = 0; i < dress.dressObjects.Count; i++)
            {
                GameObject garbageObj = dress.dressObjects[i];

                garbageObj.SetActive(garbageObj.activeInHierarchy == false);
            }
    }

    public void RemoveAllDresses()
    {
        foreach(Dress dress in dresses)
        {
            for(int i = 0; i < dress.dressObjects.Count; i++)
            {
                GameObject garbageObj = dress.dressObjects[i];

                DestroyImmediate(garbageObj);
            }

            dress.dressObjects = null;
        }
    }

    static public Texture2D RebuildTexture(Texture2D sourceTexture, Texture2D[] cullingTextures, int cullFactor = 100)
    {
        if(sourceTexture == null)
        {
            Debug.LogError("Main texture is null!");
            return null;
        }
        Profiler.BeginSample("DRESSUP new Texture2D");
        Texture2D texture2DModified = new Texture2D(sourceTexture.width, sourceTexture.height);
        Profiler.EndSample();
        Color32[] mainPixels = sourceTexture.GetPixels32();

        Color32 alphaPixel = new Color32(0, 0, 0, 0);
        Profiler.BeginSample("DRESSUP foreach::Texture2D");
        foreach(Texture2D item in cullingTextures)
        {
            Color32[] cullingPixels = item.GetPixels32();

            if(mainPixels.Length != cullingPixels.Length)
            {
                Debug.LogError("Main texture and culling texture '" + item.name + "' are not same size!");
                continue;
            }

            for(int i = 0; i < cullingPixels.Length; i++)
            {
                if(cullingPixels[i].r > cullFactor)
                {
                    Profiler.BeginSample("DRESSUP foreach::Texture2D");
                    mainPixels[i] = alphaPixel;
                    Profiler.EndSample();
                }
            }
        }
        Profiler.EndSample();
        Profiler.BeginSample("DRESSUP texture2DModified.SetPixels32");
        texture2DModified.SetPixels32(mainPixels);
        Profiler.EndSample();
        Profiler.BeginSample("DRESSUP texture2DModified.Apply");
        texture2DModified.Apply();
        Profiler.EndSample();
        return texture2DModified;
    }
}

[Serializable]
public class Dress
{
    //public ClothType clothType;
    [Tooltip("What bodypart to hide when wearing this dress")]
    public EquipmentSlot[] equipmentSlots;

    public List<GameObject> dressObjects;

    [Header("Settings")] public Transform dressSlot;
    public bool isProp;
    public Transform propParentBone;
    public bool inheritParentRotation;
    [Header("Debugging")] public bool equipped;


    public void CollectObject(GameObject go)
    {
        if(dressObjects == null)
            dressObjects = new List<GameObject>();

        dressObjects.Add(go);
    }
}