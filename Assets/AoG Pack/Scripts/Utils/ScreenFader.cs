using System;
using UnityEngine;

public class ScreenFader : MonoBehaviour
{
    private static ScreenFader mInstance;
    public float fadeDelay;
    public float fadeDuration = 2;

    public GUIStyle m_BackgroundStyle = new GUIStyle(); // Style for background tiling

    public Color
        m_CurrentScreenOverlayColor = new Color(0, 0, 0, 0); // default starting color: black and fully transparrent

    public Color
        m_DeltaColor =
            new Color(0, 0, 0,
                0); // the delta-color is basically the "speed / second" at which the current color should change

    public float m_FadeDelay;
    public int m_FadeGUIDepth = -1000; // make sure this texture is drawn on top of everything
    public Texture2D m_FadeTexture; // 1x1 pixel texture used for fading
    public Action m_OnFadeFinish;

    public Color
        m_TargetScreenOverlayColor = new Color(0, 0, 0, 0); // default target color: black and fully transparrent

    private static ScreenFader instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = FindObjectOfType(typeof(ScreenFader)) as ScreenFader;

                if (mInstance == null) mInstance = new GameObject("CameraFaderX").AddComponent<ScreenFader>();
            }

            return mInstance;
        }
    }

    private void Awake()
    {
        if (mInstance == null)
        {
            mInstance = this;
            instance.init();
        }
    }

    // Initialize the texture, background-style and initial color:
    public void init()
    {
        instance.m_FadeTexture = new Texture2D(1, 1);
        instance.m_BackgroundStyle.normal.background = instance.m_FadeTexture;
    }

    // Draw the texture and perform the fade:
    private void OnGUI()
    {
        // If delay is over...
        if (Time.time > instance.m_FadeDelay)
            // If the current color of the screen is not equal to the desired color: keep fading!
            if (instance.m_CurrentScreenOverlayColor != instance.m_TargetScreenOverlayColor)
            {
                // If the difference between the current alpha and the desired alpha is smaller than delta-alpha * deltaTime, then we're pretty much done fading:
                if (Mathf.Abs(instance.m_CurrentScreenOverlayColor.a - instance.m_TargetScreenOverlayColor.a) <
                    Mathf.Abs(instance.m_DeltaColor.a) * Time.deltaTime)
                {
                    instance.m_CurrentScreenOverlayColor = instance.m_TargetScreenOverlayColor;
                    SetScreenOverlayColor(instance.m_CurrentScreenOverlayColor);
                    instance.m_DeltaColor = new Color(0, 0, 0, 0);

                    if (instance.m_OnFadeFinish != null)
                        instance.m_OnFadeFinish();

                    Die();
                }
                else
                {
                    // Fade!
                    SetScreenOverlayColor(instance.m_CurrentScreenOverlayColor +
                                          instance.m_DeltaColor * Time.deltaTime);
                }
            }

        // Only draw the texture when the alpha value is greater than 0:
        if (m_CurrentScreenOverlayColor.a > 0)
        {
            GUI.depth = instance.m_FadeGUIDepth;
            GUI.Label(new Rect(-10, -10, Screen.width + 10, Screen.height + 10), instance.m_FadeTexture,
                instance.m_BackgroundStyle);
        }
    }

    private static void SetScreenOverlayColor(Color newScreenOverlayColor)
    {
        instance.m_CurrentScreenOverlayColor = newScreenOverlayColor;
        instance.m_FadeTexture.SetPixel(0, 0, instance.m_CurrentScreenOverlayColor);
        instance.m_FadeTexture.Apply();
    }

    public static void StartAlphaFade(Color newScreenOverlayColor, bool isFadeIn, float fadeDuration)
    {
        if (fadeDuration <= 0.0f)
        {
            SetScreenOverlayColor(newScreenOverlayColor);
        }
        else
        {
            if (isFadeIn)
            {
                instance.m_TargetScreenOverlayColor = new Color(newScreenOverlayColor.r, newScreenOverlayColor.g,
                    newScreenOverlayColor.b, 0);
                SetScreenOverlayColor(newScreenOverlayColor);
            }
            else
            {
                instance.m_TargetScreenOverlayColor = newScreenOverlayColor;
                SetScreenOverlayColor(new Color(newScreenOverlayColor.r, newScreenOverlayColor.g,
                    newScreenOverlayColor.b, 0));
            }

            instance.m_DeltaColor = (instance.m_TargetScreenOverlayColor - instance.m_CurrentScreenOverlayColor) /
                                    fadeDuration;
        }
    }

    public static void StartAlphaFade(Color newScreenOverlayColor, bool isFadeIn, float fadeDuration, float fadeDelay)
    {
        if (fadeDuration <= 0.0f)
        {
            SetScreenOverlayColor(newScreenOverlayColor);
        }
        else
        {
            instance.m_FadeDelay = Time.time + fadeDelay;

            if (isFadeIn)
            {
                instance.m_TargetScreenOverlayColor = new Color(newScreenOverlayColor.r, newScreenOverlayColor.g,
                    newScreenOverlayColor.b, 0);
                SetScreenOverlayColor(newScreenOverlayColor);
            }
            else
            {
                instance.m_TargetScreenOverlayColor = newScreenOverlayColor;
                SetScreenOverlayColor(new Color(newScreenOverlayColor.r, newScreenOverlayColor.g,
                    newScreenOverlayColor.b, 0));
            }

            instance.m_DeltaColor = (instance.m_TargetScreenOverlayColor - instance.m_CurrentScreenOverlayColor) /
                                    fadeDuration;
        }
    }

    public static void StartAlphaFade(Color newScreenOverlayColor, bool isFadeIn, float fadeDuration, float fadeDelay,
        Action OnFadeFinish)
    {
        if (fadeDuration <= 0.0f)
        {
            SetScreenOverlayColor(newScreenOverlayColor);
        }
        else
        {
            instance.m_OnFadeFinish = OnFadeFinish;
            instance.m_FadeDelay = Time.time + fadeDelay;

            if (isFadeIn)
            {
                instance.m_TargetScreenOverlayColor = new Color(newScreenOverlayColor.r, newScreenOverlayColor.g,
                    newScreenOverlayColor.b, 0);
                SetScreenOverlayColor(newScreenOverlayColor);
            }
            else
            {
                instance.m_TargetScreenOverlayColor = newScreenOverlayColor;
                SetScreenOverlayColor(new Color(newScreenOverlayColor.r, newScreenOverlayColor.g,
                    newScreenOverlayColor.b, 0));
            }

            instance.m_DeltaColor = (instance.m_TargetScreenOverlayColor - instance.m_CurrentScreenOverlayColor) /
                                    fadeDuration;
        }
    }

    private void Die()
    {
        mInstance = null;
        Destroy(gameObject);
    }

    private void OnApplicationQuit()
    {
        mInstance = null;
    }
}