using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Dialogue
{
    public class AutoReader : MonoBehaviour
    {
        private const int default_characters_read_per_second = 18;
        private const float read_time_padding = 0.5f;
        private const float max_read_time = 99f;
        private const float min_read_time = 1f;
        private const string status_text_auto = "Auto";
        private const string status_text_skip = "Skipping";

        private ConversationManager conversationManager;
        private TextArchitect architect => conversationManager.architect;

        public bool skip { get; set; } = false;
        public float speed { get; set; } = 1f;

        public bool isOn => co_running != null;
        private Coroutine co_running = null;

        [SerializeField] private TextMeshProUGUI statusText;
        [HideInInspector] public bool allowToggle = true;

        public void Initialize(ConversationManager conversationManager)
        {
            this.conversationManager = conversationManager;

            statusText.text = string.Empty;
        }

        public void Enable()
        {
            if (isOn)
                return;

            co_running = StartCoroutine(AutoRead());
        }

        public void Disable()
        {
            if (!isOn) 
                return;

            StopCoroutine(co_running);
            skip = false;
            co_running = null;
            statusText.text = string.Empty;
        }

        private IEnumerator AutoRead()
        {
            // Do nothing if no conversation
            if (!conversationManager.isRunning)
            {
                Disable();
                yield break;
            }

            if (!architect.isBuilding && architect.currentText != string.Empty)
                DialogueSystem.instance.OnSystemPrompt_Next();

            while (conversationManager.isRunning)
            {
                // Read and wait
                if (!skip)
                {
                    while (!architect.isBuilding && !conversationManager.isWaitingOnAutoTimer)
                        yield return null;

                    float timeStarted = Time.time;

                    while (architect.isBuilding || conversationManager.isWaitingOnAutoTimer)
                        yield return null;

                    float timeToRead = Mathf.Clamp(((float) architect.tmpro.textInfo.characterCount / default_characters_read_per_second), min_read_time, max_read_time);
                    timeToRead = Mathf.Clamp((timeToRead - (Time.time - timeStarted)), min_read_time, max_read_time);
                    timeToRead = (timeToRead / speed) + read_time_padding;
                
                    yield return new WaitForSeconds(timeToRead);
                }

                // skip
                else
                {
                    architect.ForceComplete();
                    yield return new WaitForSeconds(0.05f);
                }

                DialogueSystem.instance.OnSystemPrompt_Next();
            }

            Disable();
        }

        public void Toggle_Auto()
        {
            if (!allowToggle)
                return;

            bool prevState = skip;
            skip = false;

            if (prevState)
                Enable();

            else
            {
                if (!isOn)
                    Enable();
                else
                    Disable();
            }

            if (isOn)
                statusText.text = status_text_auto;
        }

        public void Toggle_Skip()
        {
            if (!allowToggle)
                return;

            bool prevState = skip;
            skip = true;

            if (!prevState)
                Enable();

            else
            {
                if (!isOn)
                    Enable();
                else
                    Disable();
            }

            if (isOn)
                statusText.text = status_text_skip;
        }

    }
}