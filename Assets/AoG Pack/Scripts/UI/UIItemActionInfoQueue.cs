using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIItemActionInfoQueue : MonoBehaviour
{
    public List<UIItemActionInfo> _infoUIElements;
    //IEnumerator _queueRoutine;

    // Start is called before the first frame update
    private void Start()
    {
        _infoUIElements = GetComponentsInChildren<UIItemActionInfo>(true).ToList();

        GameEventSystem.RefreshUI_PlayerPickedUpItem += QueueItemCollectedNotification;
        GameEventSystem.OnPlayerDropItem += QueueItemDroppedNotification;
    }

    private void OnDisable()
    {
        GameEventSystem.RefreshUI_PlayerPickedUpItem -= QueueItemCollectedNotification;
        GameEventSystem.OnPlayerDropItem -= QueueItemDroppedNotification;
    }

    private void QueueItemCollectedNotification(string identifier, int stackSize)
    {
        //itemActionQueue.Add(new KeyValuePair<string, int>("Picked up " + ItemDatabase.GetItemName(identifier) + " x " + stackSize, stackSize));
        foreach(var element in _infoUIElements)
        {
            if(element.gameObject.activeInHierarchy == false)
            {
                element.SetActionText_PickedUpItem(ItemDatabase.GetItemName(identifier), stackSize);
                element.gameObject.SetActive(true);
                break;
            }
        }
        //if(_routineRunning == false)
        //{
        //StartCoroutine(CR_QueueRoutine("Picked up " + ItemDatabase.GetItemName(identifier) + " x " + stackSize, stackSize));
        //}
    }

    private void QueueItemDroppedNotification(string identifier, int stackSize)
    {
        //itemActionQueue.Add(new KeyValuePair<string, int>("Dropped " + ItemDatabase.GetItemName(identifier) + " x " + stackSize, stackSize));
        foreach(var element in _infoUIElements)
        {
            if(element.gameObject.activeInHierarchy == false)
            {
                element.SetActionText_DroppedItem(ItemDatabase.GetItemName(identifier), stackSize);
                element.gameObject.SetActive(true);
                break;
            }
        }
        //if(_routineRunning == false)
        //{
        //StartCoroutine(CR_QueueRoutine("Dropped " + ItemDatabase.GetItemName(identifier) + " x " + stackSize, stackSize));
        //}
    }



    //TMPro.TextMeshProUGUI GetAvailableTextElement()
    //{
    //    return _infoUIElements.Where(t => t.gameObject.activeInHierarchy == false).FirstOrDefault();
    //}
}
//public class UIItemActionInfoQueue : MonoBehaviour
//{
//    List<KeyValuePair<string, int>> itemActionQueue;
//    public TMPro.TextMeshProUGUI _infoUIElement;
//    //IEnumerator _queueRoutine;
//    bool _routineRunning;
//    // Start is called before the first frame update
//    void Start()
//    {
//        itemActionQueue = new List<KeyValuePair<string, int>>();

//        _infoUIElement = GetComponentInChildren<TMPro.TextMeshProUGUI>(true);

//        GameEventSystem.OnPlayerPickedUpItem += QueueItemCollectedNotification;
//        GameEventSystem.OnPlayerDropItem += QueueItemDroppedNotification;
//    }

//    void OnDisable()
//    {
//        GameEventSystem.OnPlayerPickedUpItem -= QueueItemCollectedNotification;
//        GameEventSystem.OnPlayerDropItem -= QueueItemDroppedNotification;
//    }

//    void QueueItemCollectedNotification(string identifier, int stackSize)
//    {
//        itemActionQueue.Add(new KeyValuePair<string, int>("Picked up " + ItemDatabase.GetItemName(identifier) + " x " + stackSize, stackSize));

//        if(_routineRunning == false)
//        {
//            StartCoroutine(CR_QueueRoutine());
//        }
//    }

//    void QueueItemDroppedNotification(string identifier, int stackSize)
//    {
//        itemActionQueue.Add(new KeyValuePair<string, int>("Dropped " + ItemDatabase.GetItemName(identifier) + " x " + stackSize, stackSize));

//        if(_routineRunning == false)
//        {
//            StartCoroutine(CR_QueueRoutine());
//        }
//    }

//    IEnumerator CR_QueueRoutine()
//    {
//        _routineRunning = true;

//        Color col = _infoUIElement.color;
//        col.a = 1;
//        _infoUIElement.color = col;

//        while(itemActionQueue.Count > 0)
//        {
//            _infoUIElement.text = itemActionQueue[0].Key;

//            yield return new WaitForSeconds(2);
//            itemActionQueue.RemoveAt(0);
//        }

//        float targetValue = 0;
//        float elapsedTime = 0;


//        while(elapsedTime < 2 /*mainPanel.anchoredPosition.y < targetScrollValue*/)
//        {
//            elapsedTime += Time.deltaTime;
//            col = _infoUIElement.color;
//            col.a = Mathf.Lerp(col.a, targetValue, elapsedTime / 2);
//            _infoUIElement.color = col;
//            yield return null;
//        }
//        //if(scrollIn == false)
//        //{
//        //    _inventoryPanel.gameObject.SetActive(false);
//        //}

//        _infoUIElement.text = "";
//        _routineRunning = false;
//    }

//    //TMPro.TextMeshProUGUI GetAvailableTextElement()
//    //{
//    //    return _infoUIElements.Where(t => t.gameObject.activeInHierarchy == false).FirstOrDefault();
//    //}
//}