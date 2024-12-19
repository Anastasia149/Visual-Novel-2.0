using System;
using UnityEngine;

namespace Dialogue
{
    [System.Serializable]
    public class Map
    {
        public GameObject root;

        private CanvasGroupController cgController;

        private bool __initialized = false;
        public void Initialize()
        {
            if (__initialized)
                return;

            cgController = new CanvasGroupController(DialogueSystem.instance, root.GetComponent<CanvasGroup>());
        }

        public bool isVisible => cgController.isVisible;
        public Coroutine Show(float speed = 1f, bool immediate = false)
        {
            cgController.SetInteractableState(true);
            return cgController.Show(speed, immediate);

        }
        public Coroutine Hide(float speed = 1f, bool immediate = false)
        {
            cgController.SetInteractableState(false);
            return cgController.Hide(speed, immediate);
        }
    }
}