using UnityEditor;
using UnityEngine;

public class AnimationSettingsHelper : AssetPostprocessor
{
    private AnimationClip[] clips;

    [MenuItem("AoG Utilities/Animation/AssetToClipName")]
    private static void AssetToClipName()
    {
        
        for(int i = 0; i < Selection.objects.Length; ++i)
        {

            Object obj = Selection.objects[i];
            string szPath = AssetDatabase.GetAssetPath(obj);
            ModelImporter modelImporter = AssetImporter.GetAtPath(szPath) as ModelImporter;
            ModelImporterClipAnimation[] defaultClipAnimations = modelImporter.defaultClipAnimations;


            defaultClipAnimations[0].name = obj.name;


            //modelImporter.animationType = ModelImporterAnimationType.Human;
            modelImporter.clipAnimations = defaultClipAnimations;
            //modelImporter.SaveAndReimport();
        }
    }

    //void OnPreprocessAnimation()
    //{
    //    Debug.Log("Preprocessing");
    //    for(int i = 0; i < Selection.objects.Length; ++i)
    //    {
    //        Debug.Log("Preprocessing loop");
    //        Object obj = Selection.objects[i];
    //        string szPath = AssetDatabase.GetAssetPath(obj);
    //        ModelImporter modelImporter = AssetImporter.GetAtPath(szPath) as ModelImporter;
    //        var defaultClipAnimations = modelImporter.clipAnimations;

    //        for(int c = 0; c < defaultClipAnimations.Length; c++)
    //        {
    //            //ModelImporterClipAnimation clipAnimation in modelImporter.defaultClipAnimations)
    //            ModelImporterClipAnimation clip = defaultClipAnimations[c];
            
    //            clip.curves = new ClipAnimationInfoCurve[2];
    //            List<ClipAnimationInfoCurve> allcurves = new List<ClipAnimationInfoCurve>(modelImporter.clipAnimations[0].curves);
    //            foreach(ClipAnimationInfoCurve curveArray in allcurves)
    //            {
    //                ClipAnimationInfoCurve curves = new ClipAnimationInfoCurve();
    //                curves.curve = new AnimationCurve();
    //                curves.curve.AddKey(0f, -1f);
    //                curves.curve.AddKey(0.5f, 1f);
    //                curves.curve.AddKey(1f, -1f);
    //            }
    //            modelImporter.clipAnimations[c].curves = allcurves.ToArray();
    //            //defaultClipAnimations[c].curves = allcurves.ToArray();
    //        }

    //        modelImporter.animationType = ModelImporterAnimationType.Human;
    //        //modelImporter.clipAnimations = defaultClipAnimations;
    //        AssetDatabase.WriteImportSettingsIfDirty(modelImporter.assetPath);
    //    }
    //    AssetDatabase.SaveAssets();
    //}

    [MenuItem("AoG Utilities/Animation/ApplyCustomClipSettingsWithLoop")]
    private static void ApplyCustomClipSettingsWithLoop()
    {

        for(int i = 0; i < Selection.objects.Length; ++i)
        {

            Object obj = Selection.objects[i];
            string szPath = AssetDatabase.GetAssetPath(obj);
            ModelImporter modelImporter = AssetImporter.GetAtPath(szPath) as ModelImporter;
            var defaultClipAnimations = modelImporter.defaultClipAnimations;

            //ModelImporterClipAnimation clipAnimation = GetModelImporterClip(modelImporter);
            //clipAnimation.name = obj.name;
            //clipAnimation.loopTime = true;
            //clipAnimation.lockRootRotation = true;
            //clipAnimation.keepOriginalOrientation = true;
            //clipAnimation.lockRootHeightY = true;
            //clipAnimation.keepOriginalPositionY = true;
            //clipAnimation.keepOriginalPositionXZ = true;
            //clipAnimation.curves = new ClipAnimationInfoCurve[2];
            //clipAnimation.maskType = ClipAnimationMaskType.CopyFromOther;

            for(int c = 0; c < modelImporter.clipAnimations.Length; c++)
            {
                //ModelImporterClipAnimation clipAnimation in modelImporter.defaultClipAnimations)
                ModelImporterClipAnimation clip = modelImporter.clipAnimations[c];
                clip.name = obj.name;
                clip.loopTime = true;
                clip.lockRootRotation = true;
                clip.keepOriginalOrientation = true;
                clip.lockRootHeightY = true;
                clip.keepOriginalPositionY = true;
                clip.keepOriginalPositionXZ = true;
                clip.curves = new ClipAnimationInfoCurve[2];
                clip.maskType = ClipAnimationMaskType.CopyFromOther;
             
            }

            modelImporter.animationType = ModelImporterAnimationType.Human;
            //modelImporter.defaultClipAnimations = defaultClipAnimations;
            //modelImporter.SaveAndReimport();
        }
    }
    [MenuItem("AoG Utilities/Animation/ApplyCustomClipSettingsNoLoop")]
    private static void ApplyCustomClipSettingsNoLoop()
    {

        for(int i = 0; i < Selection.objects.Length; ++i)
        {

            Object obj = Selection.objects[i];
            string szPath = AssetDatabase.GetAssetPath(obj);
            ModelImporter modelImporter = AssetImporter.GetAtPath(szPath) as ModelImporter;
            ModelImporterClipAnimation[] defaultClipAnimations = modelImporter.defaultClipAnimations;
            foreach(ModelImporterClipAnimation clipAnimation in defaultClipAnimations)
            {

                clipAnimation.name = obj.name;
                clipAnimation.lockRootRotation = true;
                clipAnimation.keepOriginalOrientation = true;
                clipAnimation.lockRootHeightY = true;
                clipAnimation.keepOriginalPositionY = true;
                clipAnimation.keepOriginalPositionXZ = true;
            }

            modelImporter.animationType = ModelImporterAnimationType.Human;
            modelImporter.clipAnimations = defaultClipAnimations;
            //modelImporter.SaveAndReimport();
        }
    }

    private static ModelImporterClipAnimation GetModelImporterClip(ModelImporter mi)
    {
        ModelImporterClipAnimation clip = null;
        if(mi.clipAnimations.Length == 0)
        {
            //if the animation was never manually changed and saved, we get here. Check defaultClipAnimations
            if(mi.defaultClipAnimations.Length > 0)
            {
                clip = mi.defaultClipAnimations[0];
            }
            else
            {
                Debug.LogError("GetModelImporterClip can't find clip information");
            }
        }
        else
        {
            clip = mi.clipAnimations[0];
        }
        return clip;
    }

    private ModelImporterClipAnimation[] SetupDefaultClips(TakeInfo[] importedTakeInfos)
    {
        ModelImporterClipAnimation[] clips = new ModelImporterClipAnimation[importedTakeInfos.Length];

        for(int i = 0; i < importedTakeInfos.Length; i++)
        {
            TakeInfo takeInfo = importedTakeInfos[i];

            ModelImporterClipAnimation mica = new ModelImporterClipAnimation();
            mica.name = MakeUniqueClipName(clips, takeInfo.defaultClipName, -1);

            mica.takeName = takeInfo.name;
            mica.firstFrame = (float)((int)Mathf.Round(takeInfo.bakeStartTime * takeInfo.sampleRate));
            mica.lastFrame = (float)((int)Mathf.Round(takeInfo.bakeStopTime * takeInfo.sampleRate));

            mica.maskType = ClipAnimationMaskType.CopyFromOther;
            mica.keepOriginalPositionY = true;

            clips[i] = mica;
        }
        return clips;
    }

    private string MakeUniqueClipName(ModelImporterClipAnimation[] clips, string name, int row)
    {
        string text = name;
        int num = 0;
        int i;
        do
        {
            for(i = 0; i < clips.Length; i++)
            {
                if(clips[i] != null && text == clips[i].name && row != i)
                {
                    text = name + num.ToString();
                    num++;
                    break;
                }
            }
        }
        while(i != clips.Length);
            return text;
    }
}
