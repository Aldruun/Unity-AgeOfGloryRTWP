using System;
using System.Collections.Generic;
using UnityEngine;

public class Conversation : ScriptableObject
{
    public Actor owner;
    public List<ConversationNode> nodes;

    public ConversationNode LoadNode(int ID)
    {
        foreach(ConversationNode node in nodes)
        {
            if(node.ID == ID)
            {
                return node;
            }
        }

        return null;
    }
}

[Serializable]
public class ConversationNode
{
    //[SerializeField]
    public int ID;
    public bool endDialog;
    [Header("Only used if there are no choices")]
    public int moveToID;
    [TextArea]
    public string dialog;

    //public void LoadNextNode(int ID)
    //{

    //}

    public ConvoChoice[] choices;
}

[Serializable]
public class ConvoChoice
{
    public int newStage;
    //[EnumAttr(typeof(ChoiceEffect))]
    public bool endDialog;
    [NonSerialized] public ConversationNode nextNode;
    public int nextNodeID;
    public Action OnChoiceClicked;

    [TextArea]
    public string text;

    public ChoiceEffect[] effects;
}

public enum ChoiceEffect
{
    None,
    MakeCompanion,
    EndDialog
}


[System.Serializable]
public class ConversationData
{
    public Actor owner;
    public int currentStage;
    public TextAsset inkdlgJSON;

    public ConversationData(Actor owner, TextAsset inkdlgJSON, int currentStage)
    {
        this.owner = owner;
        this.inkdlgJSON = inkdlgJSON;
        this.currentStage = currentStage;
    }
}

//public class Conversation : ScriptableObject
//{
//    public ConversationNode conversationNode;

//    public AgentMonoController owner;
//}

//[Serializable]
//public class ConversationNode
//{
//    public Choice[] choices;

//    public int link;

//    /*[TextArea]*/ public string npcDialog;
//}

//[Serializable]
//public class Choice
//{
//    [EnumAttr(typeof(ChoiceEffect))]
//    public ChoiceEffect[] effects;

//    public ConversationNode nextNode;
//    public int nodeLink;
//    public Action OnChoiceClicked;

//    [TextArea]
//    public string text;
//}