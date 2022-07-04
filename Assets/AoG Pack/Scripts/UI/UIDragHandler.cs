using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIDragHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 _beginDragOffset;
    private bool _dragging;
    public RectTransform dragRectTransform;
    private RectTransform _canvasRect;

    public UnityEvent OnDragStart;

    private void Start()
    {
        _canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            _dragging = true;
            OnDragStart?.Invoke();
            StartCoroutine(CR_UpdateDragging());
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            if (_dragging)
                _dragging = false;
    }
    private IEnumerator CR_UpdateDragging()
    {
        _beginDragOffset = dragRectTransform.position - Input.mousePosition;

        while (_dragging)
        {
            dragRectTransform.position = Input.mousePosition + _beginDragOffset;
            ClampToScreen();
            yield return null;
        }
    }

    private void ClampToScreen(/*Vector3 newPos*/)
    {
        var sizeDelta = _canvasRect.sizeDelta - dragRectTransform.sizeDelta;
        var panelPivot = dragRectTransform.pivot;
        var position = dragRectTransform.anchoredPosition;
        position.x = Mathf.Clamp(position.x, -sizeDelta.x * panelPivot.x, sizeDelta.x * (1 - panelPivot.x));
        position.y = Mathf.Clamp(position.y, -sizeDelta.y * panelPivot.y, sizeDelta.y * (1 - panelPivot.y));
        dragRectTransform.anchoredPosition = position;

        //float minX = (_canvasRect.sizeDelta.x - dragRectTransform.sizeDelta.x) * -0.5f;
        //float maxX = (_canvasRect.sizeDelta.x - dragRectTransform.sizeDelta.x) * 0.5f;
        //float minY = (_canvasRect.sizeDelta.y - dragRectTransform.sizeDelta.y) * -0.5f;
        //float maxY = (_canvasRect.sizeDelta.y - dragRectTransform.sizeDelta.y) * 0.5f;

        //newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        //newPos.y = Mathf.Clamp(newPos.y, minY, maxY);

        //return newPos;
    }

}