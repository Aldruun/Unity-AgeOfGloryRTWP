//using AtmosphericHeightFog;
using UnityEngine;

//[ExecuteInEditMode]
public class GameTimeManager : MonoBehaviour
{
    public bool updateSunAndLighting;
    public bool updateFog;
    public Light sun;
    public Gradient ambienceGradient;
    public AnimationCurve lightIntensityCurve;
    public AnimationCurve skyboxBlendCurve;
    [Range(0, 1)] public float currentNTime;
    private static int _currTimeInHours;
    [Range(10, 86400)] public int dayCycleInSeconds = 10;

    public bool onValidateUpdate;

    //public HeightFogGlobal fog;
    private Clock _clock;

    private void Start()
    {        
        _clock = new Clock();
    }

    private void Update()
    {
        UpdateTimeComponents();
        _clock.Update(this);
    }

    public void UpdateTimeComponents()
    {
        if (updateSunAndLighting)
            UpdateSun();

        //if(updateFog)
        //{
        //    fog.timeOfDay = currentNTime;
        //}

        UpdateTime();
    }

    private void UpdateTime()
    {
        currentNTime += Time.deltaTime / dayCycleInSeconds;
       
        if (currentNTime >= 1.0f)
            currentNTime -= 1.0f;

        _currTimeInHours = (int)(currentNTime * 24);
    }

    private void UpdateSun()
    {
        sun.transform.localRotation = Quaternion.Euler(currentNTime * 360f - 90, 170, 0);
        //skyCameraObject.localRotation = Quaternion.Euler(0, 170, (currentNTime * 360f) - 90);
        RenderSettings.ambientLight = ambienceGradient.Evaluate(currentNTime);
        sun.intensity = lightIntensityCurve.Evaluate(sun.transform.rotation.eulerAngles.x / 180f);

        //RenderSettings.skybox.SetFloat("_Rotation", (currentNTime * 360f) - 90);
        //daySkybox.material.SetFloat("_Exposure", 1 - skyboxBlendCurve.Evaluate(currentNTime));
        //nightSkybox.material.SetFloat("_Exposure", skyboxBlendCurve.Evaluate(currentNTime));
        RenderSettings.skybox.SetFloat("_Rotation", currentNTime * 360f - 90);
        RenderSettings.skybox.SetFloat("_Blend", skyboxBlendCurve.Evaluate(currentNTime));
        //DynamicGI.UpdateEnvironment();
    }

    public static int GetGameTimeInHours()
    {
        return _currTimeInHours;
    }

    //private void OnValidate()
    //{
    //    if (onValidateUpdate)
    //        UpdateTimeComponents();
    //}

    //private void OnGUI()
    //{
    //    //GUI.Label(new Rect(10, 10, 300, 300),
    //    //    "clock: " + _clock.currentHour.ToString("F0") + " : " + _clock.currentMinute.ToString("F0"));
    //}
}

//[ExecuteInEditMode]

public class Clock
{
    public float currentHour;
    public float currentMinute;

    //public Transform hourHand;
    //public Transform minuteHand;

    private float hoursToDegrees = 360f / 12f;
    private float minutesToDegrees = 360f / 60f;

    public void Update(GameTimeManager gtm)
    {
        currentHour = 24 * gtm.currentNTime;
        currentMinute = 60 * (currentHour - Mathf.Floor(currentHour));

        //hourHand.localRotation = Quaternion.Euler(currentHour * hoursToDegrees, 0, 0);
        //minuteHand.localRotation = Quaternion.Euler(currentMinute * minutesToDegrees, 0, 0);
    }
}