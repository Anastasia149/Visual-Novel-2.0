using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;
using System.Data;

public class TestDialogueFiles : MonoBehaviour
{
    [SerializeField] private TextAsset fileToRead = null;
    void Start()
        {
            StartConversation();
        }

        void StartConversation()
        {
            List<string> lines = FileManager.ReadTextAsset("testFile 2");

            DialogueSystem.instance.Say(lines);
        }
}
