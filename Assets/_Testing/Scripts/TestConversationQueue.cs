using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

public class TestConversationQueue : MonoBehaviour
{

    private void Start()
    { 
        {
            StartCoroutine(Running());
        }
    }

    IEnumerator Running()
    {
        List<string> lines = new List<string>()
        {
            "this is line 1",
            "line 2",
            "line 3",
        };

        yield return DialogueSystem.instance.Say(lines);

        DialogueSystem.instance.Hide();
    }

    void Update()
    {
        List<string> lines = new List<string>();
        Conversation conversation = null;

        if (Input.GetKeyUp(KeyCode.Q)) 
        {
            lines = new List<string>()
            {
                "start of enqueued convo",
                "keep goin",
            };
            conversation = new Conversation(lines);
            DialogueSystem.instance.conversationManager.Enqueue(conversation);
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            lines = new List<string>()
            {
                "important convo",
                "guh"
            };
            conversation = new Conversation(lines);
            DialogueSystem.instance.conversationManager.EnqueuePriority(conversation);
        }
    }
}
