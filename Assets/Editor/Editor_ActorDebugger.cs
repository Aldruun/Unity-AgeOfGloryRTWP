using GenericFunctions;
using System;
using System.Reflection;
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

    static GUISkin cSkin;
    static ActorDebugger debugger;
    static Actor _actor;
    //static AI_CombatProtocol aiCombat;
    //static Inventory aiInventory;
    int labelFontSize = 9;
    static string[] tabs;
    static int selectedTabIndex;

    void OnEnable()
    {

        tabs = new string[] { "Overview", "Stats", "Status Effects", "Global" };

        cSkin = Resources.Load<GUISkin>("GUI Skins/Editor GUISkin");

        selectedTabIndex = EditorPrefs.GetInt("tabIndex", selectedTabIndex);
        EditorPrefs.GetInt("labelFontSize", labelFontSize);

        //SelectionManager.OnAgentSelected += (agent) => { debugger.SetObservedPlanner(agent is PlanningProtocol ? (PlanningProtocol)agent : null); };
    }

    void OnDisable()
    {

        EditorPrefs.SetInt("tabIndex", selectedTabIndex);
        EditorPrefs.SetInt("labelFontSize", labelFontSize);
    }

    void OnGUI()
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
            //    debugger
            //}

            GUILayout.FlexibleSpace();
            GUILayout.Label("Observed Actor (Runtime Only)");


            DrawSelectPlannerDropDown();

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

            switch(selectedTabIndex)
            {

                case 0:
                    //GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();
                    EditorPrefs.SetInt("tabIndex", 0);

                    GUI.skin = cSkin;

                    GUILayout.BeginVertical();

                    GUI.color = Color.yellow;
                    GUILayout.Label("- Actor State -");
                    GUI.color = Color.white;
                    //GUI.skin = cSkin;
                    if(Application.isPlaying)
                    {
                        GUILayout.Label(observee.GetName() + ", " + observee.ActorStats.Class.ToString() + "(" + observee.ActorStats.Level + ")");
                        string xpNeeded = observee.ActorStats.Level == 20 ? "MAX" : observee.ActorStats.expNeeded.ToString();
                        GUILayout.Label(string.Format("XP: {0}/{1}", observee.ActorStats.currentExp, xpNeeded));
                        GUILayout.Label(string.Format("HP: {0}/{1}", observee.ActorStats.GetBaseStat(ActorStat.HITPOINTS), observee.ActorStats.GetBaseStat(ActorStat.MAXHITPOINTS)));
                        GUILayout.Label(string.Format(
                            "STR: {0} [{6}], " +
                            "DEX: {1} [{7}], " +
                            "CON: {2} [{8}], " +
                            "INT: {3} [{9}], " +
                            "WIS: {4} [{10}], " +
                            "CHA: {5} [{11}]",
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
                        //foreach(WorldStateData kvp in debugger.observedPlanner.behaviours.actionSelector.worldState)
                        //{

                        //    bool state = (kvp.state == true);

                        //    GUI.skin.label.normal.textColor = Color.white;
                        //    if(state)
                        //    {

                        //        GUI.backgroundColor = Color.green;
                        //    }
                        //    else
                        //    {

                        //        GUI.backgroundColor = Color.red;
                        //    }
                        //    string colorTag = (state == true) ? "<color=green>" : "<color=red>";
                        //    GUILayout.Label(kvp.symbol + " = " + colorTag + state + "</color>");

                        //    GUI.skin.label.normal.textColor = Color.grey;
                        //    GUI.backgroundColor = Color.white;
                        //}
                    }
                    //GUI.skin = null;
                    GUILayout.EndVertical();
                    //GUILayout.EndHorizontal();
                    //? Misc debugging
                    GUILayout.BeginVertical();
                    if(Application.isPlaying)
                    {
                        if(observee.Combat.GetHostileTarget() == null)
                            GUILayout.Label("Attack Target: null");
                        else
                            GUILayout.Label("Attack Target: " + observee.Combat.GetHostileTarget().GetName());
                        GUILayout.Label("Enemy Flags: " + debugger.observedActor.ActorStats.GetEnemyFlags().ToString());
                        if(observee.Equipment.equippedWeapon.Weapon != null)
                        {
                            GUILayout.Label("Weapon ID: " + observee.Equipment.equippedWeapon.Weapon.identifier);
                        }
                        GUILayout.Label("Global ID: " + observee.GetGlobalID());
                        GUILayout.Label("Selected: " + observee.ActorUI.Selected);
                        GUILayout.Label("Party Index: " + observee.PartySlot);
                        GUILayout.Label("Is Downed: " + observee.isDowned);
                    }
                    GUILayout.EndVertical();
                    //? Knowledge and belongings

                    //GUILayout.FlexibleSpace();
                    if(Application.isPlaying)
                    {

                        GUI.skin = cSkin;
                        //GUI.skin.label.normal.textColor = Color.grey;
                        GUILayout.Label("Spells:");
                        //if(debugger.plan.Key == null)
                        //{

                        //    GUILayout.Label("< No Plan >");
                        //}
                        //else
                        //{

                        //    GUILayout.BeginHorizontal();

                        //    foreach(NPCAction gAction in debugger.plan.Value)
                        //    {

                        //        GUILayout.Label(gAction.GetType().Name + " -> ");
                        //    }
                        //    GUILayout.Label(debugger.plan.Key.GetType().Name);

                        //    GUILayout.EndHorizontal();
                        //}

                        GUILayout.Label("Item Count:");
                        //if(debugger.lastPlan.Key == null)
                        //{

                        //    GUILayout.Label("< No Plan >");
                        //}
                        //else
                        //{

                        //    GUILayout.BeginHorizontal();

                        //    foreach(NPCAction gAction in debugger.lastPlan.Value)
                        //    {

                        //        GUILayout.Label(gAction.GetType().Name + " -> ");
                        //    }
                        //    GUILayout.Label(debugger.lastPlan.Key.GetType().Name);

                        //    GUILayout.EndHorizontal();
                        //}
                        //GUI.skin.label.normal.textColor = Color.clear;
                        GUI.skin = null;
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
                            GUILayout.BeginVertical();
                            if(GUILayout.Button(debugger.observedActor.CurrentAction.ToString(), GUILayout.ExpandWidth(false)))
                            {

                            }
                            if(add != "")
                                GUILayout.Label("[" + add + "]");
                            GUILayout.EndVertical();
                        }
                        else
                        {
                            GUILayout.Label("-");
                        }
                        //else if(debugger.observedActor.actionQueue.Count == 0)
                        //{
                        //    GUILayout.Label("0");
                        //}
                        foreach(GameAction action in debugger.observedActor.ActionQueue)
                        {
                            //if(gAction.active == false) {

                            //    continue;
                            //}

                            GUILayout.Space(1);

                            GUILayout.BeginVertical();
                            GUILayout.BeginHorizontal();
                            //if(action.valid)
                            //{
                            //    GUI.backgroundColor = Color.green;

                            //}
                            //else
                            //{

                            //    GUI.backgroundColor = Color.red;
                            //}

                            if(GUILayout.Button(action.ToString(), GUILayout.ExpandWidth(false)))
                            {


                            }

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
                            //debugger.observedPlanner.currentGoal

                            GUILayout.BeginVertical();
                            GUILayout.BeginHorizontal();

                            if(script == null)
                            {
                                GUILayout.Label("EMPTY", GUILayout.ExpandWidth(false));
                            }
                            else
                            {
                                //if(gGoal.relevant)
                                //{

                                //    GUI.backgroundColor = Color.green;
                                //}
                                //else
                                //{

                                //    GUI.backgroundColor = Color.red;
                                //}

                                if(GUILayout.Button(script.ToString(), GUILayout.ExpandWidth(false)))
                                {

                                }

                                if(script.debug_scriptupdating)
                                {

                                    //GUILayout.Label("<-");
                                    GUILayout.Label("U", GUILayout.ExpandWidth(false));
                                }
                            }

                            GUI.backgroundColor = Color.white;
                            GUILayout.EndHorizontal();
                            GUILayout.EndVertical();
                        }

                    }
                    GUILayout.EndVertical();


                    break;
                case 1: //? Stats
                    EditorPrefs.SetInt("tabIndex", 1);
                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();
                    if(Application.isPlaying)
                    {

                        GUI.skin = cSkin;

                        foreach(System.Collections.Generic.KeyValuePair<ActorStat, int> stat in debugger.observedActor.ActorStats.StatsBase)
                        {
                            GUILayout.Label(stat.Key + ": " + stat.Value);
                        }

                        GUI.skin = null;
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    //GUILayout.FlexibleSpace();
                    break;
                case 2: //? Status
                    EditorPrefs.SetInt("tabIndex", 2);
                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();
                    if(Application.isPlaying)
                    {

                        GUI.skin = cSkin;

                        foreach(StatusEffect statusEff in debugger.observedActor.GetAppliedStatusEffects())
                        {
                            if(statusEff == null || statusEff.rounds == 0)
                                continue;
                            GUILayout.Label(statusEff.statusEffect + " - " + statusEff.rounds + " Rounds");
                        }

                        GUI.skin = null;
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    //GUILayout.FlexibleSpace();
                    break;
                case 3: //? Global
                    EditorPrefs.SetInt("tabIndex", 3);
                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();
                    if(Application.isPlaying)
                    {

                        GUI.skin = cSkin;

                        GUILayout.Label("Party Attack" + AoG.Core.GameInterface.Instance.GetCurrentGame()?.PartyAttack);
                        GUILayout.Label("Combat Counter" + AoG.Core.GameInterface.Instance.GetCurrentGame()?.CombatCounter);

                        GUI.skin = null;
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    //GUILayout.FlexibleSpace();
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

    void OnDestroy()
    {

    }

    void DropdownSelectNPC(Actor npc)
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