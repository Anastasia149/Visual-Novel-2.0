using History;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Dialogue
{
    public class PlayerInputManager : MonoBehaviour
    {
        private PlayerInput input;
        private List<(InputAction action, Action<InputAction.CallbackContext> command)> actions = new List<(InputAction action, Action<InputAction.CallbackContext> command)> ();

        private void Awake()
        {
            input = GetComponent<PlayerInput> ();

            InitializeActions();
        }

        private void InitializeActions()
        {
            actions.Add((input.actions["Next"], OnNext));
            actions.Add((input.actions["HistoryBack"], OnHistoryBack));
            actions.Add((input.actions["HistoryForward"], OnHistoryForward));
        }

        private void OnEnable()
        {
            foreach (var inputAction in actions)
                inputAction.action.performed += inputAction.command;
        }

        // if input system is disabled for whatever reason, use this failsafe
        private void OnDisable()
        {
            foreach (var inputAction in actions)
                inputAction.action.performed -= inputAction.command;
        }

        public void OnNext(InputAction.CallbackContext c)
        {
            DialogueSystem.instance.OnUserPrompt_Next();
        }

        // useless but keep for now

        public void OnHistoryBack(InputAction.CallbackContext c)
        {
            HistoryManager.instance.GoBack();
        }

        public void OnHistoryForward(InputAction.CallbackContext c)
        {
            HistoryManager.instance.GoForward();
        }
    }
}