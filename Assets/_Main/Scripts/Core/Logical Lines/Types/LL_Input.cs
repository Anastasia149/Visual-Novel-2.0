using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue.LogicalLines
{
    public class LL_Input : ILogicalLine
    {
        public string keyword => "input";
        public IEnumerator Execute(Dialogue_Line line)
        {
            string title = line.dialogue.rawData;

            InputPanel panel = InputPanel.instance;
            panel.Show(title);

            while (panel.isWaitingOnUserInput)
                yield return null;
            
        }

        public bool Matches(Dialogue_Line line)
        {
            return (line.hasSpeaker && line.speakerData.name.ToLower() == keyword);
        }
    }
}