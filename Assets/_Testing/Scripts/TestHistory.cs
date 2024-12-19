using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using History;

namespace Testing
{
    public class TestHistory : MonoBehaviour
    {
        public HistoryState state = new HistoryState();

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
                state = HistoryState.Capture();

            if (Input.GetKeyDown(KeyCode.R))
                state.Load();
            
        }
    }
}