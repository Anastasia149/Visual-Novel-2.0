using Dialogue;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using VisualNovel;

namespace Commands
{
    public class CMD_DatabaseExtension_General : CMD_DatabaseExtension
    {
        #region Parameters
        private static readonly string[] Param_Immediate = new string[] { "-i", "-immediate" };     // Specify within the MoveCharacter command - will not work for CreateCharacter
        private static readonly string[] Param_Speed = new string[] { "-spd", "-speed" };
        private static readonly string[] Param_Filepath = new string[] { "-f", "-file" };
        private static readonly string[] Param_Enqueue = new string[] { "-e", "-enqueue" };
        #endregion


        #region Extend
        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("wait", new Func<string, IEnumerator>(Wait));

            // Dialogue box controls
            database.AddCommand("showdb", new Func<string[], IEnumerator>(ShowDialogueBox));
            database.AddCommand("hidedb", new Func<string[], IEnumerator>(HideDialogueBox));

            // Dialogue system controls
            database.AddCommand("showui", new Func<string[], IEnumerator>(ShowDialogueSystem));
            database.AddCommand("hideui", new Func<string[], IEnumerator>(HideDialogueSystem));

            // Map visibility
            database.AddCommand("showmap", new Func<string[], IEnumerator>(ShowMap));
            database.AddCommand("hidemap", new Func<string[], IEnumerator>(HideMap));

            database.AddCommand("load", new Action<string[]>(LoadNewDialogueFile));
        }
        #endregion

        #region Wait
        private static IEnumerator Wait(string data)
        {
            if (float.TryParse(data, out float time)) 
            {
                yield return new WaitForSeconds(time);
            }
        }
        #endregion

        #region Show / Hide Map
        private static IEnumerator ShowMap(string[] data)
        {
            float speed;
            bool immediate;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(Param_Speed, out speed, defaultValue: 1f);
            parameters.TryGetValue(Param_Immediate, out immediate, defaultValue: false);

            yield return DialogueSystem.instance.map.Show(speed, immediate);
        }

        private static IEnumerator HideMap(string[] data)
        {
            float speed;
            bool immediate;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(Param_Speed, out speed, defaultValue: 1f);
            parameters.TryGetValue(Param_Immediate, out immediate, defaultValue: false);

            yield return DialogueSystem.instance.map.Hide(speed, immediate);
        }
        #endregion

        #region Show / Hide Dialogue Box
        private static IEnumerator ShowDialogueBox(string[] data)
        {
            float speed;
            bool immediate;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(Param_Speed, out speed, defaultValue: 1f);
            parameters.TryGetValue(Param_Immediate, out immediate, defaultValue: false);

            yield return DialogueSystem.instance.dialogueContainer.Show(speed, immediate);
        }

        private static IEnumerator HideDialogueBox(string[] data)
        {
            float speed;
            bool immediate;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(Param_Speed, out speed, defaultValue: 1f);
            parameters.TryGetValue(Param_Immediate, out immediate, defaultValue: false);

            yield return DialogueSystem.instance.dialogueContainer.Hide(speed, immediate);
        }
        #endregion

        #region Show / Hide Dialogue System
        private static IEnumerator ShowDialogueSystem(string[] data)
        {
            float speed;
            bool immediate;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(Param_Speed, out speed, defaultValue: 1f);
            parameters.TryGetValue(Param_Immediate, out immediate, defaultValue: false);

            yield return DialogueSystem.instance.Show(speed, immediate);
        }

        private static IEnumerator HideDialogueSystem(string[] data)
        {
            float speed;
            bool immediate;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(Param_Speed, out speed, defaultValue: 1f);
            parameters.TryGetValue(Param_Immediate, out immediate, defaultValue: false);

            yield return DialogueSystem.instance.Hide(speed, immediate);
        }
        #endregion

        #region Load
        private static void LoadNewDialogueFile(string[] data)
        {
            string fileName = string.Empty;
            bool enqueue = false;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(Param_Filepath, out fileName);
            parameters.TryGetValue(Param_Enqueue, out enqueue, defaultValue: false);

            string filePath = FilePaths.GetPathToResources(FilePaths.resources_dialogueFiles, fileName);
            TextAsset file = Resources.Load<TextAsset>(filePath);

            if (file == null)
            {
                Debug.LogWarning($"File '{filePath}' could not be loaded from dialogue files. Please ensure it exists within the '{FilePaths.resources_dialogueFiles}' resources folder.");
                return;
            }

            List<string> lines = FileManager.ReadTextAsset(file, includeBlankLines: true);
            Conversation newConversation = new Conversation(lines);

            if (enqueue)
                DialogueSystem.instance.conversationManager.Enqueue(newConversation);
            else
                DialogueSystem.instance.conversationManager.StartConversation(newConversation);
        }

        #endregion


    }
}
