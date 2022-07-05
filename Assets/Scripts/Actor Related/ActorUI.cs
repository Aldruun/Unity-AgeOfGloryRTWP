using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightPlus;
using Object = UnityEngine.Object;

public class ActorUI
{
    private HighlightEffect highlightEffect;
    private ActorIndicator actorIndicator;
    private UIActorPortrait actorPortrait;
    private Gradient flashGradient;
    internal bool Selected { get; private set; }
    //private int partyIndex;
    private Actor actor;
    private AudioSource voiceAudioSource;

    public ActorUI(Actor actor, AudioSource voiceAudioSource, HighlightEffect highlightEffect)
    {
        this.actor = actor;
        this.voiceAudioSource = voiceAudioSource;
    
        this.highlightEffect = highlightEffect;
        Gradient instancedFlashGradient = AoG.Core.GameInterface.Instance.DatabaseService.GameSettings.ActorCircleFlashGradient;
        flashGradient = new Gradient();
        flashGradient.SetKeys(instancedFlashGradient.colorKeys, instancedFlashGradient.alphaKeys);
        actorIndicator = new ActorIndicator(actor, flashGradient);
    }

    //! Selecting & Highlighting ---------- ------------- ------------ --------------- ---------------- --------------
    private void HighlightIndicator(bool on)
    {
        actorIndicator.SetHighlighted(on);
    }

    public void Highlight()
    {
        HighlightIndicator(true);
        actorPortrait?.SetHighlighted(true);
    }

    public void Unhighlight()
    {
        HighlightIndicator(false);
        actorPortrait?.SetHighlighted(false);

        if(Selected)
        {
            actorIndicator.SetSelected(true);
            if(actorPortrait != null)
                actorPortrait.Active = true;
        }
        else
        {
            actorIndicator.SetSelected(false);
            if(actorPortrait != null)
                actorPortrait.Active = false;
        }
    }

    public void Update()
    {
        bool talking = voiceAudioSource != null && voiceAudioSource.priority == 127 && voiceAudioSource.isPlaying;
        actorIndicator.UpdateColor(talking);
        actorPortrait?.UpdateColor(actor.IsPlayer, talking);
    }

    //public void SetTargetReticle(bool on)
    //{
    //    InternalHandler_OnSetTargetReticle?.Invoke(on);
    //}

    public void Select()
    {
        actorIndicator.SetSelected(true);
        if(actorPortrait != null)
        {
            actorPortrait.Active = true;
            //GameEventSystem.OnPartyMemberSelected?.Invoke(actor);
        }
        //if (_attackTarget != null && _attackTarget.healthDepleted == false)
        //    _attackTarget.SetTargetReticle(true);
        //InternalHandler_OnSelected?.Invoke(true);
        Selected = true;
    }

    public void Deselect()
    {
        actorIndicator.SetSelected(false);
        if(actorPortrait != null)
        {
            actorPortrait.Active = false; 
            //GameEventSystem.OnPartyMemberDeselected?.Invoke(actor);
        }
        //if(_attackTarget != null && _attackTarget.healthDepleted == false)
        //    _attackTarget.SetTargetReticle(false);
        //InternalHandler_OnSelected?.Invoke(true);
        Selected = false;
    }

    //############################
    //# HELPER FUNCTIONS
    //############################
    internal void SetPortrait(UIActorPortrait newPortrait, Sprite sprite)
    {
        //Debug.Log("Setting portrait in ActorUI");
        Debug.Assert(newPortrait != null, "new portrait null");
        actorPortrait = newPortrait;
        actorPortrait.OnPortraitEnterCallback = HighlightIndicator;
        actorPortrait.SetImage(sprite, flashGradient);
    }

    internal UIActorPortrait GetPortrait()
    {
        return actorPortrait;
    }

    internal void Flash(float duration)
    {
        actor.StartCoroutine(CR_Flash(duration));
    }

    private IEnumerator CR_Flash(float duration)
    {
        highlightEffect.SetHighlighted(true);
        yield return new WaitForSeconds(duration);
        highlightEffect.SetHighlighted(false);
    }

    internal void ChangeRelationColor(ActorFlags actorFlags)
    {
        AlignmentColor alignmentColor = AlignmentColor.Neutral;
        if(actorFlags.HasFlag(ActorFlags.PC))
        {
            alignmentColor = AlignmentColor.PC;
        }
        else if(actorFlags.HasFlag(ActorFlags.ALLY))
        {
            alignmentColor = AlignmentColor.ImportantNPC;
        }
        else if(actorFlags.HasFlag(ActorFlags.NEUTRAL))
        {
            
        }
        else if(actorFlags.HasFlag(ActorFlags.HOSTILE))
        {
            alignmentColor = AlignmentColor.Hostile;
        }

        actorIndicator.ChangeRelationColor(alignmentColor);
        actorPortrait?.ChangeRelationColor(alignmentColor);
    }

    internal void ResetHighlighting()
    {
        actorIndicator.ResetHighlighting();
        actorPortrait?.ResetHighlighting();
    }

    public static void SetFlashGradientColor(Gradient gradient, Color color)
    {
        GradientColorKey[] colorKeys = gradient.colorKeys;

        for(int i = 1; i < colorKeys.Length - 1; i++)
        {
            colorKeys[i].color = color;
        }

        gradient.SetKeys(colorKeys, gradient.alphaKeys);
    }

    internal void Disable()
    {
        actorIndicator.Disable();
        actorPortrait.Disable();
        Clear();
    }

    internal void Clear()
    {
        GameEventSystem.RequestRemovePortrait(actor.PartySlot);
        actorPortrait = null;
        actorIndicator.Destroy();
        actorIndicator = null;
    }
}
