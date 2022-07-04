using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIClickHandler : MonoBehaviour, IPointerClickHandler
{
    public Action OnClick;
    public Action OnDoubleClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (eventData.clickCount == 1)
                OnClick?.Invoke();
            else if (eventData.clickCount == 2)
                OnDoubleClick?.Invoke();
        }
    }
}