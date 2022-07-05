using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class ActorInfoPanel : MonoBehaviour
{

    public static ActorInfoPanel current;

    public RectTransform mainPanel; // The parent for all info panels (agent, building, etc.)

    public float mainPanelScrollTime = 1;

    private Actor _observedAgent;
    private bool _isPlanningAI;
    public Slider statusBar_Exp;
    public Slider statusBar_Health;
    public Slider statusBar_Mana;
    public Text text_AgentName;
    public Text text_CurrentLevel;
    public Text text_CurrentState;
    public Text text_Health;
    public Text text_Mana;
    public Text text_pAttack;
    public Text text_mAttack;
    public Text text_pDefence;
    public Text text_mDefence;
    public Text text_Exp;
    //public Color FriendColor;
    //public Color NeutralColor;
    //public Color EnemyColor;
    //public Color femaleColor;
    //public Color maleColor;

    private GameObject graphics;


    private void Awake()
    {
        if(current != null && current != this)
        {
            Debug.LogError("Only one instance allowed");
            Destroy(this);
        }
        else
            current = this;

        graphics = transform/*.GetChild(0)*/.gameObject;
        //graphics.SetActive(false);


        //SelectionManager.OnAgentSelected += ShowInfo;
        //SelectionManager.OnAgentDeselected += Hide;
    }

    private void OnDestroy()
    {

        //SelectionManager.OnAgentSelected -= ShowInfo;
        //SelectionManager.OnAgentDeselected -= Hide;
    }

    private void ShowInfo(Actor agent)
    {


        Actor pAI = agent;

        if(current._observedAgent != agent)
        {

            if(current._observedAgent != null)
            {

                //current._observedAgent.Combat.OnLevelProgressIncreased -= current.UpdateUI_ExpBar;
                //current._observedAgent.Combat.OnLevelUp -= current.UpdateUI_Level;
                //current._observedAgent.Combat.OnStatsChanged -= current.UpdateUI_Stats;
                //current._observedAgent.Combat.OnHealthChanged -= current.UpdateUI_HealthBar;
                //current._observedAgent.Combat.OnManaChanged -= current.UpdateUI_ManaBar;

                //if(current._observedAgent is NPCController)
                //    ((NPCController)current._observedAgent).internalAiBehaviours.aiPackageSelector.OnActionChanged -= current.UpdateUI_StateText;
            }

            //current.UpdateUI_ExpBar(agent);
            //current.UpdateUI_Level(agent.Combat.level);
            //current.UpdateUI_Stats(agent.m_pAttack, agent.m_mAttack, agent.m_pDefence, agent.m_mDefence);
            //current.UpdateUI_HealthBar(agent, 0);
            //current.UpdateUI_ManaBar(agent, 0);

            //if(pAI != null)
            //{
            //    current.UpdateUI_StateText(pAI.internalAiBehaviours.aiPackageSelector.currentAction); // TODO: Initial updating of task action quote
            //    pAI.internalAiBehaviours.aiPackageSelector.OnActionChanged -= current.UpdateUI_StateText;
            //    pAI.internalAiBehaviours.aiPackageSelector.OnActionChanged += current.UpdateUI_StateText;
            //}

            //agent.OnLevelProgressIncreased -= current.UpdateUI_ExpBar;
            //agent.OnLevelProgressIncreased += current.UpdateUI_ExpBar;
            //agent.OnLevelUp -= current.UpdateUI_Level;
            //agent.OnLevelUp += current.UpdateUI_Level;
            //agent.OnStatsChanged -= current.UpdateUI_Stats;
            //agent.OnStatsChanged += current.UpdateUI_Stats;
            //agent.OnHealthChanged -= current.UpdateUI_HealthBar;
            //agent.OnHealthChanged += current.UpdateUI_HealthBar;
            //agent.OnManaChanged -= current.UpdateUI_ManaBar;
            //agent.OnManaChanged += current.UpdateUI_ManaBar;


            current._observedAgent = agent;
        }

        current._observedAgent = agent;

        //current.text_AgentName.text = agent.Name != "" ? agent.Name : agent.GetName();

        //current.graphics.SetActive(true);

        StartCoroutine(CR_ScrollPanel(true));
    }

    public static void SetCurrentActionInfo(string text)
    {
        current.text_CurrentState.text = text;
    }

    private void Hide(Actor agent)
    {
        if(current._observedAgent == null)
        {


            return;
        }

        //current._observedAgent.OnLevelProgressIncreased -= current.UpdateUI_ExpBar;
        //current._observedAgent.OnLevelUp -= current.UpdateUI_Level;
        //current._observedAgent.OnStatsChanged -= current.UpdateUI_Stats;
        //current._observedAgent.OnHealthChanged -= current.UpdateUI_HealthBar;
        //current._observedAgent.OnManaChanged -= current.UpdateUI_ManaBar;
        //current._observedAgent.agent.taskController.OnTaskChanged -= current.UpdateUI_StateText;
        current._observedAgent = null;

        //current.StopAllCoroutines();
        //current.graphics.SetActive(false);
        if(agent == null)
        {

            StartCoroutine(CR_ScrollPanel(false));

        }
    }

    private IEnumerator CR_ScrollPanel(bool scrollIn)
    {

        //float startScrollValue = mainPanel.position.y;
        float targetScrollValue = scrollIn ? 0 : 90;
        float elapsedTime = 0;

        while(elapsedTime < mainPanelScrollTime/*mainPanel.anchoredPosition.y < targetScrollValue*/)
        {

            elapsedTime += Time.deltaTime;
            Vector2 pos = mainPanel.anchoredPosition;
            pos.y = Mathf.Lerp(pos.y, targetScrollValue, elapsedTime / mainPanelScrollTime);
            mainPanel.anchoredPosition = pos;
            yield return null;
        }
    }

    private void UpdateUI_ExpBar(ActorStats agent)
    {

        int currExp = (int)agent.currentExp;
        int neededExp = agent.expNeeded;

        statusBar_Exp.maxValue = neededExp;
        statusBar_Exp.value = currExp;
        text_CurrentLevel.text = "Level " + agent.Level;
        text_Exp.text = currExp + "/" + neededExp;
    }

    private void UpdateUI_Level(int level)
    {

        text_CurrentLevel.text = level.ToString();
    }

    private void UpdateUI_HealthBar(Actor agent, float amount)
    {

        if(agent.dead)
        {

            Hide(null);
            return;
        }
        //statusBar_Health.maxValue = agent.m_maxHealth;
        //statusBar_Health.value = agent.m_currentHealth;
        //text_Health.text = Mathf.CeilToInt(agent.m_currentHealth).ToString() + "/" + agent.m_maxHealth.ToString();
    }

    private void UpdateUI_ManaBar(ActorStats agent, float amount)
    {

        //statusBar_Mana.maxValue = agent.m_maxMana;
        //statusBar_Mana.value = agent.m_currentMana;
        //text_Mana.text = ((int)agent.m_currentMana).ToString() + "/" + agent.m_maxMana.ToString();
    }

    private void UpdateUI_Stats(int ar, int strength, int endurance, int agility, int speed, int intelligence, int willpower, int personality, int luck)
    {

        //text_pAttack.text = pAttack.ToString();
        //text_mAttack.text = mAttack.ToString();
        //text_pDefence.text = pDef.ToString();
        //text_mDefence.text = mDef.ToString();
    }
    //void UpdateUI_StateText(NPCAction a) {

    //    text_CurrentState.text = a == null ? "N/A" : a.actionQuote;
    //}
}
