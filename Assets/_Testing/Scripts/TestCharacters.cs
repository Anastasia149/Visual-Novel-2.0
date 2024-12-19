using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using TMPro;    
using Dialogue;

namespace Testing
{
    public class TestCharacters : MonoBehaviour
    {
        [SerializeField] private TextAsset fileToRead = null;
        public TMP_FontAsset tempFont;

        private Character CreateCharacter(string name) => CharacterManager.instance.CreateCharacter(name);

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Test());
            //Character Alexandria = CharacterManager.instance.CreateCharacter("Alexandria");
            //Character Teevee = CharacterManager.instance.CreateCharacter("Teevee");
        }

        IEnumerator Test()
        {

            //Character Alexandria = CharacterManager.instance.CreateCharacter("Alexandria");
            //Character Teevee = CharacterManager.instance.CreateCharacter("Teevee");

            CharSprite Alexandria = CreateCharacter("Alexandria") as CharSprite;
            CharSprite Teevee = CreateCharacter("Teevee") as CharSprite;
            //Character Loki = CreateCharacter("Loki");
            //Character Scylla = CreateCharacter("Scylla");
            //Character Shein = CreateCharacter("Shein");
            //Character Mako = CreateCharacter("Mako");

            Alexandria.SetPosition(Vector2.zero);
            Teevee.SetPosition(new Vector2(1,0));

            yield return new WaitForSeconds(1);

            yield return Teevee.Flip();

            yield return Alexandria.Flip(0.3f);

            Alexandria.UnHighlight();
            yield return Teevee.Say("guhjsdfuhgofg");

            Teevee.UnHighlight();
            Alexandria.Highlight();
            yield return Alexandria.Say("bruh what makes no sense");

            Alexandria.UnHighlight();
            Teevee.Highlight();
            Teevee.SetSprite(Teevee.GetSprite("Teevee - Neutral"), layer: 0);
            yield return Teevee.Say("AAAAAAAAAAAAAAAAAAAAA");

            Teevee.UnHighlight();
            Alexandria.Highlight();
            Alexandria.SetSprite(Alexandria.GetSprite("Alexandria - Upset"), layer:0);
            yield return Alexandria.Say("thats rude");

            yield return null;
        }

        // Update is called once per frame
        void Update()
        {

       }
    }
}