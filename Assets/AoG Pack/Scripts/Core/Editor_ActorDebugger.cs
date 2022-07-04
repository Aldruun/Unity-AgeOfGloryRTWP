using AoG.Core;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Editor_ActorDebugger : EditorWindow
{
    //void OnGUI() {
    //    GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), tex, ScaleMode.StretchToFill);
    //    GUI.Label(new Rect(200, 200, 100, 100), "A label");
    //    GUI.TextField(new Rect(20, 20, 70, 30), "");
    //}

    //static GUISkin cSkin;
    private static ActorDebugger debugger;
    private int labelFontSize = 9;
    private static string[] tabs;
    private static int selectedTabIndex;
    private ActorInput _observee;
    private Vector2 scrollPosition_ActorData;

    private void OnEnable()
    {

        tabs = new string[] { "Overview", "Stats", "Status Effects", "Inventory", "Global" };

        //cSkin = Resources.Load<GUISkin>("GUI Skins/Editor GUISkin");

        //if(EditorPrefs.HasKey("tabIndex"))
        //    EditorPrefs.DeleteKey("tabIndex");
        //if(EditorPrefs.HasKey("labelFontSize"))
        //    EditorPrefs.DeleteKey("labelFontSize");
        //selectedTabIndex = EditorPrefs.GetInt("tabIndex", selectedTabIndex);
        //EditorPrefs.GetInt("labelFontSize", labelFontSize);

        //SelectionManager.OnAgentSelected += (agent) => { debugger.SetObservedPlanner(agent is PlanningProtocol ? (PlanningProtocol)agent : null); };
    }

    private void OnDisable()
    {

        //EditorPrefs.SetInt("tabIndex", selectedTabIndex);
        //EditorPrefs.SetInt("labelFontSize", labelFontSize);
    }

    private void OnGUI()
    {

        //if(Application.isPlaying)
        //{
        //    if(debugger != null && debugger.observedActor == null)
        //    {
        //        debugger.SetObservedPlanner(GameStateManager.GetPC());
        //    }
        //}

        //GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), tex, ScaleMode.StretchToFill);

        //buttonFontSize = EditorGUILayout.IntSlider(buttonFontSize, 1, 20);
        //GUI.skin.button.fontSize = buttonFontSize;
        if(debugger == null)
        {
            GUILayout.Label("Initializing...");
            debugger = GameObject.FindWithTag("PersistentManagers").GetComponent<ActorDebugger>();
            return;
        }

        _observee = debugger.observedActor;
        if(_observee == null)
        {
            //if(Application.isPlaying)
            //{
            //    debugger
            //}

            GUILayout.FlexibleSpace();
            GUILayout.Label("Observed Actor (Runtime Only)");



            return;
        }
        DrawSelectPlannerDropDown();
        if(_observee == null)
            _observee = null;
        //if(_observee != debugger.observedActor)
        Repaint();

        if(debugger.observedActor == null)
        {
            GUI.color = Color.yellow;
            GUILayout.Label("Actor's action queue is null");
            GUI.color = Color.white;
        }

        if(_observee != null && _observee.gameObject.activeSelf)
        {

            selectedTabIndex = GUILayout.Toolbar(selectedTabIndex, tabs/*cSkin.GetStyle("Goap Runtime Editor Tab")*/);

            switch(selectedTabIndex)
            {

                case 0://? Actor Data
                    {
                        //GUILayout.BeginHorizontal();
                       
                        using(var scrollView = new GUILayout.ScrollViewScope(scrollPosition_ActorData))
                        {
                            scrollPosition_ActorData = scrollView.scrollPosition;
                            //EditorPrefs.SetInt("tabIndex", 0);

                            //GUI.skin = cSkin;

                            using(new GUILayout.VerticalScope())
                            {

                                GUI.color = Color.yellow;
                                GUILayout.Label("- Actor State -");
                                GUI.color = Color.white;
                                //GUI.skin = cSkin;
                                if(Application.isPlaying)
                                {
                                    GUILayout.Label(_observee.GetName() + ", " + _observee.ActorStats.Race + " " + _observee.ActorStats.Class.ToString() + "(" + _observee.ActorStats.Level + ")");
                                    string xpNeeded = _observee.ActorStats.Level == 20 ? "MAX" : _observee.ActorStats.expNeeded.ToString();
                                    GUILayout.Label(string.Format("XP: {0}/{1}", _observee.ActorStats.currentExp, xpNeeded));
                                    GUILayout.Label(string.Format("HP: {0}/{1}", ActorUtility.GetAttributeBase(_observee.ActorStats, ActorStat.HEALTH), ActorUtility.GetAttributeBase(_observee.ActorStats, ActorStat.MAXHEALTH)));
                                    //GUILayout.Label(string.Format("ST: {0}/{1}", ActorUtility.GetAttributeBase(_observee.ActorRecord, ActorStats.STUN), ActorUtility.GetAttributeBase(_observee.ActorRecord, ActorStats.MAXSTUN)));
                                    GUILayout.Label(string.Format(
                                        $"AC: {ActorUtility.GetAttributeBase(_observee.ActorStats, ActorStat.AC)} [{ActorUtility.GetModdedAttribute(_observee.ActorStats, ActorStat.AC)}], " +
                                        $"APR: {ActorUtility.GetAttributeBase(_observee.ActorStats, ActorStat.APR)} [{ActorUtility.GetModdedAttribute(_observee.ActorStats, ActorStat.APR)}], " +
                                        $"STR: {ActorUtility.GetAttributeBase(_observee.ActorStats, ActorStat.STRENGTH)} [{ActorUtility.GetModdedAttribute(_observee.ActorStats, ActorStat.STRENGTH)}], " +
                                        $"DEX: {ActorUtility.GetAttributeBase(_observee.ActorStats, ActorStat.DEXTERITY)} [{ActorUtility.GetModdedAttribute(_observee.ActorStats, ActorStat.DEXTERITY)}], " +
                                        $"CON: {ActorUtility.GetAttributeBase(_observee.ActorStats, ActorStat.CONSTITUTION)} [{ActorUtility.GetModdedAttribute(_observee.ActorStats, ActorStat.CONSTITUTION)}], " +
                                        $"INT: {ActorUtility.GetAttributeBase(_observee.ActorStats, ActorStat.INTELLIGENCE)} [{ActorUtility.GetModdedAttribute(_observee.ActorStats, ActorStat.INTELLIGENCE)}], " +
                                        $"WIL: {ActorUtility.GetAttributeBase(_observee.ActorStats, ActorStat.WISDOM)} [{ActorUtility.GetModdedAttribute(_observee.ActorStats, ActorStat.WISDOM)}], " +
                                        $"CHA: {ActorUtility.GetAttributeBase(_observee.ActorStats, ActorStat.CHARISMA)} [{ActorUtility.GetModdedAttribute(_observee.ActorStats, ActorStat.CHARISMA)}], "
                                        ));
                                    GUILayout.Label(string.Format("AR: {0}", GameMechanics.GetArmorRating(_observee.ActorStats)));

                                }

                                //? Misc debugging

                                if(Application.isPlaying)
                                {
                                    if(_observee.Combat.GetHostileTarget() == null)
                                        GUILayout.Label("Attack Target: null");
                                    else
                                        GUILayout.Label("Attack Target: " + _observee.Combat.GetHostileTarget().GetName());
                                    //GUILayout.Label("Enemy Flags: " + debugger.observedActor.GetEnemyFlags().ToString());
                                    if(_observee.Equipment.equippedWeapon.Weapon != null)
                                    {
                                        GUILayout.Label("Weapon ID: " + _observee.Equipment.equippedWeapon.Weapon.identifier);
                                        GUILayout.Label("  Weapon Type: " + _observee.Equipment.equippedWeapon.Weapon.weaponType);
                                        GUILayout.Label("  Damage Type: " + _observee.Equipment.equippedWeapon.Weapon.damageType);
                                        GUILayout.Label("  Weapon Range: " + _observee.Equipment.equippedWeapon.Weapon.range);
                                        GUILayout.Label("  Combat Type: " + _observee.Equipment.equippedWeapon.Weapon.combatType);
                                        GUILayout.Label("  Equip Type: " + _observee.Equipment.equippedWeapon.Weapon.equipType);
                                        GUILayout.Label("  Animation Pack: " + _observee.Equipment.equippedWeapon.Weapon.animationPack);
                                        GUILayout.Label("  Ammo Type: " + _observee.Equipment.equippedWeapon.Weapon.ammoType);
                                        GUILayout.Label("  Projectile ID: " + _observee.Equipment.equippedWeapon.Weapon.projectileIdentifier);
                                    }
                                    //GUILayout.Label("Global ID: " + observee.GetGlobalID());
                                    //GUILayout.Label("Selected: " + observee.selected);
                                    GUILayout.Label("Escorts: " + _observee.EscortIndex);
                                    // if(_observee is NPCInput npc)
                                    //     GUILayout.Label("Escort Target: " + (npc.escortTarget == null ? "NONE" : npc.escortTarget.ActorRecord.Name));
                                    GUILayout.Label("Is Downed: " + _observee.isDowned);
                                    GUILayout.Label("Is Downed: " + debugger.observedActor.isDowned);
                                    GUILayout.Label("Is Beast: " + _observee.ActorStats.isBeast);
                                    GUILayout.Label("Is Casting: " + _observee.isCasting);
                                    GUILayout.Label("Is Cloaked: " + _observee.isCloaked);
                                }


                            }


                            //? Knowledge and belongings

                            if(Application.isPlaying)
                            {
                                GUILayout.Label("Spells:");


                                GUILayout.Label("Item Count:");
                            }


                            GUI.color = Color.yellow;
                            GUILayout.Label("- Actions -");
                            GUI.color = Color.white;
                            if(Application.isPlaying)
                            {
                                if(debugger.observedActor is NPCInput npc)
                                {
                                    GUILayout.Label("NPC State: " + npc.npcState);
                                }
                                else
                                {
                                    GUILayout.Label("-");
                                }
                            }

                            GUI.color = Color.yellow;
                            GUILayout.Label("- Scripts -");
                            GUI.color = Color.white;
                            if(Application.isPlaying)
                            {

                            }

                            GUILayout.Label("- Navigatiom -");
                            GUI.color = Color.white;
                            if(Application.isPlaying)
                            {
                                if(_observee.NavAgent != null)
                                {
                                    GUILayout.Label("Has Path: " + _observee.NavAgent.hasPath);
                                    GUILayout.Label("Vel: " + _observee.NavAgent.velocity);
                                    GUILayout.Label("Desired Vel: " + _observee.NavAgent.desiredVelocity);
                                }
                                GUILayout.Label("Move Speed: " + _observee.Animation.m_movementSpeed);
                            }
                        }


                        break;
                    }
                case 1: //? Stats
                    {
                        //EditorPrefs.SetInt("tabIndex", 1);
                        using(new GUILayout.HorizontalScope())
                        {
                            using(new GUILayout.VerticalScope())
                            {
                                if(Application.isPlaying)
                                {

                                    //GUI.skin = cSkin;

                                    //foreach(ActorSkill stat in debugger.observedActor.ActorRecord.skills)
                                    //{
                                    //    GUILayout.Label(stat.data.Name + ": " + stat.level);
                                    //}

                                    //GUI.skin = null;
                                }
                            }
                        }
                        //GUILayout.FlexibleSpace();
                        break;
                    }
                case 2: //? Status
                    {
                        using(new GUILayout.HorizontalScope())
                        {
                            using(new GUILayout.VerticalScope())
                            {
                                if(Application.isPlaying)
                                {
                                    foreach(var eff in debugger.observedActor.GetAppliedStatusEffects())
                                    {
                                        GUILayout.Label(eff.statusEffect + ": " + eff.rounds + "s");
                                    }
                                }
                            }
                        }
                        break;
                    }
                case 3: //? Inventory
                    {
                        using(new GUILayout.HorizontalScope())
                        {
                            using(new GUILayout.VerticalScope())
                            {
                                if(Application.isPlaying)
                                {
                                    foreach(InventoryItem item in debugger.observedActor.Inventory.inventoryItems)
                                    {
                                        if(item.itemData == null)
                                        {
                                            GUILayout.Label("Item data null");
                                            continue;
                                        }
                                        GUILayout.Label(item.slotIndex + " " + item.itemData.Name + " x " + item.stackSize);
                                    }
                                }
                            }
                        }
                        break;
                    }
                case 4: //? Global
                    {
                        using(new GUILayout.HorizontalScope())
                        {
                            using(new GUILayout.VerticalScope())
                            {

                            }
                        }
                        break;
                    }
            }
        }
        //else
        //{

        //    GUILayout.Label("Select Actor");
        //}
        GUILayout.FlexibleSpace();

        if(Application.isPlaying)
            DrawSelectPlannerDropDown();
    }

    private void DrawSelectPlannerDropDown()
    {
        GUILayout.Label("Actor: " + debugger.observedActor.GetName());
        if(EditorGUILayout.DropdownButton(new GUIContent(debugger != null && debugger.observedActor != null ? debugger.observedActor.GetName() : "NONE"), FocusType.Passive))
        {

            GenericMenu gMenu = new GenericMenu();

            List<ActorInput> NPCs = debugger.registeredRuntimeActors;

            for(int i = 0; i < NPCs.Count; i++)
            {

                ActorInput npc = NPCs[i];

                gMenu.AddItem(new GUIContent(npc.GetName()), false, () => DropdownSelectNPC(npc));
            }

            gMenu.ShowAsContext();
        }
    }

    private void OnDestroy()
    {

    }

    private void DropdownSelectNPC(ActorInput npc)
    {

        debugger.SetObservedPlanner(npc);
        //if(Application.isPlaying == false)
        //{

        //    EditorSceneManager.MarkSceneDirty(npc);
        //}
    }
}