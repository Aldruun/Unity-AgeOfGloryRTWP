using System;
using System.Collections.Generic;
using UnityEngine;

public class Quest : ScriptableObject
{
    public static Action<Quest, ActorInput> OnQuestTaken;
    private Stack<QuestStage> _stages;

    [TextArea] public string description;

    public int goldReward;
    public string label;
    public int maxParticipants = 3;
    public int minLevel;
    public Action<Quest> OnQuestCompleted;

    public QuestStage preStage; // Usually a walk to the quest board 
    public QuestStage[] stages;

    public int ID { get; set; }
    //public AgentMonoController leader { get; set; }

    public QuestBoard questBoard { get; set; }
    public QuestStage currentStage { get; private set; }

    public bool running { get; private set; }
    public bool taken { get; private set; }

    private void OnEnable()
    {
        _stages = new Stack<QuestStage>();

        for(int i = 0; i < stages.Length; i++)
            _stages.Push(Instantiate(stages[i]));
    }

    public bool Complete( /*AgentMonoController agent*/)
    {
        if(taken == false) // Wait until hero took the quest from the questboard (QuestController.cs)
            return false;

        if(currentStage == null)
        {
            currentStage = _stages.Pop();
            currentStage.Init();
        }

        if(currentStage.Complete())
        {
            if(_stages.Count == 0)
            {
                Debug.Log("<color=white>All quest stages complete</color>");
                OnQuestCompleted?.Invoke(this);
                return true;
            }

            Debug.Log("<color=white>Next stage</color>");
            currentStage = _stages.Pop();
            currentStage.Init();
        }

        return false;
    }

    internal void AddQuestPartyMember(ActorInput agent)
    {
        //_questParty.targetPosition = currentStage.GetLocation();
    }

    public void TakeQuestFromQuestBoard(ActorInput agent)
    {
        OnQuestTaken?.Invoke(this, agent);
        taken = true;
    }

    //public bool IsMember(AgentMonoController agent)
    //{
    //    return questParty.IsMember(agent);
    //}
}
