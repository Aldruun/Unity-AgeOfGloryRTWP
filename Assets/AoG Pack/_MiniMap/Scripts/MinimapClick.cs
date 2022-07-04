using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MinimapClick : MonoBehaviour, IPointerClickHandler
{
    //Drag Orthographic top down camera here
    public Camera miniMapCam;
    public RectTransform thingy;
    public Transform sceneObject;
    public RawImage _rawMinimapUIImage;

    private void Update()
    {
        Vector2 pos = miniMapCam.WorldToScreenPoint(sceneObject.position);

        float coordX, coordY;
        GetWorldCoordinates(pos, out coordX, out coordY);

        
        //Convert coordX and coordY to % (0.0-1.0) with respect to texture width and height
        float recalcX = coordX / _rawMinimapUIImage.texture.width;
        float recalcY = coordY / _rawMinimapUIImage.texture.height;

        //localCursor = new Vector2(recalcX, recalcY);

        //CastMiniMapRayToWorld(localCursor);
        thingy.anchoredPosition = new Vector2(recalcX, recalcY);
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        Vector2 localCursor = new Vector2(0, 0);

        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(_rawMinimapUIImage.rectTransform, eventData.pressPosition, eventData.pressEventCamera, out localCursor))
        {
            float coordX, coordY;
            GetWorldCoordinates(localCursor, out coordX, out coordY);

            int xpos = (int)(localCursor.x);
            int ypos = (int)(localCursor.y);
            thingy.anchoredPosition = new Vector2(xpos, ypos);
            //Convert coordX and coordY to % (0.0-1.0) with respect to texture width and height
            float recalcX = coordX / _rawMinimapUIImage.texture.width;
            float recalcY = coordY / _rawMinimapUIImage.texture.height;

            localCursor = new Vector2(recalcX, recalcY);

            CastMiniMapRayToWorld(localCursor);
        }
        //MouseDown(eventData);
    }

    private void GetWorldCoordinates(Vector2 localCursor, out float coordX, out float coordY)
    {
        Rect r = _rawMinimapUIImage.rectTransform.rect;

        //Using the size of the texture and the local cursor, clamp the X,Y coords between 0 and width - height of texture
        coordX = Mathf.Clamp(0, (((localCursor.x - r.x) * _rawMinimapUIImage.texture.width) / r.width), _rawMinimapUIImage.texture.width);
        coordY = Mathf.Clamp(0, (((localCursor.y - r.y) * _rawMinimapUIImage.texture.height) / r.height), _rawMinimapUIImage.texture.height);
    }

    private void CastMiniMapRayToWorld(Vector2 localCursor)
    {
        Ray miniMapRay = miniMapCam.ScreenPointToRay(new Vector2(localCursor.x * miniMapCam.pixelWidth, localCursor.y * miniMapCam.pixelHeight));

        RaycastHit miniMapHit;

        if(Physics.Raycast(miniMapRay, out miniMapHit, Mathf.Infinity))
        {
            sceneObject.position = miniMapHit.point;
            Debug.Log("miniMapHit: " + miniMapHit.collider.gameObject);
        }
    }
}
