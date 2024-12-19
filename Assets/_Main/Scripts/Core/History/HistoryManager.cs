using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

namespace History
{
    [RequireComponent(typeof(HistoryNavigation))]

    public class HistoryManager : MonoBehaviour
    {
        public const int history_cache_limit = 15;
        public static HistoryManager instance { get; private set; }
        public List<HistoryState> history = new List<HistoryState>();

        private HistoryNavigation navigation;

        private void Awake()
        {
            instance = this;
            navigation = GetComponent<HistoryNavigation>();
        }

        void Start()
        {
            DialogueSystem.instance.onClear += LogCurrentState;
        }

        public void LogCurrentState()
        {
            HistoryState state = HistoryState.Capture();
            history.Add(state);

            if (history.Count > history_cache_limit)
                history.RemoveAt(0);
        }

        public void Loadstate(HistoryState state)
        {
            state.Load();
            
        }

        // Honestly won't need these- goes back and forth between history states
        public void GoForward() => navigation.GoForward();
        public void GoBack() => navigation.GoBack();
    }
}