using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using Commands;

namespace Dialogue
{
    public class Dialogue_Line
    {
        public string rawData { get; private set; } = string.Empty;
        public DL_SpeakerData speakerData;
        public DL_DialogueData dialogue;
        public DL_CommandData commandData;

        public bool hasDialogue => dialogue != null; // dialogue != string.Empty;

        public bool hasSpeaker => speakerData != null;

       public bool hasCommands => commandData != null;

        // default constructor
        public Dialogue_Line(string rawLine, string speaker, string dialogue, string commands)
        {
            rawData = rawLine;
            this.speakerData = (string.IsNullOrWhiteSpace(speaker) ? null : new DL_SpeakerData(speaker));
            this.dialogue = (string.IsNullOrWhiteSpace(dialogue) ? null : new DL_DialogueData(dialogue));
            this.commandData = (string.IsNullOrWhiteSpace(commands) ? null : new DL_CommandData(commands));
        }
    }
}