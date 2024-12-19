using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VisualNovel
{
    public class VNDatabaseLinkSetup : MonoBehaviour
    {
        public void SetupExternalLinks()
        {
            VariableStore.CreateVariable("VN.mainCharName", "", () => VNGameSave.activeFile.playerName, value => VNGameSave.activeFile.playerName = value);
            VariableStore.CreateVariable("Stats.Charisma", 0, () => VNGameSave.activeFile.charVal, value => VNGameSave.activeFile.charVal = value);
            VariableStore.CreateVariable("Stats.Danceoffskills", 0, () => VNGameSave.activeFile.danceVal, value => VNGameSave.activeFile.danceVal = value);
            VariableStore.CreateVariable("Stats.guh", 0, () => VNGameSave.activeFile.guhVal, value => VNGameSave.activeFile.guhVal = value);
            VariableStore.CreateVariable("Stats.Swagginess", 0, () => VNGameSave.activeFile.swagVal, value => VNGameSave.activeFile.swagVal = value);
        }
    }
}