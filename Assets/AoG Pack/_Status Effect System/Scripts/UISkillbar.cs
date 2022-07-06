using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UISkillbar : MonoBehaviour
{
    public bool debug;
    private Transform spellParentPanel;
    private List<UISkillButton> spellButtons;
    private UISkillButton _lastSelectedButton;

    public bool vertical;
    public float mainPanelScrollTime = 1;

    private RectTransform _transform;
    // Start is called before the first frame update
    private void Awake()
    {
        GameEventSystem.OnPartyMemberDeselected -= AdjustSpellbarVisibilityStatus;
        GameEventSystem.OnPartyMemberDeselected += AdjustSpellbarVisibilityStatus;
        GameEventSystem.OnPartyMemberSelected -= PopulateSkillbar;
        GameEventSystem.OnPartyMemberSelected += PopulateSkillbar;

        spellParentPanel = transform.Find("Content - Skill Buttons");
        _transform = GetComponent<RectTransform>();
        spellButtons = new List<UISkillButton>();

        for(int i = 0; i < 5; i++)
        {
            GameObject obj = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Skill Button"), transform.Find("Content - Skill Buttons"));
            obj.SetActive(false);
            spellButtons.Add(obj.GetComponent<UISkillButton>());
        }

        StartCoroutine("CR_ScrollSpellBar", true);
    }

    private void DeselectActivespellButton()
    {
        if(_lastSelectedButton != null)
        {
            _lastSelectedButton.ToggleSelected(false);
            _lastSelectedButton = null;
        }
    }

    private void OnDisable()
    {
        GameEventSystem.OnPartyMemberDeselected -= AdjustSpellbarVisibilityStatus;
        GameEventSystem.OnPartyMemberSelected -= PopulateSkillbar;
    }

    internal void PopulateSkillbar(Actor caster)
    {
        _lastSelectedButton = null;
        if(debug)
            Debug.Log("<color=cyan>Populating spell bar</color>");
        foreach(UISkillButton item in spellButtons)
        {
            item.ToggleSelected(false);
            item.gameObject.SetActive(false);
        }
        StopCoroutine("CR_ScrollSpellBar");
        if(SelectionManager.NumSelectedPCs() > 1)
        {

            StartCoroutine("CR_ScrollSpellBar", false);
            return;
        }

        StartCoroutine("CR_ScrollSpellBar", true);

        int spellCount = caster.Spellbook.SpellData.Count;
        for(int i = 0; i < spellCount; i++)// Skill skill in agent.behaviours.skillController.GetSkills())
        {
            Spell spell = caster.Spellbook.SpellData[i].spell;
            if(debug)
                Debug.Log("<color=cyan>Configuring spell hotkey</color>");
          
            UISkillButton sBtn = GetSpellButton();
            sBtn.spell = spell;
            Button spellButton = sBtn.GetComponent<Button>();

            if(spell.spellIcon == null)
            {
                if(debug)
                    Debug.LogError(caster.GetName() + ": spell.spellIcon = null");
            }

            spellButton.GetComponent<Image>().sprite = spell.spellIcon;
        
            spellButton.onClick.RemoveAllListeners();
            spellButton.onClick.AddListener(delegate
            {
                if(debug)
                    Debug.Log(caster.GetName() + ":<color=orange>Key " + spell.Name + " pressed</color>");             
                GameEventSystem.OnPlayerSpellButtonClicked?.Invoke(caster, spell, sBtn);

                foreach(UISkillButton spellBtn in spellButtons)
                {
                    spellBtn.ToggleSelected(false);
                }
                sBtn.ToggleSelected(true);
                _lastSelectedButton = sBtn;
            });
            Image radImage = sBtn.transform.Find("cooldown").GetComponent<Image>();
            spell.CooldownHook += (remainingCooldown) =>
            {
                radImage.fillAmount = remainingCooldown / spell.cooldownTime;
            };
        }
    }

    void AdjustSpellbarVisibilityStatus(Actor agent)
    {
        int numSelectedPCs = SelectionManager.NumSelectedPCs();

        foreach(UISkillButton item in spellButtons)
        {
            item.transform.Find("glow frame").GetComponent<Image>().enabled = false;
            item.gameObject.SetActive(false);
        }

        StopCoroutine("CR_ScrollSpellBar");

        if(numSelectedPCs == 1)
        {
            StartCoroutine("CR_ScrollSpellBar", true);
        }
        else
        {
            StartCoroutine("CR_ScrollSpellBar", false);
        }
    }

    private UISkillButton GetSpellButton()
    {
        UISkillButton spellButton = null;

        foreach(UISkillButton cachedObj in spellButtons)
        {

            // Pick an inactive object from the pool and leave the loop
            if(cachedObj.gameObject.activeSelf == false)
            {
                //Debug.Log("Found cached popup obj");
                spellButton = cachedObj;

                break;
            }
        }

        // No inactive popup was found in the pool, so let's create a new one
        if(spellButton == null)
        {
            GameObject spellBtn = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Skill Button"), spellParentPanel);
            spellButton = spellBtn.GetComponent<UISkillButton>();

            Debug.Assert(spellBtn != null, "spell button null");

            spellButtons.Add(spellButton);
        }

        //if(popup.InRange)
        spellButton.gameObject.SetActive(true);

        return spellButton;
    }

    private IEnumerator CR_ScrollSpellBar(bool scrollIn)
    {

        //float startScrollValue = mainPanel.position.y;
        
        //float elapsedTime = 0;

        if(vertical)
        {
            float targetScrollValue = scrollIn ? 90 : 40;
            while(_transform.anchoredPosition.y != targetScrollValue)
            {

                //elapsedTime += Time.deltaTime;
                Vector2 pos = _transform.anchoredPosition;
                pos.y = Mathf.MoveTowards(pos.y, targetScrollValue, Time.unscaledDeltaTime * 400);
                _transform.anchoredPosition = pos;
                yield return null;
            } 
        }
        else
        {
            float targetScrollValue = scrollIn ? -106.5f : 0f;
            while(_transform.anchoredPosition.x != targetScrollValue)
            {

                //elapsedTime += Time.deltaTime;
                Vector2 pos = _transform.anchoredPosition;
                pos.x = Mathf.MoveTowards(pos.x, targetScrollValue, Time.unscaledDeltaTime * 400);
                _transform.anchoredPosition = pos;
                yield return null;
            }
        }
    }

}
