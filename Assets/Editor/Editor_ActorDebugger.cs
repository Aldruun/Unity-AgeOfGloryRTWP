using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Editor_ActorDebugger : EditorWindow
{
    //void OnGUI() {
    //    GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), tex, ScaleMode.StretchToFill);
    //    GUI.Label(new Rect(200, 200, 100, 100), "A label");
    //    GUI.TextField(new Rect(20, 20, 70, 30), "");
    //}

    private GUIStyle headerBackgroundStyle;
    private static GUISkin cSkin;
    private static ActorDebugger debugger;
    private static readonly Actor _actor;

    //static AI_CombatProtocol aiCombat;
    //static Inventory aiInventory;
    private readonly int labelFontSize = 9;
    private static string[] tabs;
    private static int selectedTabIndex;

    private void OnEnable()
    {
        headerBackgroundStyle = BackgroundStyle.Get(Colors.WarmBlack);

        tabs = new string[] { "Overview", "Stats", "Status Effects", "Global" };

        cSkin = Resources.Load<GUISkin>("GUI Skins/Editor GUISkin");

        selectedTabIndex = EditorPrefs.GetInt("tabIndex", selectedTabIndex);
        EditorPrefs.GetInt("labelFontSize", labelFontSize);

        //SelectionManager.OnAgentSelected += (agent) => { debugger.SetObservedPlanner(agent is PlanningProtocol ? (PlanningProtocol)agent : null); };
    }

    private void OnDisable()
    {

        EditorPrefs.SetInt("tabIndex", selectedTabIndex);
        EditorPrefs.SetInt("labelFontSize", labelFontSize);
    }

    private void OnGUI()
    {

        if(Application.isPlaying)
        {
            if(debugger != null && debugger.observedActor == null)
            {
                debugger.SetObservedPlanner(AoG.Core.GameInterface.Instance.GetCurrentGame().GetPC(1));
            }
        }

        //GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), tex, ScaleMode.StretchToFill);

        //buttonFontSize = EditorGUILayout.IntSlider(buttonFontSize, 1, 20);
        //GUI.skin.button.fontSize = buttonFontSize;
        if(debugger == null)
        {
            GUILayout.Label("Initializing...");
            debugger = FindObjectOfType<ActorDebugger>();
            return;
        }

        if(debugger.observedActor == null)
        {
            //if(Application.isPlaying)
            //{
            //    GUILayout.FlexibleSpace();
            //    DrawSelectPlannerDropDown();
            //}
            return;
        }
        Actor observee = debugger.observedActor;

        if(debugger.observedActor.ActionQueue == null)
        {
            GUI.color = Color.yellow;
            GUILayout.Label("Actor's action queue is null");
            GUI.color = Color.white;
        }

        if(observee != null && observee.gameObject.activeSelf)
        {
            selectedTabIndex = GUILayout.Toolbar(selectedTabIndex, tabs/*cSkin.GetStyle("Goap Runtime Editor Tab")*/);

            using(new GUILayout.HorizontalScope(headerBackgroundStyle, GUILayout.ExpandWidth(true)))
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label(observee.GetName() + ", " + observee.ActorStats.Class.ToString() + "(" + observee.ActorStats.Level + ")");
                GUILayout.FlexibleSpace();
            }
            
            switch(selectedTabIndex)
            {
                case 0:
                    //GUILayout.BeginHorizontal();
                    using(new GUILayout.VerticalScope())
                    {
                        EditorPrefs.SetInt("tabIndex", 0);

                        GUI.skin = cSkin;

                        using(new GUILayout.VerticalScope())
                        {
                            GUI.color = Color.yellow;
                            GUILayout.Label("- Actor State -");
                            GUI.color = Color.white;
                            //GUI.skin = cSkin;
                            if(Application.isPlaying)
                            {
                                
                                string xpNeeded = observee.ActorStats.Level == 20 ? "MAX" : observee.ActorStats.expNeeded.ToString();
                                GUILayout.Label(string.Format("XP: {0}/{1}", observee.ActorStats.currentExp, xpNeeded));
                                GUILayout.Label(string.Format("HP: {0}/{1}", observee.ActorStats.GetBaseStat(ActorStat.HITPOINTS), observee.ActorStats.GetBaseStat(ActorStat.MAXHITPOINTS)));
                                GUILayout.Label(string.Format(
                                    "STR: {0} [{6}], " +
                                    "\nDEX: {1} [{7}], " +
                                    "\nCON: {2} [{8}], " +
                                    "\nINT: {3} [{9}], " +
                                    "\nWIS: {4} [{10}], " +
                                    "\nCHA: {5} [{11}]",
                                    observee.ActorStats.GetBaseStat(ActorStat.STRENGTH),
                                    observee.ActorStats.GetBaseStat(ActorStat.DEXTERITY),
                                    observee.ActorStats.GetBaseStat(ActorStat.CONSTITUTION),
                                    observee.ActorStats.GetBaseStat(ActorStat.INTELLIGENCE),
                                    observee.ActorStats.GetBaseStat(ActorStat.WISDOM),
                                    observee.ActorStats.GetBaseStat(ActorStat.CHARISMA),
                                    observee.ActorStats.strMod,
                                    observee.ActorStats.dexMod,
                                    observee.ActorStats.conMod,
                                    observee.ActorStats.intMod,
                                    observee.ActorStats.wisMod,
                                    observee.ActorStats.chaMod
                                    ));
                                GUILayout.Label(string.Format("Total APR: {0}", observee.RoundSystem.GetTotalAPR()));
                                GUILayout.Label(string.Format("AC: {0}", observee.ActorStats.GetBaseStat(ActorStat.AC)));
                            }
                        }

                        using(new GUILayout.VerticalScope())
                        {
                            if(Application.isPlaying)
                            {
                                if(observee.Combat.GetHostileTarget() == null)
                                {
                                    GUILayout.Label("Attack Target: null");
                                }
                                else
                                {
                                    GUILayout.Label("Attack Target: " + observee.Combat.GetHostileTarget().GetName());
                                }

                                GUILayout.Label("Enemy Flags: " + debugger.observedActor.ActorStats.GetEnemyFlags().ToString());
                                if(observee.Combat.GetEquippedWeapon().Data != null)
                                {
                                    GUILayout.Label("Weapon ID: " + observee.Combat.GetEquippedWeapon().Data.identifier);
                                }
                                GUILayout.Label("Global ID: " + observee.GetGlobalID());
                                GUILayout.Label("Selected: " + observee.ActorUI.Selected);
                                GUILayout.Label("Party Index: " + observee.PartySlot);
                                GUILayout.Label("Is Downed: " + observee.isDowned);
                            }
                        }

                        GUI.color = Color.yellow;
                        GUILayout.Label("- Actions -");
                        GUI.color = Color.white;
                        if(Application.isPlaying)
                        {
                            if(debugger.observedActor.CurrentAction != null)
                            {
                                string add = "";

                                if(debugger.observedActor.CurrentAction is AIActions.Action_CastSpellAtActor ca)
                                {
                                    add = ca.castState.ToString();
                                }

                                if(debugger.observedActor.CurrentAction is AIActions.Action_CastSpellAtLocation cl)
                                {
                                    add = cl.castState.ToString();
                                }

                                using(new GUILayout.VerticalScope())
                                {
                                    if(GUILayout.Button(debugger.observedActor.CurrentAction.ToString(), GUILayout.ExpandWidth(false)))
                                    {

                                    }
                                    if(add != "")
                                    {
                                        GUILayout.Label("[" + add + "]");
                                    }
                                }
                            }
                            else
                            {
                                GUILayout.Label("-");
                            }
                        
                            foreach(GameAction action in debugger.observedActor.ActionQueue)
                            {
                                GUILayout.Space(1);

                                GUILayout.BeginVertical();
                                GUILayout.BeginHorizontal();
                             
                                if(GUILayout.Button(action.ToString(), GUILayout.ExpandWidth(false))) { }

                                //if(debugger.observedActor.CurrentAction != null)
                                //{
                                //    //Debug.Log(debugger.observedPlanner.m_agentData.Name + ": WUT???");
                                //    GUILayout.Button("", GUILayout.ExpandWidth(false));
                                //}

                                GUI.backgroundColor = Color.white;
                                GUILayout.EndHorizontal();
                                GUILayout.EndVertical();
                            }
                        }

                        GUI.color = Color.yellow;
                        GUILayout.Label("- Scripts -");
                        GUI.color = Color.white;
                        if(Application.isPlaying)
                        {
                            foreach(GameScript script in debugger.observedActor.Scripts)
                            {
                                GUILayout.BeginVertical();
                                GUILayout.BeginHorizontal();

                                if(script == null)
                                {
                                    GUILayout.Label("EMPTY", GUILayout.ExpandWidth(false));
                                }
                                else
                                {
                                    if(GUILayout.Button(script.GetType().Name, GUILayout.ExpandWidth(false)))
                                    {

                                    }

                                    if(script.debug_scriptupdating)
                                    {
                                        GUILayout.Label("U", GUILayout.ExpandWidth(false));
                                    }
                                }

                                GUI.backgroundColor = Color.white;
                                GUILayout.EndHorizontal();
                                GUILayout.EndVertical();
                            }
                        }
                    }
                    break;

                case 1: //? Stats
                    EditorPrefs.SetInt("tabIndex", 1);
                    using(new GUILayout.HorizontalScope())
                    {
                        using(new GUILayout.VerticalScope())
                        {
                            if(Application.isPlaying)
                            {
                                GUI.skin = cSkin;

                                foreach(System.Collections.Generic.KeyValuePair<ActorStat, int> stat in debugger.observedActor.ActorStats.StatsBase)
                                {
                                    GUILayout.Label(stat.Key + ": " + stat.Value);
                                }

                                GUI.skin = null;
                            }
                        }
                    }
                    //GUILayout.FlexibleSpace();
                    break;

                case 2: //? Status
                    EditorPrefs.SetInt("tabIndex", 2);
                    using(new GUILayout.HorizontalScope())
                    {
                        using(new GUILayout.VerticalScope())
                        {
                            if(Application.isPlaying)
                            {

                                GUI.skin = cSkin;

                                foreach(StatusEffect statusEff in debugger.observedActor.GetAppliedStatusEffects())
                                {
                                    if(statusEff == null || statusEff.rounds == 0)
                                    {
                                        continue;
                                    }

                                    GUILayout.Label(statusEff.statusEffect + " - " + statusEff.rounds + " Rounds");
                                }

                                GUI.skin = null;
                            }
                        }
                    }

                    break;
                case 3: //? Global
                    EditorPrefs.SetInt("tabIndex", 3);
                    using(new GUILayout.HorizontalScope())
                    {
                        using(new GUILayout.VerticalScope())
                        {
                            if(Application.isPlaying)
                            {

                                GUI.skin = cSkin;

                                GUILayout.Label("Party Attack: " + AoG.Core.GameInterface.Instance.GetCurrentGame()?.PartyAttack);
                                GUILayout.Label("Combat Counter: " + AoG.Core.GameInterface.Instance.GetCurrentGame()?.CombatCounter);

                                GUI.skin = null;
                            }
                        }
                    }

                    break;
            }
        }
        else
        {

            GUILayout.Label("Select Actor");
        }
        GUILayout.FlexibleSpace();

        GUILayout.Label("Actor: " + debugger.observedActor.GetName());

        DrawSelectPlannerDropDown();
    }

    private void DrawSelectPlannerDropDown()
    {
        if(EditorGUILayout.DropdownButton(new GUIContent(debugger != null && debugger.observedActor != null ? debugger.observedActor.GetName() : "NONE"), FocusType.Passive))
        {

            GenericMenu gMenu = new GenericMenu();

            Actor[] NPCs = FindObjectsOfType<Actor>();

            for(int i = 0; i < NPCs.Length; i++)
            {

                Actor npc = NPCs[i];

                gMenu.AddItem(new GUIContent(npc.gameObject.name), false, () => DropdownSelectNPC(npc));
            }

            gMenu.ShowAsContext();
        }
    }

    private void OnDestroy()
    {

    }

    private void DropdownSelectNPC(Actor npc)
    {

        debugger.SetObservedPlanner(npc);
        //aiCombat = npc.GetComponent<AI_CombatProtocol>();
        //aiInventory = npc.GetComponent<Inventory>();

        if(Application.isPlaying == false)
        {

            EditorSceneManager.MarkSceneDirty(npc.gameObject.scene);
        }
    }
}

public static class BackgroundStyle
{
    private static GUIStyle style = new GUIStyle();
    private static Texture2D texture = new Texture2D(1, 1);


    public static GUIStyle Get(Color color)
    {
        texture.SetPixel(0, 0, color);
        texture.Apply();
        style.normal.background = texture;
        return style;
    }
}