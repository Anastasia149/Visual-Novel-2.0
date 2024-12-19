using UnityEngine;
using Characters;
using TMPro;

namespace Dialogue
{

    [CreateAssetMenu(fileName = "Dialogue System Configuration", menuName = "Dialogue System/Dialogue Configuration Asset")]
    public class DialogueSystemConfigSO : ScriptableObject
    {
        public const float default_fontsize_name = 34;
        public const float default_fontsize_dialogue = 30;

        public CharacterConfigSO characterConfigurationAsset;

        public Color defaultTextColor = Color.white;    
        public TMP_FontAsset defaultFont;

        public float dialogueFontScale = 1f;
        public float defaultNameFontSize = default_fontsize_name;
        public float defaultDialogueFontSize = default_fontsize_dialogue;
    }
}