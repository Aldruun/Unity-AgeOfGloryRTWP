using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UISkillbar : MonoBehaviour
{
    public bool debug;
    private Transform skillParentPanel;
    private List<UISkillButton> spellButtons;
    private UISkillButton _lastSelectedButton;

    public bool vertical;
    public float mainPanelScrollTime = 1;

    private RectTransform _transform;
    // Start is called before the first frame update
    private void Start()
    {
        //GameEventSystem.OnPartyMemberDeselected -= AdjustSkillbarVisibilityStatus;
        //GameEventSystem.OnPartyMemberDeselected += AdjustSkillbarVisibilityStatus;
        GameEventSystem.OnPartyMemberSelected -= PopulateSkillbar;
        GameEventSystem.OnPartyMemberSelected += PopulateSkillbar;
        //GameEventSystem.OnSpellAimingDone += DeselectActiveSkillButton;
        skillParentPanel = transform.Find("Content - Skill Buttons");
        _transform = GetComponent<RectTransform>();
        spellButtons = new List<UISkillButton>();

        for(int i = 0; i < 5; i++)
        {
            GameObject obj = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Skill Button"), transform.Find("Content - Skill Buttons"));
            obj.SetActive(false);
            spellButtons.Add(obj.GetComponent<UISkillButton>());
        }
        //StartCoroutine("CR_ScrollSpellBar", true);
    }

    private void DeselectActiveSkillButton()
    {
        if(_lastSelectedButton != null)
        {
            _lastSelectedButton.ToggleSelected(false);
            _lastSelectedButton = null;
        }
    }

    private void OnDisable()
    {
        //GameEventSystem.OnPartyMemberDeselected -= AdjustSkillbarVisibilityStatus;
        GameEventSystem.OnPartyMemberSelected -= PopulateSkillbar;
    }

    private void PopulateSkillbar(ActorInput caster)
    {
        _lastSelectedButton = null;
        if(debug)
            Debug.Log("<color=cyan>Populating spell bar</color>");
        foreach(UISkillButton item in spellButtons)
        {
            item.ToggleSelected(false);
            item.gameObject.SetActive(false);
        }
        //StopCoroutine("CR_ScrollSpellBar");
        //if(SelectionManager.NumSelectedPCs() > 1)
        //{
            
        //    StartCoroutine("CR_ScrollSpellBar", false);
        //    return;
        //}

        //StartCoroutine("CR_ScrollSpellBar", true);
        

        List<Skill> playerSkills = caster.GetSkills(true);
        int skillCount = playerSkills.Count;
        //int skillButtonIndex = 0;
        for(int i = 0; i < skillCount; i++)// Skill skill in agent.behaviours.skillController.GetSkills())
        {
            Skill skill = playerSkills[i];
            if(debug)
                Debug.Log(caster.GetName() + ": <color=cyan>Adding button for skill '" + skill.skillName + "'</color>");
            //if(skill is Skill_DrinkHealthPotion)
            //{
            //    Button btn_drinkHealth = transform.Find("Button - Consume Health Potion").GetComponent<Button>();
            //    if(agent.isBeast)
            //    {
            //        btn_drinkHealth.gameObject.SetActive(false);
            //        continue;
            //    }
            //    Text numInfo = btn_drinkHealth.transform.GetChild(1).GetComponent<Text>();
            //    numInfo.text = agent.GetComponent<AgentGearController>().healthPotions.ToString();
            //    btn_drinkHealth.onClick.AddListener(delegate {
            //        if(skill.CanActivate(agent))
            //        {
            //            GameEventSystem.OnPlayerSkillButtonClicked?.Invoke(skill);
            //            numInfo.text = agent.GetComponent<AgentGearController>().healthPotions.ToString();
            //        }
            //    });
            //    Image radI1 = btn_drinkHealth.transform.GetChild(0).GetComponent<Image>();
            //    skill.CooldownHook += (remainingCooldown) =>
            //    {
            //        radI1.fillAmount = remainingCooldown / skill.cooldown;
            //    };
            //    continue;
            //}
            //if(skill is Skill_DrinkManaPotion && agent.m_isSpellCaster)
            //{
            //    Button btn_drinkMana = transform.Find("Button - Consume Mana Potion").GetComponent<Button>();
            //    if(agent.m_isSpellCaster == false || agent.isBeast)
            //    {
            //        btn_drinkMana.gameObject.SetActive(false);
            //        continue;
            //    }
            //    Text numInfo = btn_drinkMana.transform.GetChild(1).GetComponent<Text>();
            //    numInfo.text = agent.GetComponent<AgentGearController>().manaPotions.ToString();
            //    btn_drinkMana.onClick.AddListener(delegate {
            //        if(skill.CanActivate(agent))
            //        {
            //            GameEventSystem.OnPlayerSkillButtonClicked?.Invoke(skill);
            //            numInfo.text = agent.GetComponent<AgentGearController>().manaPotions.ToString();
            //        }
            //    });
            //    Image radI1 = btn_drinkMana.transform.GetChild(0).GetComponent<Image>();
            //    skill.CooldownHook += (remainingCooldown) =>
            //    {
            //        radI1.fillAmount = remainingCooldown / skill.cooldown;
            //    };
            //    continue;
            //}

            UISkillButton sBtn = GetSkillButton();
            sBtn.skill = skill;
            Button skillButton = sBtn.GetComponent<Button>();

            if(skill.skillIcon == null)
            {
                if(debug)
                    Debug.LogError(caster.GetName() + ": spell.spellIcon = null");
            }

            skillButton.GetComponent<Image>().sprite = skill.skillIcon;
            //GameEventSystem.OnPlayerSkillKeyPressed += (inputKey) =>
            //{
                
            //};

            //skillButton.image.sprite = skill.spellIcon;
            //skillButton.onClick = new Button.ButtonClickedEvent();
            skillButton.onClick.RemoveAllListeners();
            skillButton.onClick.AddListener(delegate
            {
                if(debug)
                    Debug.Log(caster.GetName() + ":<color=orange>Key " + skill.skillName + " pressed</color>");             
                GameEventSystem.OnPlayerSkillButtonClicked?.Invoke(caster, skill, sBtn);

                foreach(UISkillButton skillBtn in spellButtons)
                {
                    skillBtn.ToggleSelected(false);
                }
                sBtn.ToggleSelected(true);
                _lastSelectedButton = sBtn;
            });
            Image radImage = sBtn.transform.Find("cooldown").GetComponent<Image>();
            skill.CooldownHook += (remainingCooldown) =>
            {
                radImage.fillAmount = remainingCooldown / skill.cooldown;
            };

            //skillButtonIndex++;
        }
    }

    //void AdjustSkillbarVisibilityStatus(ActorInput agent)
    //{
    //    //int numSelectedPCs = SelectionManager.NumSelectedPCs();

    //    foreach(UISpellButton item in spellButtons)
    //    {
    //        item.transform.Find("glow frame").GetComponent<Image>().enabled = false;
    //        item.gameObject.SetActive(false);
    //    }

    //    StopCoroutine("CR_ScrollSpellBar");

    //    if(SelectionManager.selected != null)
    //    {
    //        StartCoroutine("CR_ScrollSpellBar", true);
    //    }
    //    else
    //    {
    //        StartCoroutine("CR_ScrollSpellBar", false);
    //    }
    //}

    private UISkillButton GetSkillButton()
    {
        UISkillButton skillButton = null;

        foreach(UISkillButton cachedObj in spellButtons)
        {

            // Pick an inactive object from the pool and leave the loop
            if(cachedObj.gameObject.activeSelf == false)
            {
                //Debug.Log("Found cached popup obj");
                skillButton = cachedObj;

                break;
            }
        }

        // No inactive popup was found in the pool, so let's create a new one
        if(skillButton == null)
        {
            GameObject skillBtn = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Skill Button"), skillParentPanel);
            skillButton = skillBtn.GetComponent<UISkillButton>();

            Debug.Assert(skillBtn != null, "Skill button null");

            spellButtons.Add(skillButton);
        }

        //if(popup.InRange)
        skillButton.gameObject.SetActive(true);

        return skillButton;
    }

    private IEnumerator CR_ScrollSpellBar(bool scrollIn)
    {

        //float startScrollValue = mainPanel.position.y;
        
        //float elapsedTime = 0;

        if(vertical)
        {
            float targetScrollValue = scrollIn ? 106.5f : 40;
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
