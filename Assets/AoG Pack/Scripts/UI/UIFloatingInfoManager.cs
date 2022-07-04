using AoG.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace AoG.UI
{
    public class UIFloatingInfoManager
    {
        private Dictionary<ActorInput, UIFloatingInfo> uiInfoMap;

        private Camera camera;

        private bool _showAllUIInfo;
        private Transform infoContainer;
        public float infoFadeOutDistance = 20;
        public float maximumDistance = 10.0f;
        public float maximumDistanceScale = 0.1f;

        public float minimumDistance = 1.0f;
        public float minimumDistanceScale = 1.0f;

        public GameObject prefab_floatingAgentInfo;

        private bool _showingInfo;

        public UIFloatingInfoManager(Transform infoHolder, Camera camera)
        {
            infoContainer = infoHolder;
            prefab_floatingAgentInfo = Resources.Load<GameObject>("Prefabs/UI/Floating Actor Info");
            this.camera = camera;

            uiInfoMap = new Dictionary<ActorInput, UIFloatingInfo>();
        }
       
        public void Clear()
        {
            if(uiInfoMap == null)
            {
                return;
            }

            foreach(KeyValuePair<ActorInput, UIFloatingInfo> uiInfoPair in uiInfoMap.ToArray())
            {
                GameObject floatingObj = uiInfoPair.Value.parentPanel.gameObject;
                Object.Destroy(floatingObj);
            }
            uiInfoMap = new Dictionary<ActorInput, UIFloatingInfo>();
        }

        private void PrepareActorInfoUI(ActorInput actor)
        {
            if(actor.debugAnimation)
            {
                UnityEngine.Debug.Log("<color=grey>" + actor.transform.gameObject.name + ": Init agent UI</color>");
            }

            //HACK ########################################################################################################
            //HACK --------------------------------- Group Member Info Card Setup -----------------------------------------
            //HACK ########################################################################################################

            //if(actor.faction == Faction.Heroes)
            //{
            //Transform fogOfWarStencil = monoAgent.transform.Find("FOW Stencil");
            //if(fogOfWarStencil == null)
            //{
            //Transform fogOfWarStencil = Instantiate(Resources.Load<Transform>("Prefabs/Game Logic/FOW Stencil"), actor.transform).ResetPosRot();
            //////TODO Spawn FOW Stencil
            //FoW.FogOfWarUnit fowUnit = fogOfWarStencil.GetComponent<FoW.FogOfWarUnit>();
            //fowUnit.circleRadius = monoAgent.actorData.aggroRange;

            //CreateHeroInfoCard(monoAgent);

            //Transform speechBubble = Instantiate(Resources.Load<Transform>("Prefabs/Speech Bubble"), actor.transform).ResetPosRot();
            //}

            //HACK ########################################################################################################
            //HACK ------------------------------------ Floating UI Setup -------------------------------------------------
            //HACK ########################################################################################################

            Transform infoUIObj = Object.Instantiate(prefab_floatingAgentInfo, infoContainer).transform;

            infoUIObj.SetParent(infoContainer);
            infoUIObj.gameObject.SetActive(true);
            uiInfoMap.Add(actor, new UIFloatingInfo(actor, infoUIObj));

            actor.Combat.RegisterCallback_OnDeath(RemoveUI);


        }

        public void Update()
        {
            //if(GameEventSystem.showFloatingInfo == false)
            //{
            //    if(_showingInfo)
            //    {
            //        foreach(KeyValuePair<Actor, UIFloatingInfo> uiInfoPair in uiInfoMap.ToArray())
            //        {
            //            GameObject floatingObj = uiInfoPair.Value.parentPanel.gameObject;
            //            if(floatingObj.activeInHierarchy)
            //            {
            //                floatingObj.SetActive(false);
            //            }
            //        }
            //        _showingInfo = false;
            //    }
            //    return;
            //}

            _showingInfo = true;

            //? Select           : Show bars and selector permanently
            //? Mouse over       : Highlight and show bars
            //? Mouse exit       : Unhighlight and hide bars
            //? Press info button: Show bars on all selectables

            //if(Input.GetKey(KeyCode.Q))
            //{
            //    _showAllUIInfo = true;
            //    foreach(KeyValuePair<ActorMonoController, UIFloatingInfo> uiInfoPair in uiInfoMap)
            //    {
            //        //MeshRenderer mr = uiInfoPair.Key.GetComponentInChildren<MeshRenderer>();
            //        GameObject floatingObj = uiInfoPair.Value.parentPanel.gameObject;
            //        //if(mr != null && mr.IsVisibleFrom(Camera.main) == false)
            //        //{
            //        //    floatingObj.SetActive(false);
            //        //    continue;
            //        //}


            //        UIFollowObject(floatingObj, uiInfoPair.Key.transform);
            //    }

            //    return;
            //}

            if(Input.GetKeyUp(KeyCode.Q))
            {
                _showAllUIInfo = false;
                foreach(KeyValuePair<ActorInput, UIFloatingInfo> kvp in uiInfoMap)
                    kvp.Value.parentPanel.gameObject.SetActive(false);
            }

            foreach(KeyValuePair<ActorInput, UIFloatingInfo> uiInfoPair in uiInfoMap.ToArray())
            {
                GameObject floatingObj = uiInfoPair.Value.parentPanel.gameObject;
                ActorInput character = uiInfoPair.Key;
                if(uiInfoPair.Value.enabled)
                {
                    if(character == null)
                    {
                        Object.Destroy(floatingObj);
                        uiInfoMap.Remove(character);
                        continue;
                    }
                    floatingObj.SetActive(true);
                    //UIFollowObject(uiInfoPair.Value.text_NamePlate.gameObject, uiInfoPair.Value.namePlateAnchor);
                }
                UIFollowObject(floatingObj, character.transform);
            }
        }

        private void RemoveUI(ActorInput actor)
        {
            if(uiInfoMap.ContainsKey(actor))
            {
                Object.Destroy(uiInfoMap[actor].parentPanel.gameObject);
                uiInfoMap.Remove(actor);
            }
        }

        private void UIFollowObject(GameObject uiObject, Transform worldObject)
        {
            uiObject.transform.position = camera.WorldToScreenPoint(camera.transform.up * 0.5f + worldObject.position + Vector3.up * 1.7f);
            //MeshRenderer mr = worldObject.GetComponentInChildren<MeshRenderer>();
            if(/*mr != null && mr.IsVisibleFrom(_camera)*/ UIOnScreen(uiObject.transform.position) && Vector3.Distance(camera.transform.position, worldObject.position) < infoFadeOutDistance)
            {
                //UnityEngine.Debug.Log("On Screen");
                //uiObject.SetActive(true);
                uiObject.SetActive(true);
                float norm = HelperFunctions.GetLinearDistanceAttenuation(worldObject.position, camera.transform.position, minimumDistance, maximumDistance);
                Vector3 minScale = Vector3.one * maximumDistanceScale;
                Vector3 maxScale = Vector3.one * minimumDistanceScale;

                uiObject.transform.localScale = Vector3.Lerp(maxScale, minScale, norm);
            }
            else
            {
                //UnityEngine.Debug.Log("Off Screen");

                uiObject.SetActive(false);
            }
        }

        private bool UIOnScreen(Vector2 uiScreenPosition)
        {
            Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);

            return screenRect.Contains(uiScreenPosition);
        }

        //void OnGUI()
        //{
        //    foreach(AgentMonoController agent in uiInfoMap.Keys)
        //    {
        //        //MeshRenderer mr = uiInfoPair.Key.GetComponentInChildren<MeshRenderer>();
        //        //GameObject floatingObj = uiInfoPair.Value.parentPanel.gameObject;
        //        //if(mr != null && mr.IsVisibleFrom(Camera.main) == false)
        //        //{
        //        //    floatingObj.SetActive(false);
        //        //    continue;
        //        //}
        //        Vector3 screenPos = Camera.main.WorldToScreenPoint(agent.transform.position + Vector3.up * 1.9f);
        //        GUI.Label(new Rect(new Vector2(screenPos.x, Screen.height - screenPos.y), new Vector2(130, 30)), agent.actorData.Name);

        //        //UIFollowObject(floatingObj, uiInfoPair.Key.transform);
        //    }
        //}


    }

    public class UIFloatingInfo
    {
        //Text text_Mana;
        public ActorInput agent;
        public Transform parentPanel;

        private readonly Slider hpBar;

        private readonly Slider drainBar;
        //Text text_Health;
        private readonly Slider statusBar_nextAction;
        private readonly Slider statusBar_nextCast;

        private readonly Transform _hitInfoContainer;
        private readonly Text[] _hitInfoTextObjects;
        private readonly Image image_aiState;
        private readonly Image[] images_aiStates;


        public UIFloatingInfo(ActorInput agent, Transform parentPanel)
        {
            this.parentPanel = parentPanel;
            this.agent = agent;

            hpBar = parentPanel.Find("Bar Holder/HP Bar").GetComponent<Slider>();
            drainBar = hpBar.transform.Find("Drain Bar").GetComponent<Slider>();
            hpBar.gameObject.SetActive(true);
            statusBar_nextAction = parentPanel.Find("Bar Holder/Next Action Bar").GetComponent<Slider>();
            statusBar_nextCast = parentPanel.Find("Bar Holder/Next Cast Bar").GetComponent<Slider>();

            _hitInfoContainer = parentPanel.Find("Hit Info Queue");
            _hitInfoTextObjects = _hitInfoContainer.GetComponentsInChildren<Text>(true);


            image_aiState = parentPanel.Find("image_aiState").GetComponent<Image>();
            images_aiStates = image_aiState.transform.Find("aiStates").GetComponentsInChildren<Image>(true); //! Image child order needs to be same as AIState enum members
                                                                                                             //agent.OnAIStateChanged -= UpdateUI_AIStateImage;
                                                                                                             //agent.OnAIStateChanged += UpdateUI_AIStateImage;
                                                                                                             //UpdateUI_AIStateImage(AIState.IDLE);

            //parentPanel.Find("Name Plate").GetComponent<Text>().text = agent.GetName();

            //agent.OnCombatStateChanged -= ShowCombatUI;
            //agent.OnCombatStateChanged += ShowCombatUI;

            //if(this.agent.ActorRecord.faction != Faction.Heroes)
            //    hpBar.GetComponentInChildren<Image>().color = new Color(0.9f, 0.1f, 0.1f, 1.0f);

            //text_Health = statusBar_Health.GetComponent<Text>();
            //text_Mana = statusBar_Mana.GetComponent<Text>();
            //statusBar_nextAction.gameObject.SetActive(true);
            //this.agent.TimeToNextActionHook = UpdateUI_ActionBar;
            //statusBar_nextCast.gameObject.SetActive(true);
            //this.agent.TimeToNextCastHook = UpdateUI_CastPauseBar;
            //this.agent.OnNoActionsLeft = () => { statusBar_nextAction.transform.Find("Fill Area/Fill").GetComponent<Image>().color = Color.red; };

            this.agent.Combat.UnregisterCallback_OnHealthChanged(UpdateUI_HealthBar);
            this.agent.Combat.RegisterCallback_OnHealthChanged(UpdateUI_HealthBar);
            this.agent.Combat.OnHit -= UpdateUI_HitInfoText;
            this.agent.Combat.OnHit += UpdateUI_HitInfoText;
        }

        public bool enabled { get; set; }

        private void UpdateUI_HealthBar(float newCurrent, float currentMax)
        {
            hpBar.maxValue /*= drainBar.maxValue*/ = currentMax;
            hpBar.value = newCurrent;
            //text_Health.text = Mathf.CeilToInt(agent.m_currentHealth).ToString() + "/" + agent.m_maxHealth.ToString();
            if(drainBar.value > hpBar.value)
            {
                CoroutineRunner.Instance.StartCoroutine(CR_HPDrain(hpBar.value));
            }
            else
            {
                drainBar.value = hpBar.value;
            }
        }

        private IEnumerator CR_HPDrain(float targetValue)
        {
            float currDiff = drainBar.value - targetValue;
            while(drainBar.value > targetValue)
            {
                drainBar.value -= Time.unscaledDeltaTime * 3 * currDiff;

                yield return null;
            }
        }

        private void UpdateUI_HitInfoText(ActorInput source, float damage, DamageType effectType, bool hitSuccess)
        {
            if(agent.dead)
            {
                return;
            }
            if(hitSuccess == false)
            {
                agent.StartCoroutine(CR_ShowHitInfo("Missed"));
            }
            else
            {
                agent.StartCoroutine(CR_ShowHitInfoInt(damage));
            }
        }

        //void ShowCombatUI(bool on)
        //{
        //    if(on)
        //    {
        //        statusBar_nextAction.gameObject.SetActive(true);
        //        this.agent.TimeToNextActionHook = UpdateUI_ActionBar;
        //        UpdateUI_ActionBar(0);
        //    }
        //    else
        //        statusBar_nextAction.gameObject.SetActive(false);
        //}

        //void UpdateUI_ActionBar(float amount)
        //{
        //    //if(agent.flurry == 1)
        //    //    statusBar_nextAction.fillRect.GetComponent<Image>().color = Color.green;
        //    //else if(agent.flurry == 2)
        //    //    statusBar_nextAction.fillRect.GetComponent<Image>().color = Colors.LightCyan;
        //    //else if(agent.flurry == 3)
        //    //    statusBar_nextAction.fillRect.GetComponent<Image>().color = Colors.OrangeCrayola;

        //    statusBar_nextAction.maxValue = 6f;
        //    statusBar_nextAction.value = agent.timeToNextRound;
        //    //text_Mana.text = agent.m_currentMana.ToString() + "/" + agent.m_maxMana.ToString();
        //}

        //void UpdateUI_CastPauseBar(float amount)
        //{
        //    statusBar_nextCast.maxValue = 6f;
        //    statusBar_nextCast.value = 6 - agent.GetSpellPauseTimer();
        //    //text_Mana.text = agent.m_currentMana.ToString() + "/" + agent.m_maxMana.ToString();
        //}

        //void UpdateUI_AIStateImage(AIState state)
        //{
        //    for(int i = 0; i < images_aiStates.Length; i++)
        //    {
        //        if(i == (int)state)
        //        {
        //            images_aiStates[i].enabled = true;
        //            continue;
        //        }

        //        images_aiStates[i].enabled = false;
        //    }

        //    //switch(state)
        //    //{
        //    //    case AIState.IDLE:
        //    //        break;
        //    //    case AIState.COMBAT:
        //    //        break;
        //    //}
        //}

        private Text GetAvailableHitInfoText()
        {
            foreach(Text text in _hitInfoTextObjects)
            {
                if(text == null)
                {
                    continue;
                }
                if(text.gameObject.activeInHierarchy == false)
                {
                    return text;
                }
            }

            return null;
        }

        private IEnumerator CR_ShowHitInfo(string content)
        {
            Text text = GetAvailableHitInfoText();
            text.text = content;
            Color c = Color.white;
            c.a = 0;
            text.color = c;
            text.rectTransform.SetAsLastSibling();
            text.gameObject.SetActive(true);

            while(text.color.a < 1)
            {
                if(text == null)
                    yield break;

                c.a += Time.deltaTime * 4;
                text.color = c;

                yield return null;
            }

            yield return new WaitForSeconds(1f);


            while(text.color.a > 0)
            {
                if(text == null)
                    yield break;

                c.a -= Time.deltaTime * 4;
                text.color = c;

                yield return null;
            }

            text.gameObject.SetActive(false);

        }

        private IEnumerator CR_ShowHitInfoInt(float content)
        {
            Text text = GetAvailableHitInfoText();
            if(text == null)
                yield break;
            text.text = content.ToString();
            Color c = /*content > 0 ? Colors.GreenLizard :*/ text.color;
            c.a = 0;
            text.color = c;
            text.rectTransform.SetAsLastSibling();
            text.gameObject.SetActive(true);

            while(text.color.a < 1)
            {
                if(text == null)
                    yield break;

                c.a += Time.deltaTime * 4;
                text.color = c;

                yield return null;
            }

            yield return new WaitForSeconds(1f);


            while(text.color.a > 0)
            {
                if(text == null)
                    yield break;

                c.a -= Time.deltaTime * 4;
                text.color = c;

                yield return null;
            }
            if(text == null)
                yield break;
            text.gameObject.SetActive(false);

        }
        public void Activate()
        {
            enabled = true;
            parentPanel.gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            enabled = false;
            parentPanel.gameObject.SetActive(false);
        }

        #region Utilities #########################################################################################

        //public void AdjustSizeToText(Text text)
        //{
        //    //_squadMemberName = text;
        //    var rt = text.GetComponent<RectTransform>();

        //    //rt.sizeDelta = new Vector2(GetSizeOfWord(text, text.text), rt.sizeDelta.y);
        //    rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, GetSizeOfWord(text, text.text) + 10);
        //}

        //public float GetSizeOfWord(Text text, string word)
        //{
        //    //var textGen = new TextGenerator();

        //    return text.GetRenderedValues(false).x;

        //    //var generationSettings = text.GetRenderedValues(false)..(text.rectTransform.rect.size);
        //    /*return textGen.GetPreferredWidth(word, generationSettings)*/
        //    ;
        //    //float height = textGen.GetPreferredHeight(newText, generationSettings);
        //}

        #endregion Utilities

        public void ClearSubscribers()
        {

        }
    }

    public class SkillInfoUI
    {
        private Spell _skill;
        private Image _cooldownImage;

        public SkillInfoUI(Spell skill, Image cooldownImage)
        {
            _cooldownImage = cooldownImage;
            _skill = skill;
            _skill.CooldownHook += UpdateSkillCooldownUI;
        }

        private void UpdateSkillCooldownUI(float remainingCooldown)
        {
            _cooldownImage.fillAmount = remainingCooldown / _skill.cooldownTime;
        }

        public void ClearSubscribers()
        {
            _skill.CooldownHook -= UpdateSkillCooldownUI;
        }
    } 
}