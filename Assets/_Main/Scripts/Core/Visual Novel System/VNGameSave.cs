using Dialogue;
using History;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace VisualNovel
{

    [System.Serializable]
    public class VNGameSave
    {
        public static VNGameSave activeFile = null;

        public const string file_type = ".vns";
        public const string screenshot_file_type = ".jpg";
        public const bool encrypt = true;

        public const float screenshot_downscale_amount = 1f;

        public string filePath => $"{FilePaths.gameSaves}{slotNumber}{file_type}";
        public string screenshotPath => $"{FilePaths.gameSaves}{slotNumber}{screenshot_file_type}";

        public string playerName;
        public string charisma;
        public int charVal = 0;
        //public string danceOffSkills;
        public int danceVal = 0;
        //public string guh;
        public int guhVal = 0;
        //public string swagginess;
        public int swagVal = 0;
        //public string rizz { get; private set; } = string.Empty;
        public int slotNumber = 1;

        public bool newGame = true;
        public string[] activeConversations;
        public HistoryState activeState;
        public HistoryState[] historyLogs;
        public VN_VariableData[] variables;

        public string timestamp;

        public static VNGameSave Load(string filePath, bool activateOnLoad = false)
        {
            VNGameSave save = FileManager.Load<VNGameSave>(filePath, encrypt);

            activeFile = save;

            if (activateOnLoad)
                save.Activate();

            return save;
        }
        public void Save()
        {
            newGame = false;

            activeState = HistoryState.Capture();
            historyLogs = HistoryManager.instance.history.ToArray();
            activeConversations = GetConversationData();
            variables = GetVariableData();

            timestamp = DateTime.Now.ToString("yy-MM-dd HH:mm:ss");

            ScreenshotMaster.CaptureScreenshot(VNManager.instance.mainCamera, Screen.width, Screen.height, screenshot_downscale_amount, screenshotPath);

            string saveJSON = JsonUtility.ToJson(this);
            FileManager.Save(filePath, saveJSON, encrypt); 
        }

        public void Activate()
        {
            if (activeState != null)
                activeState.Load();

            HistoryManager.instance.history = historyLogs.ToList();
            
            SetVariableData();

            SetConversationData();

            DialogueSystem.instance.prompt.Hide();
        }

        public string[] GetConversationData()
        {
            List<string> retData = new List<string>();

            var conversations = DialogueSystem.instance.conversationManager.GetConversationQueue();

            for (int i = 0; i < conversations.Length; i++)
            {
                var conversation = conversations[i];
                string data = "";

                if (conversation.file != string.Empty)
                {
                    var compressedData = new VN_ConversationDataCompressed();
                    compressedData.fileName = conversation.file;
                    compressedData.progress = conversation.GetProgress();
                    compressedData.startIndex = conversation.fileStartIndex;
                    compressedData.endindex = conversation.fileEndIndex;
                    data = JsonUtility.ToJson(compressedData);
                }

                else
                {
                    var fullData = new VN_ConversationData();
                    fullData.conversation = conversation.GetLines();
                    fullData.progress = conversation.GetProgress();
                    data = JsonUtility.ToJson(fullData);
                }

                retData.Add(data);
            }

            return retData.ToArray();
        }

        private void SetConversationData()
        {
            for (int i = 0; i < activeConversations.Length; i++)
            {
                try
                {
                    string data = activeConversations[i];
                    Conversation conversation = null;

                    var fullData = JsonUtility.FromJson<VN_ConversationData>(data);
                    if (fullData != null && fullData.conversation != null && fullData.conversation.Count > 0)
                    {
                        conversation = new Conversation(fullData.conversation, fullData.progress);
                    }

                    else
                    {
                        var compressedData = JsonUtility.FromJson<VN_ConversationDataCompressed>(data);
                        if (compressedData != null && compressedData.fileName != string.Empty)
                        {
                            TextAsset file = Resources.Load<TextAsset>(compressedData.fileName);

                            int count = compressedData.endindex - compressedData.startIndex;

                            List<string> lines = FileManager.ReadTextAsset(file).Skip(compressedData.startIndex).Take(count + 1).ToList();

                            conversation = new Conversation(lines, compressedData.progress, compressedData.fileName, compressedData.startIndex, compressedData.endindex);
                        }

                        else
                        {
                            Debug.LogError($"Unknown conversation format! Unable to reload conversation from VNGameSave using data '{data}'.");
                        }
                    }

                    if (conversation != null && conversation.GetLines().Count > 0)
                    {
                        if (i == 0)
                            DialogueSystem.instance.conversationManager.StartConversation(conversation);
                        else
                            DialogueSystem.instance.conversationManager.Enqueue(conversation);
                    }
                }

                catch (System.Exception e)
                {
                    Debug.LogError($"Encountered error while extracting saves conversation data {e}");
                    continue;
                }

            }
        }

        private VN_VariableData[] GetVariableData()
        {
            List<VN_VariableData> retData = new List<VN_VariableData>();

            foreach (var database in VariableStore.databases.Values)
            {
                foreach (var variable in database.variables)
                {
                    VN_VariableData variableData = new VN_VariableData();
                    variableData.name = $"{database.name}.{variable.Key}";
                    string val = $"{variable.Value.Get()}";
                    variableData.value = val;
                    variableData.type = val == string.Empty ? "System.String" : variable.Value.Get().GetType().ToString();
                    retData.Add(variableData);
                }
            }

            return retData.ToArray();
        }

        private void SetVariableData()
        {
            foreach (var variable in variables)
            {
                string val = variable.value;

                switch (variable.type)
                {
                    case "System.Boolean":
                        if (bool.TryParse(val, out bool b_val))
                        {
                            VariableStore.TrySetValue(variable.name, b_val);
                            continue;
                        }
                        break;

                    case "System.Int32":
                        if (int.TryParse(val, out int i_val))
                        {
                            VariableStore.TrySetValue(variable.name, i_val);
                            continue;
                        }
                        break;

                    case "System.Single":
                        if (float.TryParse(val, out float f_val))
                        { 
                            VariableStore.TrySetValue(variable.name, f_val);
                            continue;
                        }
                        break;

                    case "System.String":
                        VariableStore.TrySetValue(variable.name, val);
                        continue;

                }

                Debug.LogError($"Could not interperet variable tpe. {variable.name} = {variable.type}");
            }
        }


        /*public static void SetStat()
        {
            rizz = $"{charisma}: {charVal}";
            Debug.Log( rizz );
        }*/
    }
}