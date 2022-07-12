using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public bool Active;
    internal Spell spell;
    private readonly Actor _caster;

    public void OnPointerClick(PointerEventData eventData)
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameEventSystem.RequestShowTooltip(GetComponent<RectTransform>().position, new Vector3(0, 25, 0),
            "<color=cyan>" + spell.Name + "</color>\n\n" +
            spell.description + "\n\n" +
            "Range: " + spell.activationRange + "\n\n" +
            "Effects: " + spell.magicEffectsData[0].effect +
            "Dmg: " + spell.magicEffectsData[0].magnitude
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
        Active = select;
        transform.Find("glow frame").GetComponent<Image>().enabled = select;
    }
}
