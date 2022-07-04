using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINotificationCenter : MonoBehaviour
{
    public static UINotificationCenter Instance;
    public GameObject notificationTextPrefab;
    private static List<RectTransform> _cachedTextObjects;
    private Transform _contentHolder;

    private void Awake()
    {
        if(Instance == null)
        {
            //DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if(Instance != this)
        {
            Debug.LogError("Destroying UINotificationCenter -> Only one instance allowed");
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        _contentHolder = transform.Find("scroll start");
        _cachedTextObjects = new List<RectTransform>();

        for(int i = 0; i < 5; i++)
        {
            RectTransform rt = Instantiate(notificationTextPrefab, _contentHolder).GetComponentInChildren<RectTransform>();
            rt.gameObject.SetActive(false);
            _cachedTextObjects.Add(rt);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public static void NotifySingleLine(string notification, float initialScreenPosY)
    {
        RectTransform newTextObj = null;
        foreach(RectTransform textObject in _cachedTextObjects)
        {
            if(textObject.gameObject.activeInHierarchy == false)
            {
                newTextObj = textObject;
                newTextObj.gameObject.SetActive(true);
                break;
            }
        }

        if(newTextObj == null)
        {
            newTextObj = Instantiate(Instance.notificationTextPrefab, Instance._contentHolder).GetComponentInChildren<RectTransform>();
            newTextObj.gameObject.SetActive(true);
            _cachedTextObjects.Add(newTextObj);
            Debug.Log("<color=grey>Created new ui notification object</color>");
        }

        TMPro.TextMeshProUGUI text = newTextObj.GetComponent<TMPro.TextMeshProUGUI>();
        text.text = notification;

        Instance.StartCoroutine(Instance.CR_ScrollAndFadeOut(text, initialScreenPosY, 0.1f, 10, 3, 0.3f));
    }

    private IEnumerator CR_ScrollAndFadeOut(TMPro.TextMeshProUGUI text, float initialScreenPosY, float delay, float scrollSpeed, float fadeStartTime, float fadeOutDuration)
    {
        yield return new WaitForSeconds(delay);
       
        //float startScrollValue = mainPanel.position.y;

        float elapsedTime = 0;
        float durVisibleTimer = fadeStartTime;
        float y = initialScreenPosY;
        float t = 0;
        bool done = false;

        Vector2 pos = text.rectTransform.anchoredPosition;
        pos.y = y;
        text.GetComponent<RectTransform>().anchoredPosition = pos;
        Color color = text.color;
        color.a = 1;
        text.color = color;

        while(done == false)
        {
            durVisibleTimer -= Time.unscaledDeltaTime;
            y += Time.unscaledDeltaTime * scrollSpeed;
            elapsedTime += Time.unscaledDeltaTime;
            pos = text.rectTransform.anchoredPosition;
            pos.y = y;
            text.GetComponent<RectTransform>().anchoredPosition = pos;

            if(durVisibleTimer <= 0)
            {
                if(t < fadeOutDuration)
                {
                    t += Time.unscaledDeltaTime;
                    // Turn the time into an interpolation factor between 0 and 1.
                    var blend = Mathf.Clamp01(t / fadeOutDuration);

                    color = text.color;
                    color.a = Mathf.Lerp(1, 0, blend);
                    text.color = color;
                    yield return null;
                }
                else
                    done = true;
            }

            yield return null;
        }

        text.gameObject.SetActive(false);

        //while(elapsedTime < scrollDuration)
        //{
        //    elapsedTime += SceneManagment.deltaTime;
        //    Vector2 pos = text.rectTransform.anchoredPosition;
        //    pos.y = Mathf.Lerp(pos.y, targetScrollValue, elapsedTime / scrollDuration);
        //    text.GetComponent<RectTransform>().anchoredPosition = pos;
        //    yield return null;
        //}
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(3, 200, 150, 30), "Notification Center");
        if(GUI.Button(new Rect(3, 230, 150, 30), "Add Notification"))
        {
            NotifySingleLine("New note", -30);
        }
    }
}
