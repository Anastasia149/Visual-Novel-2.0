using AYellowpaper.SerializedCollections;
using Dialogue;
using TMPro;
using UnityEngine;

namespace Characters
{
    [System.Serializable]
    public class CharacterConfigData
    {
        public string name;
        public string alias;
        public Character.CharacterType characterType;

        public Color nameColor;
        public Color dialogueColor;

        public TMP_FontAsset nameFont;
        public TMP_FontAsset dialogueFont;

        public float nameFontScale = 1f;
        public float dialogueFontScale = 1f;

        [SerializedDictionary("Path / ID", "Sprite")]
        public SerializedDictionary<string, Sprite> sprites = new SerializedDictionary<string, Sprite>();

        public CharacterConfigData Copy()
        {
            CharacterConfigData result = new CharacterConfigData();

            result.name = name;
            result.alias = alias;
            result.characterType = characterType;

            result.nameColor = new Color(nameColor.r, nameColor.g, nameColor.b, nameColor.a);
            result.dialogueColor = new Color(dialogueColor.r, dialogueColor.g, dialogueColor.b, dialogueColor.a);

            result.nameFont = nameFont;
            result.dialogueFont = dialogueFont;

            result.nameFontScale = nameFontScale;
            result.dialogueFontScale = dialogueFontScale;

            return result;
        }

     // Default

        private static Color defaultColor => DialogueSystem.instance.config.defaultTextColor;
        private static TMP_FontAsset defaultFont => DialogueSystem.instance.config.defaultFont;
        public static CharacterConfigData Default
        {
            get
            {
                CharacterConfigData result = new CharacterConfigData();
 
                result.name = "";
                result.alias = "";
                result.characterType = Character.CharacterType.Text;

                result.nameColor = new Color(defaultColor.r, defaultColor.g, defaultColor.b, defaultColor.a);
                result.dialogueColor = new Color(defaultColor.r, defaultColor.g, defaultColor.b, defaultColor.a);

                result.nameFont = defaultFont;
                result.dialogueFont = defaultFont;

                result.nameFontScale = 1f;
                result.dialogueFontScale = 1f;

                return result;
            }
        }
    }
}