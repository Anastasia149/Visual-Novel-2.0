using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

namespace Testing
{
    public class TestParsing : MonoBehaviour
    {
        [SerializeField] private TextAsset file;

        // Start is called before the first frame update
        void Start()
        {
            SendFileToParse();
        }

        
        void SendFileToParse()
        {
            List<string> lines = FileManager.ReadTextAsset("L_S");

            foreach(string line in lines)
            {
                if (line == string.Empty)
                    continue;

                Dialogue_Line dL = DialogueParser.Parse(line);
            }
        }
    }
}