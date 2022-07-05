using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestBoard : Interactable
{
    public int maxNumQuests = 3;
    public List<Quest> availableQuests { get; private set; }
    public Action<Quest> OnQuestAdded { get; private set; }

    public int QuestCount => availableQuests.Count;

    private void Awake()
    {
        if (maxNumQuests <= 0) Debug.LogError("Invalid value -> maxNumQuests");

        availableQuests = new List<Quest>();
    }

    public bool AddQuest(Quest quest)
    {
        if (QuestCount >= maxNumQuests) return false;

        availableQuests.Add(quest);
        availableQuests = availableQuests.OrderBy(q => q.goldReward).ToList();
        quest.OnQuestCompleted += q => { availableQuests.Remove(q); };
        OnQuestAdded?.Invoke(quest);

        return true;
    }

    public Quest GetQuest(Actor agent)
    {
        return availableQuests.FirstOrDefault();
    }

    public override void Interact(Actor agent)
    {
        if (agent.IsPlayer)
        {
            Debug.Log("<color=orange>Player interacted</color>");
            GameEventSystem.OnPlayerInteraction_Questboard?.Invoke(agent, this);
        }
    }
}