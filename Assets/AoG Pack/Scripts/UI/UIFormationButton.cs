using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFormationButton : MonoBehaviour
{
    public FormationType formation;
    Button _button;

    // Start is called before the first frame update
    void Awake()
    {
        _button = GetComponent<Button>();
        //_button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() => { GameEventSystem.SetFormation?.Invoke(formation); /*Debug.Log("____________Added");*/ });
    }
}
