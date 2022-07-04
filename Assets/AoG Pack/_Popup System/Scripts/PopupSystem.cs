using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupSystem : MonoBehaviour
{
    public static PopupSystem Instance;

    public float infoFadeOutDistance = 100;
    private List<GameObject> _cachedSpritePopupObjects;
    private List<GameObject> _cachedTextPopupObjects;

    private Camera _camera;

    private void Awake()
    {
        Instance = this;
    }

    private void Start() // Called in Unity's Start() function
    {
        _cachedTextPopupObjects = new List<GameObject>();
        _cachedSpritePopupObjects = new List<GameObject>();

        //Quest.OnQuestTaken += (q, a) =>
        //{
        //    CreatePopup(a.transform, 2, ResourceManager.GetSprite("quest contract"), 5, 0, 1);
        //};

        GameEventSystem.OnRequestTextPopup = CreatePopup;

        _camera = Camera.main;
    }

    private void OnDisable()
    {
        GameEventSystem.OnRequestTextPopup = null;
    }

    private void CreatePopup(Transform anchor, float height, string context, float duration, float scrollSpeed, float fadeDuration, Color? color = null)
    {
        if(Vector3.Distance(_camera.transform.position, anchor.transform.position) > infoFadeOutDistance)
            return;

        GameObject popupObj = null;

        foreach(GameObject cachedPopupObj in _cachedTextPopupObjects)
            // Pick an inactive popup from the pool and leave the loop
            if(cachedPopupObj.activeSelf == false)
            {
                //Debug.Log("Found cached popup obj");
                popupObj = cachedPopupObj;

                break;
            }

        // No inactive popup was found in the pool, so let's create a new one
        if(popupObj == null)
        {
            popupObj = Instantiate(ResourceManager.prefab_uitextpopup,
                GameObject.FindGameObjectWithTag("PopupContainer").transform);

            _cachedTextPopupObjects.Add(popupObj);
        }


        UIPopup popup = popupObj.GetComponent<UIPopup>();

        popup.Initialize(anchor, height, context, duration, scrollSpeed, fadeDuration, color);

        //if(popup.InRange)
        popupObj.SetActive(true);
    }

    public void CreatePopup(Vector3 anchorPosition, string context, float duration, float scrollSpeed, float fadeDuration, Color? color = null)
    {
        if(Vector3.Distance(_camera.transform.position, anchorPosition) > infoFadeOutDistance)
            return;

        GameObject popupObj = null;

        foreach(GameObject cachedPopupObj in _cachedTextPopupObjects)
            if(cachedPopupObj.activeSelf == false)
            {
                //Debug.Log("Found cached popup obj");
                popupObj = cachedPopupObj;

                break;
            }

        if(popupObj == null)
        {
            popupObj = Instantiate(ResourceManager.prefab_uitextpopup, GameObject.FindGameObjectWithTag("PopupContainer").transform);

            _cachedTextPopupObjects.Add(popupObj);
        }


        UIPopup popup = popupObj.GetComponent<UIPopup>();

        popup.Initialize(anchorPosition, context, duration, scrollSpeed, fadeDuration, color);

        popupObj.SetActive(true);
    }

    private void CreatePopup(Transform anchor, float height, Sprite icon, float duration, float scrollSpeed, float fadeDuration)
    {
        if(Vector3.Distance(_camera.transform.position, anchor.transform.position) > infoFadeOutDistance)
            return;

        GameObject popupObj = null;

        foreach(GameObject cachedPopupObj in _cachedSpritePopupObjects)
            // Pick an inactive popup from the pool and leave the loop
            if(cachedPopupObj.activeSelf == false)
            {
                //Debug.Log("Found cached popup obj");
                popupObj = cachedPopupObj;

                break;
            }

        // No inactive popup was found in the pool, so let's create a new one
        if(popupObj == null)
        {
            popupObj = Instantiate(ResourceManager.prefab_uispritepopup,
                GameObject.FindGameObjectWithTag("PopupContainer").transform);

            _cachedSpritePopupObjects.Add(popupObj);
        }


        UIPopup popup = popupObj.GetComponent<UIPopup>();

        popup.Initialize(anchor, height, icon, duration, scrollSpeed, fadeDuration);

        //if(popup.InRange)
        popupObj.SetActive(true);
    }

}
