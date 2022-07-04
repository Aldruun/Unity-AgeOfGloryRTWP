using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIItemActionInfo : MonoBehaviour
{
    private TextMeshProUGUI _infoUIElement;
    private string _action;
    private string _itemName;
    private int _stackSize;
    public bool dropping { get; private set; }

    private Color col;
    private float _elapsedTime = 0;
    private float _timer = 2;

    private void OnEnable()
    {
        col = _infoUIElement.color;
        col.a = 1;
        _infoUIElement.color = col;
        _elapsedTime = 0;
        _timer = 2;

        StartCoroutine(CR_FadeOut());
    }

    // Update is called once per frame
    //void Update()
    //{
        //if(_timer > 0)
        //{
        //    _timer -= Time.deltaTime;
        //    return;
        //}

        //if(_elapsedTime < 4.0f)
        //{
        //    _elapsedTime += Time.deltaTime;
        //    col = _infoUIElement.color;
        //    col.a = Mathf.Lerp(col.a, 0, _elapsedTime / 4.0f);
        //    _infoUIElement.color = col;
        //}
            
        
    //}

    private IEnumerator CR_FadeOut()
    {
        yield return new WaitForSeconds(2);

        while(_elapsedTime < 3)
        {
            _elapsedTime += Time.deltaTime;
            col = _infoUIElement.color;
            col.a = Mathf.Lerp(col.a, 0, _elapsedTime);
            _infoUIElement.color = col;
            yield return null;
        }
        //for(float t = 0f; t < 3; t += Time.deltaTime)
        //{
        //    float normalizedTime = t / 3;
        //    col = _infoUIElement.color;
        //    col.a = Mathf.Lerp(col.a, 0, normalizedTime);
        //    _infoUIElement.color = col;
        //    yield return null;
        //}
        gameObject.SetActive(false);
    }


    public void SetActionText_PickedUpItem(string itemName, int stackSize)
    {
        if(_infoUIElement == null)
        {
            _infoUIElement = GetComponent<TextMeshProUGUI>();
        }

        dropping = false;
        _stackSize = stackSize;
        _itemName = itemName;
        _infoUIElement.text = "Picked up " + itemName + (stackSize > 1 ? " x " + stackSize : "");
    }
    public void SetActionText_DroppedItem(string itemName, int stackSize)
    {
        if(_infoUIElement == null)
        {
            _infoUIElement = GetComponent<TextMeshProUGUI>();
        }

        dropping = true;
        _stackSize = stackSize;
        _itemName = itemName;
        _infoUIElement.text = "Dropped " + itemName + (stackSize > 1 ? " x " + stackSize : "");
    }
}
