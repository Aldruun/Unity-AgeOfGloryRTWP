using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Editor_FactionTable : Editor
{
    [MenuItem("AoG Utilities/Faction Table", false, 1)]
    public static void FactionTable()
    {
        var factionTable = EditorWindow.GetWindow(typeof(FactionTableWindow)) as FactionTableWindow;
        factionTable.titleContent = new GUIContent("Faction Table");
        factionTable.Show();
    }
}

public class FactionTableWindow : EditorWindow
{
    private static string[] factionNames;
    private readonly float columnSpacing = 1.0f;

    private readonly int factionCount;

    private FactionRelationManager factionManager;
    private readonly float frontOffset = 4;
    private float labelWidth;
    private List<FactionRelation> relMap;

    public FactionTableWindow()
    {
        factionCount = Enum.GetNames(typeof(Faction)).Length;
    }

    private void OnInspectorUpdate()
    {
    }

    private void OnEnable()
    {
        if (factionManager == null)
            factionManager =
                Resources.Load("ScriptableObjects/FactionRelationManager", typeof(FactionRelationManager)) as
                    FactionRelationManager;

        if (factionManager == null) Debug.LogError("Faction Manager may not be null at this point");

        factionNames = Enum.GetNames(typeof(Faction));

        for (var i = 0; i < factionManager.relations.Count; i++)
        {
            var rel = factionManager.relations[i];

            if (factionNames.Contains(rel.relationData.faction1.ToString()) == false ||
                factionNames.Contains(rel.relationData.faction2.ToString()) == false)
                factionManager.relations.Remove(rel);
        }
    }

    private void OnGUI()
    {
        labelWidth = GUI.skin.label.CalcSize(new GUIContent(Enum.GetNames(typeof(Faction))
            .Aggregate("", (max, cur) => max.Length > cur.Length ? max : cur))).x;

        GUILayout.BeginHorizontal();
        for (var c = factionCount; c > 0; --c) //Vector2 pos = GUILayoutUtility.GetLastRect().center;
            //EditorGUIUtility.RotateAroundPivot(90, pos);

            GUILayout.Label(((Faction) c).ToString(), GUILayout.Width(labelWidth + columnSpacing));
        //EditorGUIUtility.RotateAroundPivot(-90, pos);
        GUILayout.EndHorizontal();

        //GUILayout.Space(topSpacing);

        for (var r = 0; r < factionCount - 1; r++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(labelWidth - GUI.skin.label.CalcSize(new GUIContent(((Faction) r).ToString())).x +
                            frontOffset);
            GUILayout.Label(((Faction) r).ToString());

            for (var c = factionCount - 1; c > 0; --c)
            {
                if (c <= r
                    ) //GUILayout.Box("", GUILayout.Width(labelWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight));

                    //GUILayout.Space(columnWidth + columnSpacing);
                    //GUILayout.Space(columnSpacing);
                    continue;

                var fr = GetRelation((Faction) c, (Faction) r);

                if (fr == null)
                {
                    fr = new FactionRelation((Faction) c, (Faction) r, 0);
                    factionManager.relations.Add(fr);
                    Debug.Log("Adding faction relation " + (Faction) r + " to " + ((Faction) c));
                }

                fr.relationData.relations =
                    EditorGUILayout.IntField(fr.relationData.relations, GUILayout.Width(labelWidth));

                GUILayout.Space(columnSpacing);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Save Changes"))
        {
            EditorUtility.SetDirty(factionManager);
            AssetDatabase.SaveAssets();
        }

        if (GUILayout.Button("Save Changes & Close"))
        {
            EditorUtility.SetDirty(factionManager);
            AssetDatabase.SaveAssets();
            Close();
        }
    }

    private bool Contains(Faction fac1, Faction fac2)
    {
        for (var i = 0; i < factionManager.relations.Count; i++)
            if (factionManager.relations[i].relationData.faction1 == fac1 &&
                factionManager.relations[i].relationData.faction2 ==
                fac2 /*|| (factionManager.relations[i].factionID1 == fac2 && factionManager.relations[i].factionID2 == fac1)*/
            )
                return true;

        return false;
    }

    private FactionRelation GetRelation(Faction fac1, Faction fac2)
    {
        //if(fac1 == Faction.None || fac2 == Faction.None) {

        //    return null;
        //}

        for (var i = 0; i < factionManager.relations.Count; i++)
            if (factionManager.relations[i].relationData.faction1 == fac1 &&
                factionManager.relations[i].relationData.faction2 == fac2 ||
                factionManager.relations[i].relationData.faction1 == fac2 &&
                factionManager.relations[i].relationData.faction2 == fac1)
                return factionManager.relations[i];

        return null;
    }
}