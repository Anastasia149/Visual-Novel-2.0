using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using Commands;
using Dialogue.LogicalLines;    // Input panel

namespace Dialogue
{
    public class ConversationManager
    {
        private DialogueSystem dialogueSystem => DialogueSystem.instance;

        private Coroutine process = null;
        public bool isRunning => process != null;
        public bool isOnLogicalLine { get; private set; } = false;

    // constructor for conversation characterManager
        public TextArchitect architect = null;
    // keypress
        private bool userPrompt = false;

        private LogicalLineManager logicalLineManager;

        private ConversationQueue conversationQueue;
        public int conversationProgress => (conversationQueue.IsEmpty() ? -1 : conversationQueue.top.GetProgress());
        public Conversation conversation => (conversationQueue.IsEmpty() ? null : conversationQueue.top);

        public bool allowUserPrompts = true;

        public ConversationManager(TextArchitect architect)
        {
            this.architect = architect;
            dialogueSystem.onUserPrompt_Next += OnUserPrompt_Next;

            logicalLineManager = new LogicalLineManager();
            conversationQueue = new ConversationQueue();
        }

        public Conversation[] GetConversationQueue() => conversationQueue.GetReadOnly();

        private void OnUserPrompt_Next()
        {
            if (allowUserPrompts)
                userPrompt = true;
        }

        #region Conversation Queue (choices)
        public void Enqueue (Conversation conversation) => conversationQueue.Enqueue(conversation);
        public void EnqueuePriority (Conversation conversation) => conversationQueue.EnqueuePriority(conversation);

        #endregion

        #region Conversation runner
        public Coroutine StartConversation(Conversation conversation)
        {
            StopConversation();
            conversationQueue.Clear();

            Enqueue(conversation);

            process = dialogueSystem.StartCoroutine(RunningConversation());

            return process;
        }


        public void StopConversation()
        {
            if (!isRunning)
                return;

            dialogueSystem.StopCoroutine(process);
            process = null;
        }

        IEnumerator RunningConversation()
        {
            while (!conversationQueue.IsEmpty())
            {
                Conversation currentConversation = conversation;

                if (currentConversation.HasReachedEnd())
                {
                    conversationQueue.Dequeue();
                    continue;
                }

                string rawLine = currentConversation.Currentlines();
                
                // don't show blank lines or try to run logic through them
                if (string.IsNullOrWhiteSpace(rawLine))
                {
                    TryAdvanceConversation(currentConversation);
                    continue;
                }

                Dialogue_Line line = DialogueParser.Parse(rawLine);

                if (logicalLineManager.TryGetLogic(line, out Coroutine logic))
                {
                    isOnLogicalLine = true;
                    yield return logic;
                }

                else
                {
                    // show dialogue
                    if (line.hasDialogue)
                        yield return Line_RunDialogue(line);

                    if (line.hasCommands)
                        yield return Line_RunCommands(line);


                    // wait for user input if dialogue was in this line
                    if (line.hasDialogue)
                    {
                        // wait for user input
                        yield return WaitForUserInput();

                        CommandManager.instance.StopAllProcesses();

                        dialogueSystem.onSystemPrompt_Clear();
                    }
                }

                TryAdvanceConversation(currentConversation);
                isOnLogicalLine = false;
            }

            process = null;

        }

        private void TryAdvanceConversation(Conversation conversation)
        {
            conversation.IncrementProgress();

            if (conversation != conversationQueue.top)
                return;

            if (conversation.HasReachedEnd())
                conversationQueue.Dequeue();
        }
        #endregion

        #region Dialogue architect
        IEnumerator Line_RunDialogue(Dialogue_Line line)
        {
            // Show or hide speaker name if there is one present
            if (line.hasSpeaker)
                HandleSpeakerLogic(line.speakerData);

            // if the dialogue box is not visible - make sure it becomes visible automatically
            if (!dialogueSystem.dialogueContainer.isVisible)
                dialogueSystem.dialogueContainer.Show();

            // Build Dialogue
            yield return BuildLineSegments(line.dialogue);
        }
        #endregion

        #region Command architect
        IEnumerator Line_RunCommands(Dialogue_Line line)
        {
            List<DL_CommandData.Command> commands = line.commandData.commands;

            foreach(DL_CommandData.Command command in commands)
            {
                if (command.waitForCompletion || command.name == "wait")
                {
                    CoroutineWrapper cw = CommandManager.instance.Execute(command.name, command.arguments);
                    while (!cw.isDone)
                    {
                        if(userPrompt)
                        {
                            CommandManager.instance.StopCurrentProcess();
                            userPrompt = false;
                        }
                        yield return null;
                    }
                }
                else
                    CommandManager.instance.Execute(command.name, command.arguments);
            }
            yield return null;
        }

        #endregion

        #region Line building / appending
        IEnumerator BuildLineSegments(DL_DialogueData line)
        {
            for (int i = 0; i < line.segments.Count; i++)
            {
                DL_DialogueData.Dialogue_Segment segment = line.segments[i];

                yield return WaitForDialogueSegmentSignalToBeTriggered(segment);

                yield return BuildDialogue(segment.dialogue, segment.appendText);
            }
        }
        // autoreader fixer for waiting
        public bool isWaitingOnAutoTimer { get; private set; } = false;

        IEnumerator WaitForDialogueSegmentSignalToBeTriggered(DL_DialogueData.Dialogue_Segment segment)
        {
            switch (segment.startSignal)
            {
                case DL_DialogueData.Dialogue_Segment.StartSignal.A:
                    yield return WaitForUserInput();
                    break;
                case DL_DialogueData.Dialogue_Segment.StartSignal.WA:
                    isWaitingOnAutoTimer = true;
                    yield return new WaitForSeconds(segment.signalDelay);
                    isWaitingOnAutoTimer = false;
                    break;
                default:
                    break;
            }
        }

        IEnumerator BuildDialogue(string dialogue, bool append = false)
        {
            dialogue = TagManager.Inject(dialogue);

            // build dialogue
            if (!append)
                architect.Build(dialogue);

            else
                architect.Append(dialogue);

            // wait for dialogue to complete
            while (architect.isBuilding)
            {
                if (userPrompt)
                {
                    if (!architect.hurryUp)
                        architect.hurryUp = true;
                    else
                        architect.ForceComplete();

                    userPrompt = false;
                }
                yield return null;
            }
        }
        #endregion

        #region Character spawning- Sprite / name handling
        private void HandleSpeakerLogic(DL_SpeakerData speakerData)
        {
            bool characterMustBeCreated = (speakerData.makeCharacterEnter || speakerData.isCastingPosition || speakerData.isCastingExpressions);

            Character character = CharacterManager.instance.GetCharacter(speakerData.name, createIfDoesNotExist: characterMustBeCreated);

            // Forces character to enter the screen
            if (speakerData.makeCharacterEnter && (!character.isVisible && !character.isRevealing))
                character.Show();

            // add character name to UI
            dialogueSystem.ShowSpeakerName(TagManager.Inject(speakerData.displayName));

            // custmize dialogue for this character- if applicable
            DialogueSystem.instance.ApplySpeakerDataToDialogueContainer(speakerData.name);

            // Cast position
            if (speakerData.isCastingPosition)
                character.MoveToPostion(speakerData.castPosition);

            // Cast expression
            if (speakerData.isCastingExpressions)
            {
                foreach (var ce in speakerData.CastExpressions)
                    character.OnRecieveCastingExpression(ce.layer, ce.expression);
            }
        }
        #endregion

        #region User input
        IEnumerator WaitForUserInput()
        {
            dialogueSystem.prompt.Show();

            while(!userPrompt)
                yield return null;

            dialogueSystem.prompt.Hide();
                
            userPrompt = false;
        }
        #endregion
    }
}