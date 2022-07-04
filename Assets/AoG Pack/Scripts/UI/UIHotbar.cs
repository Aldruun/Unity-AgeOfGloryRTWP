using UnityEngine;
using UnityEngine.UI;

public class UIHotbar : MonoBehaviour
{
    private RectTransform _hotbarRectTransform;

    private Button[] hotbarButtons;
    private UIInventorySlot[] hotbarSlots;

    private int _numDown = -1;

    private void OnEnable()
    {
    }

    // Start is called before the first frame update
    private void Start()
    {
        _hotbarRectTransform = transform.Find("Hotbar").GetComponent<RectTransform>();

        hotbarButtons = _hotbarRectTransform.GetComponentsInChildren<Button>();
        hotbarSlots = _hotbarRectTransform.GetComponentsInChildren<UIInventorySlot>();

        //for(int i = 0; i < hotbarButtons.Length; i++)
        //{
        //    hotbarButtons[i].onClick.AddListener(() => { ActivateHotkey(i); });
        //}
    }

    private void Update()
    {
        //int num;
        if(Input.inputString != "")
        {
            int number;
            bool is_a_number = System.Int32.TryParse(Input.inputString, out number);
            if(is_a_number && number >= 1 && number < 9)
            {
                //if(_numDown == number)
                //    return;

                //_numDown = number;
                if(UIInventory.Instance.Activated)
                    return;

                if(Input.GetKeyDown(number.ToString()))
                {
                    ActivateHotkey(number);
                }
            }
            //else if(Event.current.type == EventType.KeyUp)
            //{
            //    _numDown = Event.current.keyCode - KeyCode.Alpha1 + 1;
            //    if(_numDown == num)
            //    {
            //        _numDown = 0;
            //        Event.current.Use();
            //    }
        }

        //if(_numDown > -1 && Input.GetKeyUp(_numDown.ToString()))
        //{
        //    Debug.Log("KeyUp");
        //    _numDown = -1;
        //}
    }

    private void ActivateHotkey(int index) // 0 - 7
    {
        Debug.Log("Activated Hotkey " + index);
    }
}