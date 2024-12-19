using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.IO;
using Dialogue;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class ConfigPage : MenuPage
{
    public static ConfigPage instance {  get; private set; }   
    
    [SerializeField] private GameObject[] panels;
    private GameObject activePanel;

    public UI_Items ui;

    private VN_Configuration config => VN_Configuration.activeConfig;

    private void Awake()
    {
       instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == 0);
        }

        activePanel = panels[0];

        SetAvailableResolutions();

        LoadConfig();
    }

    private void LoadConfig()
    {
        if (File.Exists(VN_Configuration.filePath))
            VN_Configuration.activeConfig = FileManager.Load<VN_Configuration>(VN_Configuration.filePath, encrypt: VN_Configuration.Encrypt);
    
        else
            VN_Configuration.activeConfig = new VN_Configuration();

        VN_Configuration.activeConfig.Load();
    }

    private void OnApplicationQuit()
    {
        VN_Configuration.activeConfig.Save();
        VN_Configuration.activeConfig = null;
    }

    public void OpenPanel(string panelName)
    {
        GameObject panel = panels.First(p => p.name.ToLower() == panelName.ToLower());

        if (panel == null)
        {
            Debug.LogWarning($"Did not find panel called '{panelName}' in config menu");
            return;
        }

        if (activePanel != null && activePanel != panel)
            activePanel.SetActive(false);

        panel.SetActive(true);
        activePanel = panel;
    }

    private void SetAvailableResolutions()
    {
        Resolution[] resolutions = Screen.resolutions;
        List<string> options = new List<string>();

        for (int i = resolutions.Length - 1; i >= 0; i--)
        {
            options.Add($"{resolutions[i].width}x{resolutions[i].height}");
        }

        ui.resolutions.ClearOptions();
        ui.resolutions.AddOptions(options);
    }

    [System.Serializable]
    public class UI_Items
    {
        private static Color button_selectedColor = new Color(1f, 1f, 1f, 1);
        private static Color button_unselectedColor = new Color(1f, 1f, 1f, 1);
        private static Color text_selectedColor = new Color(0.25f, 0.25f, 0.25f, 1);
        private static Color text_unselectedColor = new Color(0.25f, 0.25f, 0.25f, 1);

        public static Color musicOnColor = new Color(1, 0.65f, 0, 1);
        public static Color musicOffColor = new Color(0.5f, 0.5f, 0.5f, 1);

        [Header("General")]
        public Button fullscreen;
        public Button windowed;
        public TMP_Dropdown resolutions;
        public Button skippingContinue, skippingStop;
        public Slider architectSpeed, autoreaderSpeed;

        [Header("Audio")]
        public Slider musicVolume;
        public Image musicFill;
        public Image sfxFill;
        public Slider sfxVolume;
        public Image musicMute;
        public Image sfxMute;
        public Sprite mutedSymbol;
        public Sprite unmutedSymbol;

        public void SetButtonColors(Button A, Button B, bool selectedA)
        {
            A.GetComponent<Image>().color = selectedA ? button_selectedColor : button_unselectedColor;
            B.GetComponent<Image>().color = !selectedA ? button_selectedColor : button_unselectedColor;

            A.GetComponentInChildren<TextMeshProUGUI>().color = selectedA ? text_selectedColor : text_unselectedColor;
            B.GetComponentInChildren<TextMeshProUGUI>().color = !selectedA ? text_selectedColor : text_unselectedColor;
        }
        
    }

    // UI Callable functions
    public void SetDisplayToFullScreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
        ui.SetButtonColors(ui.fullscreen, ui.windowed, fullscreen);
    }

    public void SetDisplayResolution()
    {
        string resolution = ui.resolutions.captionText.text;
        string[] values = resolution.Split('x');

        if (int.TryParse(values[0], out int width) && int.TryParse(values[1], out int height))
        {
            Screen.SetResolution(width, height, Screen.fullScreen);
            config.display_resolution = resolution;
        }

        else
            Debug.LogError($"Parsing error for screen resolution! [{resolution}] could not be parsed into WidthxHeight");
    }

    public void SetContinueSkippingAfterChoice(bool continueSkipping)
    {
        config.continueSkippingAfterChoice = continueSkipping;
        ui.SetButtonColors(ui.skippingContinue, ui.skippingStop, continueSkipping);
    }

    public void SetTextArchitectSpeed()
    {
        config.dialogueTextSpeed = ui.architectSpeed.value;

        if (DialogueSystem.instance != null)
            DialogueSystem.instance.conversationManager.architect.speed = config.dialogueTextSpeed;
    }

    public void SetAutoReaderSpeed()
    {
        config.dialogueAutoReadSpeed = ui.autoreaderSpeed.value;

        if (DialogueSystem.instance == null)
            return;
        AutoReader autoReader = DialogueSystem.instance.autoReader;

        if (autoReader != null)
            autoReader.speed = config.dialogueAutoReadSpeed;
    }

    public void SetMusicVolume()
    {
        config.musicVolume = ui.musicVolume.value;

        AudioManager.instance.SetMusicVolume(config.musicVolume, config.musicMute);

        ui.musicFill.color = config.musicMute ? UI_Items.musicOffColor : UI_Items.musicOnColor;

    }
    public void SetSFXVolume()
    {
        config.sfxVolume = ui.sfxVolume.value;

        AudioManager.instance.SetSFXVolume(config.sfxVolume, config.sfxMute);

        ui.sfxFill.color = config.sfxMute ? UI_Items.musicOffColor : UI_Items.musicOnColor;
    }

    public void SetMusicMute()
    {
        config.musicMute = !config.musicMute;
        ui.musicVolume.fillRect.GetComponent<Image>().color = config.musicMute ? UI_Items.musicOffColor : UI_Items.musicOnColor;
        ui.musicMute.sprite = config.musicMute ? ui.mutedSymbol : ui.unmutedSymbol;

        AudioManager.instance.SetMusicVolume(config.musicVolume, config.musicMute);

    }
    public void SetSFXMute()
    {
        config.sfxMute = !config.sfxMute;
        ui.sfxVolume.fillRect.GetComponent<Image>().color = config.sfxMute ? UI_Items.musicOffColor : UI_Items.musicOnColor;
        ui.sfxMute.sprite = config.sfxMute ? ui.mutedSymbol : ui.unmutedSymbol;

        AudioManager.instance.SetSFXVolume(config.sfxVolume, config.sfxMute);
    }
}
