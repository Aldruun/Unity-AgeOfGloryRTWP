using UnityEngine;
using UnityEngine.UI;

public class UIToggleController : MonoBehaviour
{
    public Color activeColor = Color.green;
    public GameObject buttonGroupParent;
    public Button[] buttons;

    public Button cachedActiveButton;
    public Color inactiveColor = Color.grey;

    public int initialToggleIndex;

    private void OnEnable()
    {
        buttons = buttonGroupParent.GetComponentsInChildren<Button>();

        //for (var i = 0; i < buttons.Length; i++)
        //{
        //    var button = buttons[i];
        //    button.onClick.AddListener(() => SetButtonActive(button));
        //    if (i == initialToggleIndex)
        //    {
        //        //Debug.Log("Toggle " + i + " on");
        //        if (i > 0)
        //            cachedActiveButton = button;
        //        button.onClick.Invoke();
        //    }

        //    //toggle.image.color = color
        //}

        //UIHandler.OnSpacebarPressed += ToggleTimeStop;
        //UIHandler.OnGameSpeedChanged += SetButtonActive;
    }

    private void OnDisable()
    {
        //for (var i = 0; i < buttons.Length; i++)
        //{
        //    var button = buttons[i];
        //    button.onClick.RemoveAllListeners();
        //    //if(i == initialToggleIndex || (i == toggles.Length && initialToggleIndex > toggles.Length)) {

        //    //Debug.Log("Toggle " + i + " on");
        //    //button.isOn = false;
        //    //}
        //    //toggle.image.color = color
        //}

        //UIHandler.OnSpacebarPressed -= ToggleTimeStop;
        //UIHandler.OnGameSpeedChanged -= SetButtonActive;
    }

    private void SetButtonActive(Button button)
    {
        for (var i = 0; i < buttons.Length; i++)
            if (buttons[i] == button)
                button.image.color = activeColor;
            else
                buttons[i].image.color = inactiveColor;
    }

    private void SetButtonActive(int speed)
    {
        switch (speed)
        {
            case 0:
                SetButtonActive(buttons[0]);
                break;
            case 1:
                SetButtonActive(buttons[1]);
                break;
            case 2:
                SetButtonActive(buttons[2]);
                break;
            case 4:
                SetButtonActive(buttons[3]);
                break;
        }
    }

    private void ToggleTimeStop()
    {
        var button = GetActiveButton();

        if (buttons[0] == button)
        {
            SetButtonActive(cachedActiveButton);
            cachedActiveButton.onClick.Invoke();
        }
        else
        {
            cachedActiveButton = button;
            SetButtonActive(buttons[0]);
            buttons[0].onClick.Invoke();
        }
    }

    private Button GetActiveButton()
    {
        for (var i = 0; i < buttons.Length; i++)
            if (buttons[i].image.color == activeColor)
                return buttons[i];

        return null;
    }
}