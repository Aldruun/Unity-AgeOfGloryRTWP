using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;

public class DevConsole : MonoBehaviour/*, IBeginDragHandler*/
{
    public bool log;

    public static DevConsole Instance;
    private static object _prevContent;
    private static Text _currStack;
    public RectTransform contentPanel;
    public GameObject textPrefab;
    private static ScrollRect _scrollRect;
    private static int _maxLogs = 20;
    private static List<Text> logs;
    private StringBuilder _strBld;
    private static int _maxLogsPerSB = 15;
    private static int _logsCount;
    private static Text _textObj;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Debug.LogError(gameObject.name + ": DevConsole: Only one instance allowed");
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        _scrollRect = GetComponentInChildren<ScrollRect>();
        logs = new List<Text>();
        _scrollRect.onValueChanged.AddListener((v) => StopCoroutine(ScrollToBottom()));
        _strBld = new StringBuilder();
        _textObj = Instantiate(Instance.textPrefab, Instance.contentPanel).GetComponent<Text>();
    }

    public static void Log(string content)
    {
        if(Instance == null || Instance.log == false)
            return;

        _logsCount++;

        bool startedAtBottom = _scrollRect.normalizedPosition == new Vector2(0, 0);

        if(_prevContent != null && content.Equals(_prevContent))
            return;

        if(_logsCount % _maxLogsPerSB == 0)
        {
            _textObj = Instantiate(Instance.textPrefab, Instance.contentPanel).GetComponent<Text>();
        }

        //logs.Add(textObj);

        //if(logs.Count == _maxLogs)
        //{
        //    for(int i = 0; i < 10; i++)
        //    {
        //        var t = logs[i];
        //        Destroy(t.gameObject);
        //        logs.RemoveAt(i);
        //    }
        //}

        //_prevContent = content;
        _textObj.text += content + "\n"/*.ToString()*/;

        //if(startedAtBottom)
        //    _scrollRect.normalizedPosition = new Vector2(0, 0);

        Instance.StartCoroutine(Instance.ScrollToBottom());
    }

    private IEnumerator ScrollToBottom()
    {
        yield return new WaitForSeconds(0.1f);
        //float currPos = _scrollRect.verticalNormalizedPosition;
        float totalHeight = _scrollRect.transform.parent.GetComponent<RectTransform>().sizeDelta.y;// find total width of scroll rect transform.
        float targetValue = 80;
        float targetPercentage = targetValue / totalHeight;

        //_scrollRect.horizontalNormalizedPosition = targetPercentage;
        //if(_scrollRect.verticalNormalizedPosition > 1 - targetPercentage)
        //{
        //    yield break;
        //}
        float start = _scrollRect.verticalNormalizedPosition;
        //var t = 0.0f;
        //while(t < 1.0f)
        //{
        //    t += Time.deltaTime;
        //    Mathf.Lerp(start, 0, t);
        //    _scrollRect.verticalNormalizedPosition = start;
        //   yield return null;
        //}
        for(float t = 0f; t < .5f; t += Time.deltaTime)
        {
            _scrollRect.verticalNormalizedPosition = Mathf.Lerp(start, 0f, t / .5f);
            yield return null;
        }

    //public void OnBeginDrag(PointerEventData eventData)
    //{
    //    StopCoroutine("ScrollToBottom");
    }
}

public struct DevConsoleContentData
{
    public object content;
    public string stackSize;

    public DevConsoleContentData(object content, string stackSize)
    {
        this.content = content;
        this.stackSize = stackSize;
    }
}