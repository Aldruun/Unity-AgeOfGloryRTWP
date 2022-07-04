using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldMap
{
    private RectTransform _playerIcon;
    private List<MapLocation> mapLocations;

    public WorldMap()
    {
        mapLocations = new List<MapLocation>();
        SetMapIcons();
        MarkPlayerArea();
    }

    private void SetMapIcons()
    {
        Transform worldMapPanel = GameObject.FindWithTag("UIMaster").transform.Find("Canvas - Player Interface/World Map");
        _playerIcon = worldMapPanel.Find("wmpplayer").GetComponent<RectTransform>();
        Button[] locationIconButtons = worldMapPanel.Find("Locations").GetComponentsInChildren<Button>();
        foreach(Button button in locationIconButtons)
        {
            MapLocation mapLoc = new MapLocation(button.transform.GetChild(0).name, SceneManager.GetSceneByName(button.name), button.image);

            if(mapLocations.Contains(mapLoc) == false)
                mapLocations.Add(mapLoc);

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => SceneManager.LoadScene(button.name));
        }
    }

    private void MarkPlayerArea()
    {
        foreach(var item in mapLocations)
        {
            if(item.linkedScene.buildIndex == SceneManager.GetActiveScene().buildIndex)
            {
                _playerIcon.anchoredPosition = item.mapIcon.rectTransform.anchoredPosition;
                break;
            }
        }
    }
}

public class MapLocation
{
    public string Name;
    public Scene linkedScene;
    public Image mapIcon;

    public MapLocation(string name, Scene linkedScene, Image mapIcon)
    {
        Name = name;
        this.linkedScene = linkedScene;
        this.mapIcon = mapIcon;
    }
}