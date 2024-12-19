using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Characters;
using System.Linq;

namespace Commands
{
    public class CMD_DatabaseExtension_Characters : CMD_DatabaseExtension
    {
        private static string[] Param_Enable => new string[] { "-e", "-enable" };
        private static string[] Param_Immediate => new string[] { "-i", "-immediate" };     // Specify within the MoveCharacter command - will not work for CreateCharacter
        private static string[] Param_Speed => new string[] { "-spd", "-speed" };
        private static string[] Param_Smooth => new string[] { "-s", "-smooth" };
        private static string[] Param_State => new string[] { "-st", "-state" };
        private static string Param_XPos => "-x";
        private static string Param_YPos => "-y";
        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("createcharacter", new Action<string[]>(CreateCharacter));
            database.AddCommand("movecharacter", new Func<string[], IEnumerator>(MoveCharacter));
            database.AddCommand("show", new Func<string[], IEnumerator>(ShowAll));
            database.AddCommand("hide", new Func<string[], IEnumerator>(HideAll));
            database.AddCommand("sort", new Action<string[]>(Sort));
            database.AddCommand("highlight", new Func<string[], IEnumerator>(HighlightAll));
            database.AddCommand("unhighlight", new Func<string[], IEnumerator>(UnHighlightAll));

            // Character specific commands
            CommandDatabase baseCommands = CommandManager.instance.CreateSubDatabase(CommandManager.Database_Characters_Base);
            baseCommands.AddCommand("move", new Func<string[], IEnumerator>(MoveCharacter));
            baseCommands.AddCommand("show", new Func<string[], IEnumerator>(Show));
            baseCommands.AddCommand("hide", new Func<string[], IEnumerator>(Hide));
            baseCommands.AddCommand("setpriority", new Action<string[]>(SetPriority));
            baseCommands.AddCommand("setposition", new Action<string[]>(SetPosition));
            baseCommands.AddCommand("setcolor", new Func<string[], IEnumerator>(SetColor));
            baseCommands.AddCommand("highlight", new Func<string[], IEnumerator>(Highlight));
            baseCommands.AddCommand("unhighlight", new Func<string[], IEnumerator>(Unhighlight));

            // Add character specific databases
            CommandDatabase spriteCommands = CommandManager.instance.CreateSubDatabase(CommandManager.Database_Characters_Sprite);
            spriteCommands.AddCommand("setsprite", new Func<string[], IEnumerator>(SetSprite));
            spriteCommands.AddCommand("flip", new Func<string[], IEnumerator>(Flip));
            spriteCommands.AddCommand("animate", new Func<string[], IEnumerator>(Animate));
        }

        #region Sorting characters
        private static void Sort(string[] data)
        {
            CharacterManager.instance.SortCharacters(data);
        }
        #endregion

        #region Create character
        public static void CreateCharacter(string[] data)
        {
            string characterName = data[0];
            bool enable = false;
            bool immediate = false;
            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(Param_Enable, out enable, defaultValue: false);
            parameters.TryGetValue(Param_Immediate, out immediate, defaultValue: false);
 

            Character character = CharacterManager.instance.CreateCharacter(characterName);

            if (!enable)
                return;

            else if (immediate)
            {
                character.isVisible = true;
                character.Show();
            }

        }
        #endregion

        #region Move character
        private static IEnumerator MoveCharacter(string[] data)
        {
            string characterName = data[0];
            Character character = CharacterManager.instance.GetCharacter(characterName);

            if (character == null)
                yield break;

            float x = 0, y = 0;
            float speed = 1;
            bool smoothing = false;
            bool immediate = false;

            var parameters = ConvertDataToParameters(data);

            // try to get the x axis position
            parameters.TryGetValue(Param_XPos, out x);

            // try to get the y axis position
            parameters.TryGetValue(Param_YPos, out y);

            // try to get the speed
            parameters.TryGetValue(Param_Speed, out speed, defaultValue: 1);

            // try to get smoothing
            parameters.TryGetValue(Param_Smooth, out smoothing, defaultValue: false);

            // try to get immediate setting of position
            parameters.TryGetValue(Param_Immediate, out immediate, defaultValue: false);

            Vector2 position = new Vector2(x, y);

            if (immediate)
                character.SetPosition(position);
            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { character?.SetPosition(position); });
                yield return character.MoveToPostion(position, speed, smoothing);
            }
        }
        #endregion

        #region Show characters
        public static IEnumerator ShowAll(string[] data)
        {
            List<Character> characters = new List<Character>();
            bool immediate = false;
            float speed = 1f;

            foreach (string s in data)
            {
                Character character = CharacterManager.instance.GetCharacter(s, createIfDoesNotExist: false);
                if (character != null)
                    characters.Add(character);
            }

            if (characters.Count == 0)
                yield break;

            // Convert the data array to a parameter container
            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(Param_Immediate, out immediate, defaultValue: false);
            parameters.TryGetValue(Param_Speed, out speed, defaultValue: 1f);

            // Call the logic on all the characters
            foreach (Character character in characters)
            {
                if (immediate)
                    character.isVisible = true;
                else
                    character.Show(speed);
            }

            if(!immediate)
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() =>
                {
                    foreach(Character character in characters)
                        character.isVisible = true;
                });

                while(characters.Any(c => c.isRevealing))
                    yield return null;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------\\
        // Specific to sub database
        private static IEnumerator Show(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0]);

            if (character == null)
                yield break;

            bool immediate = false;
            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(new string[] { "-i", "-immediate" }, out immediate, defaultValue: false);

            if (immediate)
                character.isVisible = true;

            else
            {
                // a long running process should have a stop action to cancel out the coroutine and run logic that should complete this command
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { if (character != null) character.isVisible = true; });

                yield return character.Show();
            }
            yield return null;
        }
        #endregion

        #region Hide characters
        public static IEnumerator HideAll(string[] data)
        {
            List<Character> characters = new List<Character>();
            bool immediate = false;
            float speed = 1f;

            foreach (string s in data)
            {
                Character character = CharacterManager.instance.GetCharacter(s, createIfDoesNotExist: false);
                if (character != null)
                    characters.Add(character);
            }

            if (characters.Count == 0)
                yield break;

            // Convert the data array to a parameter container
            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(Param_Immediate, out immediate, defaultValue: false);
            parameters.TryGetValue(Param_Speed, out speed, defaultValue: 1f);

            foreach (Character character in characters)
            {
                if (immediate)
                    character.isVisible = false;
                else
                    character.Hide(speed);
            }

            if (!immediate)
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() =>
                {
                    foreach (Character character in characters)
                        character.isVisible = false;
                });

                while (characters.Any(c => c.isHiding))
                    yield return null;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------\\
        // specific to subdatabases
        private static IEnumerator Hide(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0]);

            if (character == null)
                yield break;

            bool immediate = false;
            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(new string[] { "-i", "-immediate" }, out immediate, defaultValue: false);

            if (immediate)
                character.isVisible = false;

            else
            {
                // a long running process should have a stop action to cancel out the coroutine and run logic that should complete this command
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { if (character != null) character.isVisible = false; });

                yield return character.Hide();
            }
        }
        #endregion

        #region Set character position
        public static void SetPosition(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0], createIfDoesNotExist: false);
            float x = 0, y = 0;

            if (character == null || data.Length < 2)
                return;

            var parameters = ConvertDataToParameters(data, 1);

            parameters.TryGetValue(Param_XPos, out x, defaultValue: 0);
            parameters.TryGetValue(Param_YPos, out y, defaultValue: 0);

            character.SetPosition(new Vector2(x, y));

        }
        #endregion

        #region Set character priority
        public static void SetPriority(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0], createIfDoesNotExist: false);
            int priority;

            if (character == null || data.Length < 2)
                    return;
            if (!int.TryParse(data[1], out priority))
                priority = 0;

            character.SetPriority(priority);
        }

        #endregion

        #region Set character color
        private static IEnumerator SetColor(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0], createIfDoesNotExist: false);
            string colorName;
            float speed;
            bool immediate;

            if (character == null || data.Length < 2)
                yield break;

            // grab the extra parameters
            var parameters = ConvertDataToParameters(data, startingIndex: 1);

            // Try to get color name
            parameters.TryGetValue(new string[] { "-c", "-color" }, out colorName);
            // Try to get transition speed
            bool specifiedSpeed = parameters.TryGetValue(new string[] { "-spd", "-speed" }, out speed, defaultValue: 1f);
            // Try to get the instant value
            if (!specifiedSpeed)
                parameters.TryGetValue(new string[] { "-i", "-immediate" }, out immediate, defaultValue: true);
            else
                immediate = false;

            // get the color value from the name
            Color color = Color.white;
            color = color.GetColorFromName(colorName);

            if (immediate)
                character.SetColor(color);

            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { character?.SetColor(color); });
                character.TransitionColor(color, speed);
            }

            yield break;
        }
        #endregion

        #region Highlight character

        public static IEnumerator HighlightAll(string[] data)
        {
            List<Character> characters = new List<Character>();
            bool immediate = false;
            bool handleUnspecifiedCharacters = true;
            List<Character> unspecifiedCharacters = new List<Character>();

            // Add any characters specified to be highlighted
            for (int i = 0; i < data.Length; i++)
            {
                Character character = CharacterManager.instance.GetCharacter(data[i], createIfDoesNotExist: false);
                if (character != null)
                    characters.Add(character);
            }

            if (characters.Count == 0)
                yield break;

            // Grab the extra parameters
            var parameters = ConvertDataToParameters(data, startingIndex: 1);

            parameters.TryGetValue(new string[] { "-i", "-immediate" }, out immediate, defaultValue: false);
            parameters.TryGetValue(new string[] { "-o", "-only" }, out handleUnspecifiedCharacters, defaultValue: true);

            // make all characters perform the logic
            foreach (Character character in characters)
                character.Highlight(immediate: immediate);

            // if we are forcing any unspecified characters to use the opposite highlighted status
            if (handleUnspecifiedCharacters)
            {
                foreach (Character character in CharacterManager.instance.allCharacters)
                {
                    if (characters.Contains(character))
                        continue;

                    unspecifiedCharacters.Add(character);
                    character.UnHighlight(immediate: immediate);
                }
            }

            // Wait for all characters to finish highlighting
            if (!immediate)
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() =>
                {
                    foreach (var character in characters)
                        character.Highlight(immediate: true);

                    if (!handleUnspecifiedCharacters) return;

                    foreach (var character in unspecifiedCharacters) 
                        character.Highlight(immediate: true);
                });

                while (characters.Any(c => c.isHighlighting) || (handleUnspecifiedCharacters && unspecifiedCharacters.Any(unspecifiedCharacters => unspecifiedCharacters.isUnHighlighting)))
                    yield return null;
            }
        }

        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------\\
        // Specific to subdatabase
        public static IEnumerator Highlight(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0], createIfDoesNotExist : false) as Character;

            if (character == null)
                yield break;

            bool immediate = false;

            // grab the xtra parameters
            var parameters = ConvertDataToParameters(data, startingIndex: 1);

            parameters.TryGetValue(new string[] { "-i", "-immediate" }, out immediate, defaultValue: false);

            if (immediate)
                character.Highlight(immediate: true);

            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { character?.Highlight(immediate: true); });
                yield return character.Highlight();
            }
        }
        #endregion

        #region Character Flipping
        public static IEnumerator Flip(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0], createIfDoesNotExist: false) as Character;
            int layer = 0;
            string spriteName;
            bool immediate = false;
            float speed;

            if (character == null || data.Length < 2)
                yield break;

            // Grab the extra parameters
            var parameters = ConvertDataToParameters(data, startingIndex: 1);

            // Try to get the sprite name
            parameters.TryGetValue(new string[] { "-s", "-sprite" }, out spriteName);
            // Try to get the layer
            parameters.TryGetValue(new string[] { "-l", "-layer" }, out layer, defaultValue: 0);

            // Try to get the transition speed
            bool specifiedSpeed = parameters.TryGetValue(Param_Speed, out speed, defaultValue: 0.1f);

            // Try to get whether this is an immediate transition or not
            if (!specifiedSpeed)
                parameters.TryGetValue(Param_Immediate, out immediate, defaultValue: true);

            if (immediate)
                character.Flip(immediate: true);

            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { character?.Flip(); });
                yield return character.Flip(speed);
            }
            yield return null;

        }
        #endregion

        #region Unhighlight character
        public static IEnumerator UnHighlightAll(string[] data)
        {
            List<Character> characters = new List<Character>();
            bool immediate = false;
            bool handleUnspecifiedCharacters = true;
            List<Character> unspecifiedCharacters = new List<Character>();

            // Add any characters specified to be highlighted
            for (int i = 0; i < data.Length; i++)
            {
                Character character = CharacterManager.instance.GetCharacter(data[i], createIfDoesNotExist: false);
                if (character != null)
                    characters.Add(character);
            }

            if (characters.Count == 0)
                yield break;

            // Grab the extra parameters
            var parameters = ConvertDataToParameters(data, startingIndex: 1);

            parameters.TryGetValue(new string[] { "-i", "-immediate" }, out immediate, defaultValue: false);
            parameters.TryGetValue(new string[] { "-o", "-only" }, out handleUnspecifiedCharacters, defaultValue: true);

            // make all characters perform the logic
            foreach (Character character in characters)
                character.UnHighlight(immediate: immediate);

            // if we are forcing any unspecified characters to use the opposite highlighted status
            if (handleUnspecifiedCharacters)
            {
                foreach (Character character in CharacterManager.instance.allCharacters)
                {
                    if (characters.Contains(character))
                        continue;

                    unspecifiedCharacters.Add(character);
                    character.Highlight(immediate: immediate);
                }
            }

            // Wait for all characters to finish highlighting
            if (!immediate)
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() =>
                {
                    foreach (var character in characters)
                        character.UnHighlight(immediate: true);

                    if (!handleUnspecifiedCharacters) return;

                    foreach (var character in unspecifiedCharacters)
                        character.Highlight(immediate: true);
                });

                while (characters.Any(c => c.isUnHighlighting) || (handleUnspecifiedCharacters && unspecifiedCharacters.Any(unspecifiedCharacters => unspecifiedCharacters.isHighlighting)))
                    yield return null;
            }
        }

        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------\\
        // Specific to subdatabase

        public static IEnumerator Unhighlight(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0], createIfDoesNotExist: false) as Character;

            if (character == null)
                yield break;

            bool immediate = false;

            // grab the xtra parameters
            var parameters = ConvertDataToParameters(data, startingIndex: 1);

            parameters.TryGetValue(new string[] { "-i", "-immediate" }, out immediate, defaultValue: false);

            if (immediate)
                character.UnHighlight(immediate: true);

            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { character?.UnHighlight(immediate: true); });
                yield return character.UnHighlight();
            }
        }
        #endregion

        #region Set character sprite
        public static IEnumerator SetSprite(string[] data)
        {
            CharSprite character = CharacterManager.instance.GetCharacter(data[0], createIfDoesNotExist: false) as CharSprite;
            int layer = 0;
            string spriteName;
            bool immediate = false;
            float speed;

            if (character == null || data.Length < 2)
                yield break;

            // Grab the extra parameters
            var parameters = ConvertDataToParameters(data, startingIndex: 1);

            // Try to get the sprite name
            parameters.TryGetValue(new string[] { "-s", "-sprite" }, out spriteName);
            // Try to get the layer
            parameters.TryGetValue(new string[] { "-l", "-layer" }, out layer, defaultValue: 0);

            // Try to get the transition speed
            bool specifiedSpeed = parameters.TryGetValue(Param_Speed, out speed, defaultValue: 0.1f);

            // Try to get whether this is an immediate transition or not
            if (!specifiedSpeed)
                parameters.TryGetValue(Param_Immediate, out immediate, defaultValue: true);

            // Run the logic
            Sprite sprite = character.GetSprite(spriteName);

            if (sprite == null)
                yield break;

            if (immediate)
                character.SetSprite(sprite, layer);

            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => {  character?.SetSprite(sprite, layer); });
                yield return character.TransitionSprite(sprite, layer, speed);
            }
        }
        #endregion

        #region Character animation
        public static IEnumerator Animate(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0], createIfDoesNotExist: false) as Character;
            int layer = 0;
            string spriteName;
            bool state = false;
            string animation;
            float speed;

            if (character == null || data.Length < 2)
                yield break;

            // Grab the extra parameters
            var parameters = ConvertDataToParameters(data, startingIndex: 1);

            // Try to get the sprite name
            parameters.TryGetValue(new string[] { "-s", "-sprite" }, out spriteName);
            // Try to get the layer
            parameters.TryGetValue(new string[] { "-l", "-layer" }, out layer, defaultValue: 0);
            // Try to get the animation
            parameters.TryGetValue(new string[] { "-a", "-animation" }, out animation);
            // try to get smoothing
            parameters.TryGetValue(Param_State, out state, defaultValue: false);

            //character.animator.SetTrigger(animation);
            //character.animator.SetBool(animation, state);
            // Try to get the transition speed
            bool specifiedSpeed = parameters.TryGetValue(Param_Speed, out speed, defaultValue: 0.1f);

            if (!specifiedSpeed)
                parameters.TryGetValue(Param_State,out state, defaultValue: true);

            if (state)
                character.Animate(animation, state);

            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { character?.Animate(animation, state); });
                yield return character.Animate(animation, state);
            }

            yield return null;
        }
        #endregion
    }
}
