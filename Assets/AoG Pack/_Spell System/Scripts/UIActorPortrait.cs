using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum AlignmentColor
{
    Neutral,
    PC,
    Hostile,
    ImportantNPC
}

public class UIActorPortrait : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Action<bool> OnPortraitEnterCallback;
    public float doubleClickGap = 0.4f;
    internal int partyIndex;
    private RectTransform rectTransform;
    private Slider healthbar;
    private Image actionIcon;
    private Text onOverHPText;

    private Color selectedColor;
    private Color defaultColor;
    private Color storedNormalColor;

    private bool isSpeaking;
    private bool highlighted;
    private int lerp_state = 1;
    private float lerp_time;
    private float lastClick;
    private Image selectionOverlay;
    private Transform pcTransform;
    private Gradient flashGradient;

    private bool active;
    public bool Active
    {
        get => active;
        set {

            active = value;
            RectTransform rt = rectTransform;
            lerp_time = 0;
            // Make selected portrait a little bit bigger
            rt.localScale = active ? new Vector3(1.03f, 1.03f, 1) : new Vector3(1, 1, 1);
            //GetComponent<Image>().set = active ? 2f : 1.5f;
            if(selectionOverlay != null)
            {
                selectionOverlay.color = active ? new Color(0.196f, 1f, 0f) : storedNormalColor;
                selectionOverlay.rectTransform.localScale = active ? new Vector3(1.03f, 1.03f, 1) : new Vector3(1, 1, 1);
            }
        }
    }

    public void Init(int partyIndex, Transform pcTransform)
    {
        this.partyIndex = partyIndex;
        this.pcTransform = pcTransform;
        rectTransform = transform.GetComponent<RectTransform>();
        selectionOverlay = rectTransform.Find("selector").GetComponent<Image>();

        storedNormalColor = selectionOverlay.color;

        selectedColor = Color.green;
        defaultColor = selectedColor * 0.6f;

        onOverHPText = transform.Find("highlight content/hp text").GetComponent<Text>();
        onOverHPText.enabled = false;

        healthbar = transform.Find("portrait/healthbar").GetComponent<Slider>();
        actionIcon = transform.Find("action icon").GetComponent<Image>();
        EnableActionIcon(false);
    }

    public int GetLinkedPartyIndex()
    {
        return partyIndex;
    }

    internal void ResetHighlighting()
    {
        lerp_time = 0;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if((lastClick + doubleClickGap) > Time.time)
            {
                //Debug.Log("Double clicked");
                GameEventSystem.RequestCameraJumpToPosition?.Invoke(pcTransform.position);
            }
            else
            {
                lastClick = Time.time;
            }
        }
        else if(eventData.button == PointerEventData.InputButton.Right)
        {
            GameEventSystem.RequestCameraFollowPC?.Invoke(pcTransform);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameEventSystem.OnSetPCUnderCursorForGameControl.Invoke(partyIndex);
        OnPortraitEnterCallback?.Invoke(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameEventSystem.OnSetPCUnderCursorForGameControl.Invoke(0);
        OnPortraitEnterCallback?.Invoke(false);
    }

    public void ToggleSpeaking(bool on)
    {
        if(isSpeaking == on)
        {
            return;
        }

        isSpeaking = on;

        if(on)
        {
            selectionOverlay.color = Color.white;
        }
        else
        {
            selectionOverlay.color = Active ? selectedColor : defaultColor;
        }
    }

    internal void ChangeRelationColor(AlignmentColor alignmentColor)
    {
        switch(alignmentColor)
        {
            case AlignmentColor.Neutral:
                defaultColor = selectedColor = Color.white;
                break;
            case AlignmentColor.ImportantNPC:
                defaultColor = selectedColor = Color.cyan;
                break;
            case AlignmentColor.PC:
                selectedColor = Color.green;
                defaultColor = selectedColor * 0.15f;
                break;
            case AlignmentColor.Hostile:
                defaultColor = selectedColor = Color.red;
                break;
        }

        ActorUI.SetFlashGradientColor(flashGradient, selectedColor);
        selectionOverlay.color = highlighted ? selectedColor : defaultColor;
    }

    internal void SetImage(Sprite sprite, Gradient flashGradient)
    {
        this.flashGradient = flashGradient;
        Debug.Log("Setting portrait sprite");
        rectTransform.Find("portrait").GetComponent<Image>().sprite = sprite;
    }

    internal void Disable()
    {
        throw new NotImplementedException();
    }

    //void ShowHealthInfo(bool on)
    //{
    //    _onOverHPText.text = _linkedActorUtility.GetBaseStat(ActorStat.HITPOINTS) + "/" + _linkedActorUtility.GetBaseStat(ActorStat.MAXHITPOINTS);
    //    _onOverHPText.enabled = on;
    //}
    public void UpdateColor(bool talking)
    {
        //Debug.Log("Updating portrait color");
        ToggleSpeaking(talking);

        if(isSpeaking)
        {
            selectionOverlay.color = Color.white;
        }
        else if(highlighted)
        {
            if(lerp_time < 0.6f)
            {
                float value = Mathf.Lerp(0f, 1f, lerp_time / 0.6f);
                lerp_time += Time.unscaledDeltaTime;
                Color color = flashGradient.Evaluate(value);
                selectionOverlay.color = color;
            }
            else
            {
                lerp_time = 0f;
            }
        }
    }

    internal void OverrideAlignmentColor(Color color)
    {
        ActorUI.SetFlashGradientColor(flashGradient, color);
    }


    internal void RevertToAlignmentColor()
    {
        ActorUI.SetFlashGradientColor(flashGradient, selectedColor);
    }

    internal void SetHighlighted(bool on)
    {
        highlighted = on;
    }

    private void UpdateActionIcon(Spell action)
    {
        //action.OnDone -= ClearActionIcon;
        //action.OnDone += ClearActionIcon;

        //_currCommand = action;

        //if(action == null || action is MoveAction)
        //{
        //    EnableActionIcon(false);
        //    return;
        //}
        //else if(action is AIActions.Action_CastSpellAtActor sca)
        //{
        //    _actionIcon.sprite = sca._spell.spellIcon;
        //}
        //else if(action is AIActions.Action_CastSpellAtLocation scl)
        //{
        //    _actionIcon.sprite = scl._spell.spellIcon;
        //}
        //else if(action is AIActions.Action_Attack atk)
        //{
        //    Weapon weapon = _linkedActor.Combat.GetEquippedWeapon;
        //    _actionIcon.sprite = weapon.identifier == "fist" ? ResourceManager.cursor_attack : ResourceManager.GetItemSprite(weapon.identifier);
        //}

        EnableActionIcon(true);
    }
    private void EnableActionIcon(bool on)
    {
        Color c = actionIcon.color;
        c.a = on ? 1 : 0;
        actionIcon.color = c;
    }

    public void UpdatePortraitHealthbar(float newCurrent, float currentMax)
    {
        //Debug.Log("UpdateHealthbar: " + currentHealth);
        healthbar.value = currentMax - newCurrent;
        onOverHPText.text = newCurrent + "/" + currentMax;

        healthbar.maxValue = currentMax;
        onOverHPText.text = newCurrent + "/" + currentMax;
    }
}