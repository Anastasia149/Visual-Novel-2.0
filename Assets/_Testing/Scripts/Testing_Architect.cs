using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

namespace Testing
{
   /* public class Testing_Architect : MonoBehaviour
    {
        DialogueSystem ds;
        TextArchitect architect;

        string[] lines = new string[5]
        {
            "this is a random line of dialogue",
            "i want to say something, come over here",
            "the world is a crazy place sometimes.",
            "don't lose hope, things will get better!",
            "nae nae frogge"
        };

        // Start is called before the first frame update
        void Start()
        {
            ds = DialogueSystem.instance;
            architect = new TextArchitect(ds.dialogueContainer.dialogueText);
            architect.buildMethod = TextArchitect.BuildMethod.typewriter;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                architect.Build(lines[Random.Range(0, lines.Length)]);
            }
        }
    }*/
}