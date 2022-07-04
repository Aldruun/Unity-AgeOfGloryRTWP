using System.Linq;
using UnityEditor;
using UnityEngine;

public class ConstructionHelper : ScriptableWizard
{
    [MenuItem("GameObject Utilities/Flip On X Axis", false, 0)]
    private static void FlipX()
    {
        var mirroredObject = Instantiate(Selection.activeGameObject, Selection.activeTransform.parent);
        mirroredObject.name = mirroredObject.name.Substring(0, mirroredObject.name.Length - 7);
        mirroredObject.GetComponentsInChildren<Transform>()
            .ToList()
            .ForEach(t =>
            {
                t.localPosition = new Vector3(-t.localPosition.x, t.localPosition.y, t.localPosition.z);
                t.localEulerAngles = new Vector3(t.localEulerAngles.x, -t.localEulerAngles.y, -t.localEulerAngles.z);
                //var ca = t.GetComponent<CurveAnimation>
            });
    }

    [MenuItem("GameObject Utilities/Flip On Y Axis", false, 0)]
    private static void FlipY()
    {
        var mirroredObject = Instantiate(Selection.activeGameObject, Selection.activeTransform.parent);
        mirroredObject.name = mirroredObject.name.Substring(0, mirroredObject.name.Length - 7);
        mirroredObject.GetComponentsInChildren<Transform>()
            .ToList()
            .ForEach(t =>
            {
                t.localPosition = new Vector3(t.localPosition.x, -t.localPosition.y, t.localPosition.z);
                t.localEulerAngles = new Vector3(t.localEulerAngles.x, 180, 180);
                //var ca = t.GetComponent<CurveAnimation>
            });
    }
}