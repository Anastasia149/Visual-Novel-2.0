using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VisualNovel;

namespace Testing
{
    public class GameSavetesting : MonoBehaviour
    {
        public VNGameSave save;
        // Start is called before the first frame update
        void Start()
        {
            VNGameSave.activeFile = new VNGameSave();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                VNGameSave.activeFile.Save();
                    
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                try
                {
                    save = VNGameSave.Load($"{FilePaths.gameSaves}1{VNGameSave.file_type}", activateOnLoad: true);
                }
                catch(System.Exception e)
                {
                    Debug.LogError($"Do soemthing because we found an error. {e.ToString()}");
                }
            }
        }
    }
}