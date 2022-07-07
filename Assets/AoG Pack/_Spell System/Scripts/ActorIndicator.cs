using System;
using UnityEngine;

public class ActorIndicator
{
    internal bool isSpeaking;
    private readonly Transform indicatorMainPanel;
    private readonly Actor self;
    private readonly SpriteAnimationController _targetReticleAnimator;
    private readonly AudioSource voiceAudioSource;
    private readonly HighlightPlus.HighlightEffect circleOutline;
    private readonly Renderer circleRenderer;
    private readonly Renderer targetReticleRenderer;
    private Color defaultColor;
    private bool highlighted;
    private float lerp_time;
    private float pingpongTime;
    private Color selectedColor;
    private readonly Gradient flashGradient;
    private bool selected;

    public ActorIndicator(Actor self, Gradient flashGradient)
    {
        this.self = self;

        voiceAudioSource = self.transform.Find("Audio/AS Voice").GetComponent<AudioSource>();
        Debug.Assert(voiceAudioSource != null);
        indicatorMainPanel = UnityEngine.Object.Instantiate(Resources.Load<Transform>("Prefabs/GFX/actorindicator"), self.transform);

        circleOutline = indicatorMainPanel.Find("circle").GetComponent<HighlightPlus.HighlightEffect>();
        circleOutline.SetHighlighted(true);
        circleRenderer = circleOutline.GetComponent<Renderer>();
        targetReticleRenderer = indicatorMainPanel.Find("targetreticle").GetComponent<Renderer>();
        _targetReticleAnimator = targetReticleRenderer.GetComponent<SpriteAnimationController>();

        circleRenderer.enabled = true;
        targetReticleRenderer.enabled = false;

        this.flashGradient = flashGradient;
    }

    internal void ChangeRelationColor(AlignmentColor alignmentColor)
    {
        switch(alignmentColor)
        {
            case AlignmentColor.Neutral:
                defaultColor = selectedColor = Color.white;
                break;
            case AlignmentColor.ImportantNPC:
                defaultColor = selectedColor = Color.cyan;
                break;
            case AlignmentColor.PC:
                selectedColor = Color.green;
                defaultColor = selectedColor * 0.15f;
                break;
            case AlignmentColor.Hostile:
                defaultColor = selectedColor = Color.red;
                break;
        }

        lerp_time = 0;
        ActorUI.SetFlashGradientColor(flashGradient, selectedColor);
        circleOutline.outlineColor = highlighted ? selectedColor : defaultColor;
    }

    internal void ResetHighlighting()
    {
        lerp_time = 0;
    }

    internal void SetHighlighted(bool on)
    {
        highlighted = on;
    }

    internal void SetSelected(bool on)
    {
        selected = on;

        circleOutline.outline = 1;
        lerp_time = 0;
        if(on)
        {
            if(isSpeaking == false)
            {
                circleOutline.outlineColor = selectedColor;
            }
        }
        else
        {
            if(isSpeaking == false)
            {
                circleOutline.outlineColor = defaultColor;
            }
        }
    }

    internal void UpdateColor(bool talking)
    {
        AnimateTalkCircle(talking);
        if(talking)
        {
            if(targetReticleRenderer.enabled)
            {
                SetTargetReticle(false);
            }

            pingpongTime += Time.unscaledDeltaTime;
            float curveValue = Mathf.Lerp(-0.1f, 1f, Mathf.PingPong(pingpongTime, 0.3f));
            circleRenderer.transform.localScale = Vector3.one * (1 + curveValue);
        }
        else if(targetReticleRenderer.enabled)
        {
            _targetReticleAnimator.UpdateFrames();
        }
        else if(highlighted)
        {
            if(lerp_time < 0.6f)
            {
                float value = Mathf.Lerp(0f, 1f, lerp_time / 0.6f);
                lerp_time += Time.unscaledDeltaTime;
                Color color = flashGradient.Evaluate(value);
                circleOutline.outline = color.a;
                circleOutline.outlineColor = color;
            }
            else
            {
                lerp_time = 0f;
            }
        }
    }

    /// <summary>
    /// Enables indicator circle scaling and coloring based on the old Black Isle RPGs.
    /// </summary>
    /// <param name="on"></param>
    private void AnimateTalkCircle(bool on)
    {
        if(isSpeaking == on)
        {
            return;
        }

        isSpeaking = on;

        if(on)
        {
            circleOutline.outlineColor = Color.white;
        }
        else
        {
            circleOutline.outlineColor = selected ? selectedColor : defaultColor;
            circleRenderer.transform.localScale = Vector3.one;
            pingpongTime = 0.5f;
        }
    }

    /// <summary>
    /// Swap the default circle beneath the actor with the reticle that is used to show by which PC it is targeted.
    /// </summary>
    /// <param name="on"></param>
    private void SetTargetReticle(bool on)
    {
        circleRenderer.enabled = on == false;
        targetReticleRenderer.enabled = on;
        _targetReticleAnimator.Stop(0);

        if((self.ActorStats.GetEnemyFlags() & ActorFlags.PC) != 0) // PC is enemy, so tint the reticle red
        {
            targetReticleRenderer.material.color = Color.red;
        }
        else
        {
            targetReticleRenderer.material.color = Color.green;
        }
    }

    internal void OverrideAlignmentColor(Color color)
    {
        ActorUI.SetFlashGradientColor(flashGradient, color);
    }

    internal void RevertToAlignmentColor()
    {
        ActorUI.SetFlashGradientColor(flashGradient, selectedColor);
    }

    internal void Disable()
    {
        circleOutline.ignore = true;
        circleRenderer.enabled = false;
        SetTargetReticle(false);
    }

    internal void Destroy()
    {
        UnityEngine.Object.Destroy(indicatorMainPanel.gameObject);
    }

}