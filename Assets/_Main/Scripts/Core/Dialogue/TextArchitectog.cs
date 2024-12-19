/*using System.Collections;
using UnityEngine;
using TMPro;
using Characters;

namespace Dialogue
{
    public class TextArchitect
    {
        public TextMeshProUGUI tmpro_ui;
        public TextMeshPro tmpro_world;
    
        public TMP_Text tmpro => tmpro_ui != null ? tmpro_ui : tmpro_world;
    
        public string currentText => tmpro.text;
        public string targetText { get; private set; } = "";
        public string preText { get; private set; } = "";
        public string fullTargetText => preText + targetText;
    
    // adds typewriter effect to text
        public enum BuildMethod {typewriter}
        public BuildMethod buildMethod = BuildMethod.typewriter;
    
    // allows change of color if desired
        public Color textColor { get { return tmpro.color; } set {tmpro.color = value; } }
    
    // base speed of the text- one character per second
        public float speed { get { return baseSpeed * speedMultiplier; } set {speedMultiplier = value; } }
        private const float baseSpeed = 1;
        private float speedMultiplier = 1;
    
    // speeds up how fast the text moves on click- double click increases speed
        public int characterPerCycle { get { return speed <= 2f ? characterMultiplier : speed <= 2.5f ? characterMultiplier * 2 : characterMultiplier * 3; } }
        private int characterMultiplier = 1;
    
        public bool hurryUp = false;


    
        public TextArchitect(TextMeshProUGUI tmpro_ui)
        {
            this.tmpro_ui = tmpro_ui;
        }
    
        public TextArchitect(TextMeshPro tmpro_world)
        {
            this.tmpro_world = tmpro_world;
        }
    
    // begins to build the actual text
        public Coroutine Build(string text)
        {
            preText = "";
            targetText = text;
    
            Stop();
    
            buildProcess = tmpro.StartCoroutine(Building());
            return buildProcess;
        }

        /*public void SetText(string text)
        {
            preText = "";
            targetText = text;

            Stop();

            tmpro.text = targetText;
        }*/
    
    // append
  /*      public Coroutine Append(string text)
        {
            preText = tmpro.text;
            targetText = text;
    
            Stop();
    
            buildProcess = tmpro.StartCoroutine(Building());
            return buildProcess;
        }
    
    // handles text generation
        private Coroutine buildProcess = null;
        public bool isBuilding => buildProcess != null;
    
        public void Stop()
        {
            if (!isBuilding)
                return;
            tmpro.StopCoroutine(buildProcess);
            buildProcess = null;
        }
    
        IEnumerator Building()
        {
            Prepare();
    
            switch(buildMethod)
            {
                case BuildMethod.typewriter:
                    yield return Build_Typewriter();
                    break;
            }
    
            OnComplete();
        }
    
        private void OnComplete()
        {
            buildProcess= null;
            hurryUp = false; 
        }
    
    // forces text to stop on double click
        public void ForceComplete()
        {
            switch(buildMethod)
            {
                case BuildMethod.typewriter:
                    tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
                    break;  
            }
    
            Stop();
            OnComplete();
        }
    
    // prepares method to avoid glitches
        private void Prepare()
        {
            switch(buildMethod)
            {
                case BuildMethod.typewriter:
                    Prepare_Typewriter();
                    break;
            }
        }
    
        private void Prepare_Typewriter()
        {
            tmpro.color = tmpro.color;
            tmpro.maxVisibleCharacters = 0;
            tmpro.text = preText;
    
            if (preText != "")
            {
                tmpro.ForceMeshUpdate();
                tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
            }
    
            tmpro.text += targetText;
            tmpro.ForceMeshUpdate();
        }
    
    // builds typewriter method
        private IEnumerator Build_Typewriter()
        {
            while(tmpro.maxVisibleCharacters < tmpro.textInfo.characterCount)
            {
                tmpro.maxVisibleCharacters += hurryUp ? characterPerCycle * 5 : characterPerCycle;
    
                yield return new WaitForSeconds( 0.015f / speed);
            }
        }
    }
}*/