//using UnityEditor;
//using UnityEngine;

//public class FormulaTestingWindow : EditorWindow
//{
//    FormulaTesting _formulaTester;
//    //SerializedObject _serObjFormulaTester;

//    float _finalDamage;

//    void OnEnable()
//    {
//        if(_formulaTester == null)
//            _formulaTester = Resources.Load("ScriptableObjects/FormulaTesting", typeof(FormulaTesting)) as FormulaTesting;

//        if(_formulaTester == null)
//            Debug.LogError("FormulaTester may not be null at this point");


//            GameEventSystem.OnStatsToTesterOpp1 = agent => {

//                _formulaTester.Name = agent.Name;
//                //_formulaTester.isMagicAttack = agent.isSpellCaster;
//                _formulaTester.currentHealth = agent.GetBaseStat(Stat.HITPOINTS);
//                //_formulaTester.rawDamage = agent.isSpellCaster && agent.behaviours.combatController.currentSpell != null ? agent.behaviours.combatController.currentSpell.magnitude : 0;
//                //_formulaTester.pAttack = agent.actorData.m_pAttack;
//                //_formulaTester.mAttack = agent.actorData.m_mAttack;
//                //_formulaTester.pDef = agent.actorData.m_pDefence;
//                //_formulaTester.mDef = agent.actorData.m_mDefence;
//                Repaint();
//            };

//            GameEventSystem.OnStatsToTesterOpp2 = agent => {

//                _formulaTester.Name_opp = agent.Name;
//                //_formulaTester.isMagicAttack_opp = agent.isSpellCaster;
//                _formulaTester.currentHealth_opp = agent.GetBaseStat(Stat.HITPOINTS);
//                //_formulaTester.rawDamage_opp = agent.isSpellCaster && agent.behaviours.combatController.currentSpell != null ? agent.behaviours.combatController.currentSpell.magnitude : 0;
//                //_formulaTester.pAttack_opp = agent.actorData.m_pAttack;
//                //_formulaTester.mAttack_opp = agent.actorData.m_mAttack;
//                //_formulaTester.pDef_opp = agent.actorData.m_pDefence;
//                //_formulaTester.mDef_opp = agent.actorData.m_mDefence;
//                Repaint();
//            };
        
//    }
//    Vector2 scrollPos;
//    void OnGUI()
//    {
//        //_serObjFormulaTester = new SerializedObject(_formulaTester);
//        scrollPos = GUILayout.BeginScrollView(scrollPos);
//        GUI.color = Color.yellow;
//        GUILayout.Label("EXP");
//        GUI.color = Color.white;

//        _formulaTester.profession = (Class)EditorGUILayout.EnumPopup("Class", _formulaTester.profession);
//        _formulaTester.agentLevel = EditorGUILayout.IntField("Agent Level", _formulaTester.agentLevel);
//        _formulaTester.currentHealth = EditorGUILayout.FloatField("Agent Health", _formulaTester.currentHealth);

//        _formulaTester.enemyLevel = EditorGUILayout.IntField("Enemy Level", _formulaTester.enemyLevel);
//        _formulaTester.currentHealth = EditorGUILayout.FloatField("Enemy Health", _formulaTester.currentHealth);

//        //float result = perc * baseExp;
//        //source.Execute_IncreaseLevelProgress(result * GameMaster.Instance.gameSettings.globalExpMult);
//        if(GUILayout.Button("Calculate EXP"))
//        {
//            IncreaseLevelProgress(HelperFunctions.GetCalculatedEXP_WOW(_finalDamage, _formulaTester.agentLevel,_formulaTester.enemyLevel, _formulaTester.currentHealth));
//        }

//        GUILayout.BeginHorizontal();
//        _formulaTester.currentExp = EditorGUILayout.FloatField("EXP", _formulaTester.currentExp);
//        GUILayout.Label("/", GUILayout.Width(10));
//        _formulaTester.expNeeded = EditorGUILayout.IntField("", _formulaTester.expNeeded);
//        GUILayout.EndHorizontal();

//        if(GUILayout.Button("Reset Agents", GUILayout.Width(100)))
//        {
//            _formulaTester.agentLevel = 1;
//            _formulaTester.enemyLevel = 1;
//            _formulaTester.currentHealth = 100;
//            _formulaTester.currentHealth = 100;
//            _formulaTester.currentExp = 0;
//            _formulaTester.expNeeded = 50;
//        }

//        GUILayout.Space(5);
//        GUI.color = Color.yellow;
//        GUILayout.Label("Damage");
//        GUI.color = Color.white;

//        GUILayout.BeginHorizontal();
//        GUILayout.BeginVertical();

//        if(_formulaTester.Name_opp == null)
//        {
//            GUILayout.Label("Player");
//        }
//        else
//        {
//            GUILayout.Label(_formulaTester.Name);
//        }
//        DrawUILine(Color.grey, false, 1, 2);

//        EditorGUIUtility.labelWidth = 100;
//        _formulaTester.isMagicAttack = EditorGUILayout.Toggle("Magic Attack", _formulaTester.isMagicAttack);
//        _formulaTester.rawDamage = EditorGUILayout.FloatField("Raw Damage", _formulaTester.rawDamage);
//        _formulaTester.pAttack = EditorGUILayout.IntField("pAttack", _formulaTester.pAttack);
//        _formulaTester.pDef = EditorGUILayout.IntField("pDef", _formulaTester.pDef);
//        _formulaTester.mAttack = EditorGUILayout.IntField("mAttack", _formulaTester.mAttack);
//        _formulaTester.mDef = EditorGUILayout.IntField("mDef", _formulaTester.mDef);

//        if(GUILayout.Button("Reset Player Stats", GUILayout.Width(120)))
//        {
//            _finalDamage = 0;
//            _formulaTester.rawDamage = 10;
//            _formulaTester.pAttack = 0;
//            _formulaTester.pDef = 0;
//            _formulaTester.mAttack = 0;
//            _formulaTester.mDef = 0;
//        }

//        GUILayout.EndVertical();
//        GUILayout.BeginVertical();

//        GUILayout.Space(50);
//        if(GUILayout.Button(" -> Player dmg enemy"))
//        {
//            _finalDamage = HelperFunctions.GetCalculatedDamage(
//                _formulaTester.Name,
//                _formulaTester.Name_opp,
//                _formulaTester.isMagicAttack,
//                _formulaTester.rawDamage,
//                _formulaTester.pAttack,
//                _formulaTester.mAttack,
//                _formulaTester.pDef_opp,
//                _formulaTester.mDef_opp
//                );
//        }
//        if(GUILayout.Button(" <- Enemy dmg player"))
//        {
//            _finalDamage = HelperFunctions.GetCalculatedDamage(
//                _formulaTester.Name_opp,
//                _formulaTester.Name,
//                _formulaTester.isMagicAttack_opp,
//                _formulaTester.rawDamage_opp,
//                _formulaTester.pAttack_opp,
//                _formulaTester.mAttack_opp,
//                _formulaTester.pDef,
//                _formulaTester.mDef
//                );
//        }
//        GUILayout.EndVertical();
//        GUILayout.BeginVertical();

//        if(_formulaTester.Name_opp == null)
//        {
//            GUILayout.Label("Enemy");
//        }
//        else
//        {
//            GUILayout.Label(_formulaTester.Name_opp);
//        }


//        DrawUILine(Color.grey, false, 1, 2);

//        _formulaTester.isMagicAttack_opp = EditorGUILayout.Toggle("Magic Attack", _formulaTester.isMagicAttack_opp);
//        _formulaTester.rawDamage_opp = EditorGUILayout.FloatField("Raw Damage", _formulaTester.rawDamage_opp);
//        _formulaTester.pAttack_opp = EditorGUILayout.IntField("pAttack", _formulaTester.pAttack_opp);
//        _formulaTester.pDef_opp = EditorGUILayout.IntField("pDef", _formulaTester.pDef_opp);
//        _formulaTester.mAttack_opp = EditorGUILayout.IntField("mAttack", _formulaTester.mAttack_opp);
//        _formulaTester.mDef_opp = EditorGUILayout.IntField("mDef", _formulaTester.mDef_opp);
        
//        if(GUILayout.Button("Reset Enemy Stats", GUILayout.Width(120)))
//        {
//            _finalDamage = 0;
//            _formulaTester.rawDamage_opp = 10;
//            _formulaTester.pAttack_opp = 0;
//            _formulaTester.pDef_opp = 0;
//            _formulaTester.mAttack_opp = 0;
//            _formulaTester.mDef_opp = 0;
//        }

//        GUILayout.EndVertical();
//        GUILayout.EndHorizontal();

//        EditorGUILayout.FloatField("Final Damage", _finalDamage);

//        GUILayout.EndScrollView();

//        if(GUI.changed)
//        {
//            EditorUtility.SetDirty(_formulaTester);
//        }
//    }

//    public void IncreaseLevelProgress(float amount)
//    {
//        _formulaTester.currentExp += amount * _formulaTester.expMultiplier;

//        while(_formulaTester.currentExp >= _formulaTester.expNeeded)
//        {
//            switch(_formulaTester.profession)
//            {
//                case Class.Warrior:
//                    IncreaseStats(15, 0, 4, 0, 3, 1);
//                    break;
//                case Class.Wizard:
//                    IncreaseStats(7, 15, 1, 5, 1, 5);
//                    break;
//                case Class.Cleric:
//                    IncreaseStats(8, 12, 2, 4, 1, 4);
//                    break;
//                case Class.Rogue:
//                    IncreaseStats(11, 0, 7, 0, 2, 1);
//                    break;
//            }

//            _formulaTester.agentLevel++;
   
//            _formulaTester.currentExp -= _formulaTester.expNeeded;
//            //_formulaTester.expNeeded += _formulaTester.expNeeded * 2;
//            //_formulaTester.expNeeded = (int)HelperFunctions.GetCalculatedEXPIncrease(_formulaTester.agentLevel);
//        }
//    }

//    void IncreaseStats(float maxHealth, float maxMana, int pAttack, int mAttack, int pDef, int mDef)
//    {
//        _formulaTester.maxHealth = _formulaTester.currentHealth += maxHealth;
//        //agentData.m_maxMana = m_maxMana += maxMana;
//        _formulaTester.pAttack += pAttack;
//        _formulaTester.mAttack += mAttack;
//        _formulaTester.pDef += pDef;
//        _formulaTester.mDef += mDef;
//    }
//    public static void DrawUILine(Color color, bool vertical, int thickness = 2, int padding = 10)
//    {
//        Rect r = EditorGUILayout.GetControlRect(vertical ? GUILayout.Width(padding + thickness) : GUILayout.Height(padding + thickness));

//        if(vertical)
//        {
//            r.width = thickness;
//            r.x += padding / 2;
//            r.y -= 2;
//            r.height += 6;
//        }
//        else
//        {
//            r.height = thickness;
//            r.y += padding / 2;
//            r.x -= 2;
//            r.width += 6;
//        }

//        EditorGUI.DrawRect(r, color);
//    }

//}