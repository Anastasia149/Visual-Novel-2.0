using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VN_Configuration
{
    public static VN_Configuration activeConfig;

    public static string filePath => $"{FilePaths.root}vnconfig.cfg";

    public const bool Encrypt = false;

    #region General Settings
    public bool display_fullscreen = true;
    public string display_resolution = "1920x1080";
    public bool continueSkippingAfterChoice = false;
    public float dialogueTextSpeed = 1f;
    public float dialogueAutoReadSpeed = 1f;
    #endregion

    #region Audio Settings
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
    public bool musicMute = false;
    public bool sfxMute = false;
    #endregion

    public void Load()
    {
        var ui = ConfigPage.instance.ui;

    // General settings
        // Window size
        ConfigPage.instance.SetDisplayToFullScreen(display_fullscreen);
        ui.SetButtonColors(ui.fullscreen, ui.windowed, display_fullscreen);

        // Screen Resolution
        int res_index = 0;
        for (int i = 0; i < ui.resolutions.options.Count; i++)
        {
            string resolution = ui.resolutions.options[i].text;
            if (resolution == display_resolution) 
            {
                res_index = i;
                break;
            }
        }
        ui.resolutions.value = res_index;

        // Continue After Skipping
        ui.SetButtonColors(ui.skippingContinue, ui.skippingStop, continueSkippingAfterChoice);

        // Architect and auto reader speed
        ui.architectSpeed.value = dialogueTextSpeed;
        ui.autoreaderSpeed.value = dialogueAutoReadSpeed;


    // Audio Settings
        // Volumes
        ui.musicVolume.value = musicVolume;
        ui.sfxVolume.value = sfxVolume;
        ui.musicMute.sprite = musicMute ? ui.mutedSymbol : ui.unmutedSymbol;
        ui.sfxMute.sprite = sfxMute ? ui.mutedSymbol : ui.unmutedSymbol;
        
    }

    public void Save()
    {
        FileManager.Save(filePath, JsonUtility.ToJson(this), encrypt: Encrypt);
    }
}
