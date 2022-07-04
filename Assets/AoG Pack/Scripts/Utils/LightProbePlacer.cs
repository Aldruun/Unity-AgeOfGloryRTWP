using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class LightProbePlacer : EditorWindow
{
    private static float progress;
    private static string current = "Hello";
    private static bool working;

    private float mergeDistance = 1;
    private GameObject probeObject;

    [MenuItem("Window/Generate Light Probes")]
    private static void Init()
    {
        var window = GetWindow(typeof(LightProbePlacer));
        window.Show();
    }

    private void PlaceProbes()
    {
        var probe = probeObject;
        if (probe != null)
        {
            var p = probe.GetComponent<LightProbeGroup>();

            if (p != null)
            {
                working = true;

                progress = 0.0f;
                current = "Triangulating navmesh...";
                EditorUtility.DisplayProgressBar("Generating probes", current, progress);


                probe.transform.position = Vector3.zero;

                var navMesh = NavMesh.CalculateTriangulation();


                current = "Generating necessary lists...";
                EditorUtility.DisplayProgressBar("Generating probes", current, progress);

                var newProbes = navMesh.vertices;
                var probeList = new List<Vector3>(newProbes);
                var probeGen = new List<ProbeGenPoint>();

                foreach (var pg in probeList) probeGen.Add(new ProbeGenPoint(pg, false));

                EditorUtility.DisplayProgressBar("Generating probes", current, progress);

                var mergedProbes = new List<Vector3>();

                var probeListLength = newProbes.Length;

                var done = 0;
                foreach (var pro in probeGen)
                    if (pro.used == false)
                    {
                        current = "Checking point at " + pro.point;
                        progress = done / (float) probeListLength;
                        EditorUtility.DisplayProgressBar("Generating probes", current, progress);
                        var nearbyProbes = new List<Vector3>();
                        nearbyProbes.Add(pro.point);
                        pro.used = true;
                        foreach (var pp in probeGen)
                            if (pp.used == false)
                            {
                                current = "Checking point at " + pro.point;
                                //EditorUtility.DisplayProgressBar ("Generating probes", current, progress);
                                if (Vector3.Distance(pp.point, pro.point) <= mergeDistance)
                                {
                                    pp.used = true;
                                    nearbyProbes.Add(pp.point);
                                }
                            }

                        var newProbe = new Vector3();
                        foreach (var prooo in nearbyProbes) newProbe += prooo;
                        newProbe /= nearbyProbes.ToArray().Length;
                        newProbe += Vector3.up;

                        mergedProbes.Add(newProbe);
                        done += 1;
                        //Debug.Log ("Added probe at point " + newProbe.ToString ());
                    }

                /*for(int i=0; i<newProbes.Length; i++) {
					newProbes[i] = newProbes[i] + Vector3.up;
				}*/


                current = "Final steps...";
                EditorUtility.DisplayProgressBar("Generating probes", current, progress);

                p.probePositions = mergedProbes.ToArray();
                EditorUtility.DisplayProgressBar("Generating probes", current, progress);

                working = false;
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Probe object does not have a Light Probe Group attached to it",
                    "OK");
            }
        }
        else
        {
            EditorUtility.DisplayDialog("Error", "Probe object not set", "OK");
        }
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Generate probes")) PlaceProbes();
        mergeDistance = EditorGUILayout.FloatField("Vector merge distance", mergeDistance);
        probeObject =
            (GameObject) EditorGUILayout.ObjectField("Probe GameObject", probeObject, typeof(GameObject), true);
        EditorGUILayout.LabelField(
            "This script will automatically generate light probe positions based on the current navmesh.");
        EditorGUILayout.LabelField("Please make sure that you have generated a navmesh before using the script.");

        if (working)
            EditorUtility.DisplayProgressBar("Generating probes", current, progress);
        else
            EditorUtility.ClearProgressBar();
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }
}

public class ProbeGenPoint
{
    public Vector3 point;
    public bool used;

    public ProbeGenPoint(Vector3 p, bool u)
    {
        point = p;
        used = u;
    }
}