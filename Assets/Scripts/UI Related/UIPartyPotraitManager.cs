using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class UIPartyPotraitManager
{
    private Dictionary<int, UIActorPortrait> partymemberPortraits = new Dictionary<int, UIActorPortrait>();
    private Transform potraitHolder;

    public UIPartyPotraitManager(Transform portraitHolder)
    {
        this.potraitHolder = portraitHolder;

        //actorPortraitGameObjects = new Dictionary<GameObject, UIActorPortrait>()
        //    {
        //        { portraitHolder.GetChild(0).gameObject, null },
        //        { portraitHolder.GetChild(1).gameObject, null },
        //        { portraitHolder.GetChild(2).gameObject, null },
        //        { portraitHolder.GetChild(3).gameObject, null },
        //        { portraitHolder.GetChild(4).gameObject, null },
        //        { portraitHolder.GetChild(5).gameObject, null }
        //    };

        //foreach(KeyValuePair<GameObject, UIActorPortrait> kvp in actorPortraitGameObjects)
        //{
        //    kvp.Key.SetActive(false);
        //}
    }

    public UIActorPortrait CreatePortrait(int partyMemberIndex, Transform pcTransform)
    {
        Debug.Log("Creating portrait");

        if(partyMemberIndex < 1)
        { //!What? Why is that?

            Debug.LogError("Wrong party member index");
            return null;
        }

        if(ContainsIndex(partyMemberIndex))
        {

            Debug.LogError("Agent was already in squad");
            return null;
        }

        //Debug.Log("Adding agent to roster");
        GameObject portraitObj = Object.Instantiate(ResourceManager.prefab_actorportrait, potraitHolder);

        Button button = portraitObj.GetComponentInChildren<Button>();
        UIActorPortrait portrait = button.GetComponentInParent<UIActorPortrait>();

        //if(sprite != null)
        //{
        //    button.image.sprite = sprite;
        //}

        portrait.Init(partyMemberIndex, pcTransform);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => OnHeroPortraitClicked(portrait));

        partymemberPortraits.Add(partyMemberIndex, portrait);

        GameEventSystem.OnHeroPortraitAdded?.Invoke(partyMemberIndex, portraitObj.GetComponent<Image>());

        return portrait;
    }

    public void RemovePortrait(int partyMemberIndex)
    {
        UIActorPortrait portrait = partymemberPortraits[partyMemberIndex];
        GameObject portraitObj = portrait.gameObject;
        Object.Destroy(portraitObj);
        partymemberPortraits.Remove(partyMemberIndex);
    }

    public void HighlightPartymemberPortrait(int partyMemberIndex, bool on)
    {
        UIActorPortrait portrait = partymemberPortraits[partyMemberIndex];
        portrait.Active = on;
    }

    public bool ContainsIndex(int partyMemberIndex)
    {
        return partymemberPortraits.ContainsKey(partyMemberIndex);
    }

    private void OnHeroPortraitClicked(UIActorPortrait button)
    {
        if(GameEventSystem.isAimingSpell)
            return;

        //Hack Add to selection
        if(Input.GetKey(KeyCode.LeftShift))
        {
            button.Active = !button.Active;
        }
        else //Hack Deselect all other buttons
        {
            button.Active = true;

            for(int i = 0; i < partymemberPortraits.Count; i++)
            {
                UIActorPortrait b = partymemberPortraits[i + 1];

                if(b != button)
                    b.Active = false;
            }
        }

        GameEventSystem.OnHeroPortraitClicked?.Invoke(button.GetLinkedPartyIndex(), button.Active);
    }
}
