using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestMaster : MonoBehaviour
{
    public static QuestMaster Instance;
    private float _addNewQuestsTimer;
    private List<Quest> _availableQuests;
    private float _broadCastQuestInfoTimer;

    private int _questCounter;
    private QuestBoard[] questBoards;

    public bool QuestAvailable
    {
        get
        {
            for(int i = 0; i < questBoards.Length; i++)
                if(questBoards[i].availableQuests.Count > 0)
                    //Debug.Log("<color=white>Quests available</color>");
                    return true;

            return false;
        }
    }

    private void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
        _availableQuests = new List<Quest>();
        GameEventSystem.OnQuestStarted += q =>
        {
            Debug.Log("<color=white>Quest started</color>");
            GameEventSystem.OnQuestStarted?.Invoke(q);
        };

        questBoards = FindObjectsOfType<QuestBoard>();
    }

    private void Update()
    {
        if(_broadCastQuestInfoTimer > 0)
        {
            _broadCastQuestInfoTimer -= Time.deltaTime;
        }
        else
        {
            GameEventSystem.OnQuestReminder?.Invoke(_availableQuests);
            _broadCastQuestInfoTimer = 5;
        }


        for(int i = 0; i < _availableQuests.Count; i++)
            if(_availableQuests[i].taken && _availableQuests[i].Complete())
            {
                //Quest q = _availableQuests[i];
                //if(UnityEngine.Random.value > 0.5f)
                GameEventSystem.OnQuestRemoved?.Invoke(_availableQuests[i]);

                _availableQuests[i] = null;
                _availableQuests.RemoveAt(i);
                Debug.Log("<color=white>Ongoing quest complete</color>");
            }

        if(_addNewQuestsTimer > 0)
        {
            _addNewQuestsTimer -= Time.deltaTime;
        }
        else
        {
            _addNewQuestsTimer = 40;
            //questAvailable = true;
            Quest[] newQuests = ResourceManager.questRefs;
            _questCounter = 0;
            // Add quests to questboards
            for(int i = 0; i < questBoards.Length; i++)
            {
                //while(_questBoards[i]. == true)
                //{
                QuestBoard qb = questBoards[i];
                Debug.Log("<color=white>Adding quests</color>");
                while(qb.availableQuests.Count < qb.maxNumQuests)
                {
                    Debug.Log("<color=white>Added quest</color>");
                    Quest newQuest = Instantiate(newQuests[Random.Range(0, newQuests.Length)]);
                    //newQuest.OnQuestCompleted += (q) => { Debug.Log("<color=white>Quest complete</color>"); _availableQuests.Remove(q); q = null; };
                    newQuest.ID = _questCounter;
                    newQuest.questBoard = qb;
                    _questCounter++;
                    qb.AddQuest(newQuest);
                    _availableQuests.Add(newQuest);
                    GameEventSystem.OnQuestAdded?.Invoke(newQuest);
                }
            }
        }
    }

    internal Quest GetQuest(ActorInput agent)
    {
        return null; // _availableQuests.Where(q => q.CanJoin).OrderBy(q => q.goldReward).FirstOrDefault();
    }
}