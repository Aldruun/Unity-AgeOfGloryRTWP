using AoG.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AoG.UI
{
    public class ResizePanel : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public RectTransform resizeHandle;

        public Vector2 minSize;
        public Vector2 maxSize;
        //Vector2 _startMousePos;
        private RectTransform rectTransform;
        private Vector2 currentPointerPosition;
        private Vector2 previousPointerPosition;

        private float ratio;
        private Vector2 sizeDelta;

        private void Awake()
        {
            rectTransform = transform.parent.GetComponent<RectTransform>();
            //float originalWidth;
            //float originalHeight;
            //originalWidth = rectTransform.rect.width;
            //originalHeight = rectTransform.rect.height;
            //ratio = originalHeight / originalWidth;
            //minSize = new Vector2(0.1f * originalWidth, 0.1f * originalHeight);
            //maxSize = new Vector2(10f * originalWidth, 10f * originalHeight);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(GameEventSystem.isAimingSpell)
                return;
            //if(Input.mousePosition.y < sizeDelta.y + 5)
            //{
            //    UIHandler.SetCursor(CursorType.RESIZEHOR);
            //}
            //else/* if(Input.mousePosition.x < sizeDelta.x + 5)*/
            //{
            //GameStateManager.Instance.GetUIScript().SetCursor(CursorType.RESIZEVERT);
            //}

        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(GameEventSystem.isAimingSpell)
                return;

            //GameStateManager.Instance.GetUIScript().SetCursor(CursorType.NORMAL);
        }

        public void OnPointerDown(PointerEventData data)
        {
            //_startMousePos = Input.mousePosition;
            //rectTransform.SetAsLastSibling();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, data.position, data.pressEventCamera, out previousPointerPosition);
        }

        public void OnDrag(PointerEventData data)
        {
            if(rectTransform == null)
                return;
            //GameStateManager.Instance.GetUIScript().SetCursor(CursorType.RESIZEVERT);

            sizeDelta = rectTransform.sizeDelta;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, data.position, data.pressEventCamera, out currentPointerPosition);

            sizeDelta.y = currentPointerPosition.y;
            sizeDelta = new Vector2(
                Mathf.Clamp(sizeDelta.x, minSize.x, maxSize.x),
                Mathf.Clamp(sizeDelta.y, minSize.y, maxSize.y)
                );

            rectTransform.sizeDelta = sizeDelta;

            previousPointerPosition = currentPointerPosition;

        }

    }
}