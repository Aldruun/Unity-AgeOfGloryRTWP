using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class UISpellButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Actor _caster;
    internal Spell spell;

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameEventSystem.RequestShowTooltip(GetComponent<RectTransform>().position, new Vector3(0, 25, 0),
            "<color=cyan>" + spell.Name + "</color>\n\n" +
            spell.description + "\n\n" +
            "Range: " + spell.activationRange + "\n\n" +
            "Dmg: " + spell.damageRollDice
            );
        //GetComponent<TooltipTrigger>().text = "<color=cyan>" + skill.skillName + "</color>\n\n" +
        //    skill.description + "\n\n" +
        //    "Dmg: " + skill.effectValue;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameEventSystem.RequestHideTooltip?.Invoke();

    }

    internal void ToggleSelected(bool select)
    {
        transform.Find("glow frame").GetComponent<Image>().enabled = select;
    }
}
