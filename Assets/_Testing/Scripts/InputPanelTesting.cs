using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Testing
{
    public class InputPanelTesting : MonoBehaviour
    {
        ChoicePanel panel;
        // Start is called before the first frame update
        void Start()
        {
            panel = ChoicePanel.instance;
            StartCoroutine(Running());
        }

        IEnumerator Running()
        {
            panel = ChoicePanel.instance;

            string[] choices = new string[]
            {
                "nuh uh",
                "yuh huh",
                "possibly",
                "no"
            };

            panel.Show("Your mom?", choices);

            while (panel.isWaitingOnUserChoice)
                yield return null;

            var decision = panel.lastDecision;

            Debug.Log($"Made choice {decision.answerIndex} '{decision.choices[decision.answerIndex]}'");
        }

    }
}