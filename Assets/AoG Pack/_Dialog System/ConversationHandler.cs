using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationHandler : MonoBehaviour
{
    private Transform _target;

    private void Awake()
    {
        //GameEventSystem.OnDialogueToggled -= StartConversationSequence;
        //GameEventSystem.OnDialogueToggled += StartConversationSequence;
    }

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    private void OnDisable()
    {
        //GameEventSystem.OnDialogueToggled -= StartConversationSequence;
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void StartConversationSequence(Conversation conversation)
    {

    }
}
