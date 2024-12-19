using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

using static Dialogue.LogicalLines.LogicalLineUtilities.Encapsulation;

namespace Dialogue.LogicalLines
{
    public class LL_Choice : ILogicalLine
    {
        public string keyword => "choice";
        private const char choice_identifier = '-';

        public IEnumerator Execute(Dialogue_Line line)
        {
            var currentConversation = DialogueSystem.instance.conversationManager.conversation;
            var progress = DialogueSystem.instance.conversationManager.conversationProgress;
            EncapsulatedData data = RipEncapsulationData(currentConversation, progress, ripHeaderandEncapsulators: true, parentStartingIndex: currentConversation.fileStartIndex);
            List<Choice> choices = GetChoicesFromData(data);
        
            string title = line.dialogue.rawData;
            ChoicePanel panel = ChoicePanel.instance;
            string[] choiceTitles = choices.Select(c => c.title).ToArray();

            panel.Show(title, choiceTitles);

            while (panel.isWaitingOnUserChoice)
                yield return null;

            Choice selectedChoice = choices[panel.lastDecision.answerIndex];

            Conversation newConversation = new Conversation(selectedChoice.resultLines, file: currentConversation.file, fileStartIndex: selectedChoice.startIndex, fileEndIndex: selectedChoice.endIndex);
            DialogueSystem.instance.conversationManager.conversation.SetProgress(data.endingIndex - currentConversation.fileStartIndex);
            DialogueSystem.instance.conversationManager.EnqueuePriority(newConversation);

            AutoReader autoReader = DialogueSystem.instance.autoReader;
            if (autoReader != null && autoReader.isOn && autoReader.skip)
            {
                if (VN_Configuration.activeConfig != null && !VN_Configuration.activeConfig.continueSkippingAfterChoice)
                    autoReader.Disable();
            }
        }

        public bool Matches(Dialogue_Line line)
        {
            return (line.hasSpeaker && line.speakerData.name.ToLower() == keyword);
        }

        private List<Choice> GetChoicesFromData(EncapsulatedData data)
        {
            List<Choice> choices = new List<Choice>();
            int encapsulateDepth = 0;
            bool isFirstChoice = true;

            Choice choice = new Choice
            {
                title = string.Empty,
                resultLines = new List<string>(),
            };

            int choiceIndex = 0, i = 0;

            //foreach (var line in data.lines.Skip(1))
            for (i = 1; i < data.lines.Count; i++)
            {
                var line = data.lines[i];
                if (IsChoiceStart(line) && encapsulateDepth == 1)
                {
                    if (!isFirstChoice)
                    {
                        choice.startIndex = data.startingIndex + (choiceIndex + 1);
                        choice.endIndex = data.startingIndex + (i - 1);
                        choices.Add(choice);
                        choice = new Choice
                        {
                            title = string.Empty,
                            resultLines = new List<string>(),
                        };
                    }

                    choiceIndex = i;
                    choice.title = line.Trim().Substring(1);
                    isFirstChoice = false;
                    continue;
                }

                AddLineToResults(line, ref choice, ref encapsulateDepth);
            }

            if (!choices.Contains(choice))
            {
                choice.startIndex = data.startingIndex + (choiceIndex + 1);
                choice.endIndex = data.startingIndex + (i - 2);
                choices.Add(choice);
            }

            return choices;
        }

        private void AddLineToResults(string line, ref Choice choice, ref int encapsulateDepth)
        {
            line.Trim();

            if (IsEncapsulationStart(line))
            {
                if (encapsulateDepth > 0)
                    choice.resultLines.Add(line);

                encapsulateDepth++;
                return;
            }

            if (IsEncapsulationEnd(line))
            {
                encapsulateDepth--;

                if (encapsulateDepth > 0)
                    choice.resultLines.Add(line);

                return;
            }

            choice.resultLines.Add(line);
        }

        private bool IsChoiceStart(string line) => line.Trim().StartsWith(choice_identifier);

        private struct Choice
        {
            public string title;
            public List<string> resultLines;
            public int startIndex;
            public int endIndex;
        }
    }
}