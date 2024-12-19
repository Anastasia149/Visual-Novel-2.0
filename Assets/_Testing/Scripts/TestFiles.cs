using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Testing
{
    public class TestFiles : MonoBehaviour
    {
        //private string fileName = "L_S";
        [SerializeField] private TextAsset fileName;//USE FOR TESTING PURPOSES !!

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Run());
        }

    // read file- every line found is logged
        IEnumerator Run()
        {
            List<string> lines = FileManager.ReadTextAsset(fileName, false);

            foreach (string line in lines)
            {
                Debug.Log(line);
            }
            yield return null;
        }
    }
}
