using Commands;
using Dialogue;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Characters
{
    public class CharacterManager : MonoBehaviour
    {
        public static CharacterManager instance { get; private set; }

        public Character[] allCharacters => characters.Values.ToArray();

        private Dictionary<string, Character> characters = new Dictionary<string, Character>();

        private CharacterConfigSO config => DialogueSystem.instance.config.characterConfigurationAsset;

        private const string CharacterCastingID = " as ";

        private const string CharacterNameID = "<charname>";
        public string characterRootPathFormat => $"Characters/{CharacterNameID}";
        public string characterPrefabNameFormat => $"Characters - {CharacterNameID}";
        public string characterPrefabPathFormat => $"{characterRootPathFormat}/Character-[{CharacterNameID}]";


        [SerializeField] private RectTransform _characterpanel = null;
        public RectTransform characterPanel => _characterpanel;


        private void Awake()
        {
            instance = this;
        }

        public CharacterConfigData GetCharacterConfig(string characterName)
        {
            return config.GetConfig(characterName);
        }

        public Character GetCharacter(string characterName, bool createIfDoesNotExist = false)
        {
            if (characters.ContainsKey(characterName.ToLower()))
                return characters[characterName.ToLower()];

            else if (createIfDoesNotExist) 
                return CreateCharacter(characterName);

            return null;
        }

        // Character specific commands
        public bool HasCharacter(string characterName) => characters.ContainsKey(characterName.ToLower());

        public Character CreateCharacter(string characterName, bool revealAfterCreation = false)
        {
            if (characters.ContainsKey(characterName.ToLower()))
            {
                Debug.LogWarning($"A character called '{characterName}' already exists, Could not create character.");
                return null;
            }

            CharacterInfo info = GetCharacterInfo(characterName);

            Character character = CreateCharacterFromInfo(info);

            characters.Add(info.name.ToLower(), character);

            if (revealAfterCreation)
                character.Show();

            return character;
        }

        private CharacterInfo GetCharacterInfo(string characterName)
        {
            CharacterInfo result = new CharacterInfo();
            
            string[] nameData = characterName.Split(CharacterCastingID, System.StringSplitOptions.RemoveEmptyEntries);
            result.name = nameData[0];
            result.castingName = nameData.Length > 1 ? nameData[1] : result.name;

            result.config = config.GetConfig(result.castingName);

            result.prefab = GetPrefabForCharacter(result.castingName);

            result.rootCharacterFolder = FormatCharacterPath(characterRootPathFormat, result.castingName);
          
            return result;
        }

        private GameObject GetPrefabForCharacter(string characterName)
        {
            string prefabPath = FormatCharacterPath(characterPrefabPathFormat, characterName);
            return Resources.Load<GameObject>(prefabPath);
        }

        public string FormatCharacterPath (string path, string characterName) => path.Replace(CharacterNameID, characterName);

        private Character CreateCharacterFromInfo(CharacterInfo info)
        {
            CharacterConfigData config = info.config;

            switch (config.characterType)
            {
                case Character.CharacterType.Text:
                    return new CharText(info.name, config);

                case Character.CharacterType.Sprite:
                case Character.CharacterType.SpriteSheet:
                    return new CharSprite(info.name, config, info.prefab, info.rootCharacterFolder);

                default:
                    return null;
            }
        }

        public void SortCharacters()
        {
            List<Character> activeCharacters = characters.Values
                .Where(c => c.root.gameObject.activeInHierarchy && c.isVisible)
                .ToList();

            List<Character> inactiveCharacters = characters.Values
                .Except(activeCharacters)
                .ToList();

            activeCharacters.Sort((a, b) => a.priority.CompareTo(b.priority));
            activeCharacters.Concat(inactiveCharacters);

            SortCharacters(activeCharacters);
        }

        public void SortCharacters(string[] characternames)
        {
            List<Character> sortedCharacters = new List<Character>();

            sortedCharacters = characternames
                .Select(name => GetCharacter(name))
                .Where(character => character != null)
                .ToList();

            List<Character> remainingCharacters = characters.Values
                .Except(sortedCharacters)
                .OrderBy(character => character.priority)
                .ToList();

            sortedCharacters.Reverse();

            int startingPriority = remainingCharacters.Count > 0 ? remainingCharacters.Max(c => c.priority) : 0;
            for (int i = 0; i < sortedCharacters.Count; i++)
            {
                Character character = sortedCharacters[i];
                character.SetPriority(startingPriority + i + 1, autoSortCharactersOnUI: false);
            }

            List<Character> allCharacters = remainingCharacters.Concat(sortedCharacters).ToList();
            SortCharacters(allCharacters);
        }

        private void SortCharacters(List<Character> charactersSortingOrder)
        {
            int i = 0;
            foreach (Character character in charactersSortingOrder)
            {
                Debug.Log($"{character.name} priority is {character.priority}");
                character.root.SetSiblingIndex(i++);
            }
        }

        private class CharacterInfo
        {
            public string name = "";
            public string castingName = "";

            public string rootCharacterFolder = "";

            public CharacterConfigData config = null;

            public GameObject prefab = null;
        }
    }
}
