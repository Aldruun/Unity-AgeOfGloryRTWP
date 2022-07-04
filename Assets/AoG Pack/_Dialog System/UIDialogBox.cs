using AoG.Core;
using Ink.Runtime;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class UIDialogBox
{
    //public Conversation conversation;

    [SerializeField] private TextAsset inkJSONAsset = null;
    public Story story;
    private Transform _narrationContentPanel;
    private Transform _choicebox;
    private Transform _choiceholder;
    private GameObject _narrTextPrefab;
    private GameObject _dialogchoicePrefab;
    private Button _endDialogButton;

    private ScrollRect _scrollrectNarration;
    //public Transform dialogHolder;
    //public ActorInput dialogNPC;
    private string _dialogNPCName;
    private string _PCName;
    public static event Action<Story> OnCreateStory;
    public Action<bool> OnDialogBoxToggled;
    //public AudioClip sfx_Hide;

    //public AudioClip sfx_Show;

    //bool _isTalking;
    //float _speakTime;

    // Start is called before the first frame update

    internal Transform dialogboxPanel;

    public UIDialogBox(Transform dialogboxPanel)
    {
        this.dialogboxPanel = dialogboxPanel;
        _choicebox = this.dialogboxPanel.Find("Choicebox");
        
        _choiceholder = _choicebox.Find("Scroll View - Choices/Viewport/Content");
        _narrTextPrefab = Resources.Load<GameObject>("Prefabs/Dialog - Speaker");
        _scrollrectNarration = dialogboxPanel.Find("Scroll View - Narration").GetComponent<ScrollRect>();
        
        _dialogchoicePrefab = ResourceManager.prefab_dialogchoice;
        
        _endDialogButton = dialogboxPanel.Find("End Dialog Button").GetComponent<Button>();
        _endDialogButton.onClick.RemoveAllListeners();
        _endDialogButton.onClick.AddListener(delegate {
            GameEventSystem.RequestCloseDialogueBox?.Invoke();
        });
        _endDialogButton.gameObject.SetActive(false);
        //_dialogNPCName = this.dialogboxPanel.Find("Name - Speaker/Text - NPC Name").GetComponent<Text>();
        _narrationContentPanel = this.dialogboxPanel.Find("Scroll View - Narration/Viewport/Content");
        //_npcText = _narrationContentPanel.Find("Dialog - Speaker").GetComponent<Text>();
        //_npcText.gameObject.SetActive(false);
    }

    // This is the main function called every time the story changes. It does a few things:
    // Destroys all the old content and choices.
    // Continues over all the lines of text, then displays all the choices. If there are no choices, the story is finished!
    private void RefreshView(bool isSpeaker)
    {
        // Remove all the UI on screen
        RemoveChildren();

        // Read all the content until we can't continue any more
        while(story.canContinue)
        {
            // Continue gets the next line of the story
            string text = story.Continue();
            // This removes any white space from the text.
            text = (isSpeaker ? "<color=#eba675>" + _dialogNPCName + ":</color> " : "<color=#a6cf7e>" + _PCName + ":</color> ") + text.Trim();
            // Display the text on screen!
            CreateContentView(text);
            _scrollrectNarration.verticalNormalizedPosition = 0;
            _scrollrectNarration.StartCoroutine(CR_ScrollToBottom());
        }

        // Display all the choices, if there are any!
        if(story.currentChoices.Count > 0)
        {
            for(int i = 0; i < story.currentChoices.Count; i++)
            {
                Choice choice = story.currentChoices[i];
                Button button = CreateChoiceView(choice.text.Trim());
                // Tell the button what to do when we press it
                button.onClick.AddListener(delegate {
                    OnClickChoiceButton(choice);
                });
            }
        }
        // If we've read all the content and there's no choices, the story is finished!
        else
        {
            //Button choice = CreateChoiceView("[DIALOG BEENDEN]");
            //choice.
            _endDialogButton.gameObject.SetActive(true);
        }
    }

    private IEnumerator CR_ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        _scrollrectNarration.verticalNormalizedPosition = 0;
    }

    // When we click the choice button, tell the story to choose that choice!
    private void OnClickChoiceButton(Choice choice)
    {
        story.ChooseChoiceIndex(choice.index);

        RefreshView(false);
    }

    // Creates a textbox showing the the line of text
    private void CreateContentView(string text)
    {
        Text storyText = Object.Instantiate(_narrTextPrefab).GetComponent<Text>();
        storyText.text = text;
        storyText.transform.SetParent(_narrationContentPanel, false);
     
    }

    // Creates a button showing the choice text
    private Button CreateChoiceView(string text)
    {
        // Creates the button from a prefab
        Button choice = Object.Instantiate(_dialogchoicePrefab).GetComponent<Button>();
        choice.transform.SetParent(_choiceholder, false);

        // Gets the text from the button prefab
        Text choiceText = choice.GetComponentInChildren<Text>();
        choiceText.text = text;

        // Make the button expand to fit the text
        //HorizontalLayoutGroup layoutGroup = choice.GetComponent<HorizontalLayoutGroup>();
        //layoutGroup.childForceExpandHeight = false;

        return choice;
    }

    // Destroys all the children of this gameobject (all the UI)
    private void RemoveChildren()
    {
        //int childCount = _narrationContentPanel.childCount;
        //for(int i = childCount - 1; i >= 0; --i)
        //{
        //    GameObject.Destroy(_narrationContentPanel.GetChild(i).gameObject);
        //}
        int childCount2 = _choiceholder.childCount;
        for(int i = childCount2 - 1; i >= 0; --i)
        {
            GameObject.Destroy(_choiceholder.GetChild(i).gameObject);
        }
    }

    public void Setup(ActorInput talkingPC, ConversationData data)
    {
        _PCName = talkingPC.GetName();
        _dialogNPCName = data.owner.GetName();
        inkJSONAsset = data.inkdlgJSON;
        story = new Story(inkJSONAsset.text);
        OnCreateStory?.Invoke(story);
        RefreshView(true);
        _endDialogButton.gameObject.SetActive(false);
        //if (sfx_Show != null)
        //    UISoundManager.PlaySound(sfx_Show);

        //ConversationNode newNode = conversation.nodes.Where(n => n.ID == conversation.owner.currentDialogNodeID).FirstOrDefault();
        //NextPage(-1, conversation);


        //dialogNPC.transform.LookAt(PlayerController.Instance.transform);
        //PlayerController.Instance.transform.transform.LookAt(dialogNPC.transform);
        //ThirdPersonCamera2.Instance.enabled = false;        
    }

    //void NextPage(Choice choice)
    //{
    //    //Debug.Log("Turning page");
    //    var newNode = choice.nextNode;

    //    foreach(var c in newNode.choices)
    //        c.OnChoiceClicked = () => NextPage(c);

    //    AddDialog(newNode.npcDialog);
    //    AddChoices(newNode.choices);
    //}

    //public void Show(Conversation conversation, int stage)
    //{
    //    dialogNPC = conversation.owner;
    //    dialogNPCName.text = dialogNPC.ActorRecord.Name;
    //    //Conversation convo = ScriptableObject.Instantiate(conversation);


    //    if(sfx_Show != null)
    //        UISoundManager.PlaySound(sfx_Show);

    //    canvasToHide.enabled = false;

    //    Time.timeScale = 0.000000001f;
    //    //OnDialogBoxToggled?.Invoke(true);
    //    gameObject.SetActive(true);

    //    var newNode = conversation.conversationNode;

    //    foreach(var choice in newNode.choices)
    //        choice.OnChoiceClicked = () => NextPage(choice);

    //    AddDialog(newNode.npcDialog);
    //    AddChoices(newNode.choices);
    //}
    public void Hide()
    {


        //if (sfx_Hide != null)
        //    UISoundManager.PlaySound(sfx_Hide);

        //UIHandler.current.HideDialogBox();
    }

    public void InvokeChoiceEffect(ChoiceEffect effect)
    {
        //Debug.Log("Invoking choice effect: " + effect.ToString());
        switch(effect)
        {
            case ChoiceEffect.None:
                break;
            case ChoiceEffect.MakeCompanion:
                //dialogNPC.isFollower = true;
                break;
            case ChoiceEffect.EndDialog:
                Hide();
                break;
        }
    }

}