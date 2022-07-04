using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFormationPicker : MonoBehaviour
{
    private RectTransform _buttonLayoutGroup;
    private Button _mainButton;
    private Button[] _formationPickerButtons;

    private void Start()
    {
        _buttonLayoutGroup = transform.Find("Layout Group").GetComponent<RectTransform>();
        _formationPickerButtons = _buttonLayoutGroup.transform.GetComponentsInChildren<Button>();
        _mainButton = transform.Find("MainButton").GetComponent<Button>();
        _mainButton.onClick.AddListener(() =>
        {
            _buttonLayoutGroup.gameObject.SetActive(_buttonLayoutGroup.gameObject.activeInHierarchy == false);

        });
        foreach(Button button in _formationPickerButtons)
        {
            button.onClick.AddListener(() => { _mainButton.image.sprite = button.image.sprite; _buttonLayoutGroup.gameObject.SetActive(false); /*FormationController.SetFormation();*/ });
            
        }
        _buttonLayoutGroup.gameObject.SetActive(false);
    }
}
