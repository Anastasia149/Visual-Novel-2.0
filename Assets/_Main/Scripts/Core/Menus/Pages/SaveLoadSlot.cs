using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using VisualNovel;

public class SaveLoadSlot : MonoBehaviour
{
    public GameObject root;
    public RawImage previewImage;
    public TextMeshProUGUI titleText;
    public Button deleteButton;
    public Button saveButton;
    public Button loadButton;

    [HideInInspector] public int fileNumber = 0;
    [HideInInspector] public string filePath = "";

    private UIConfirmationMenu uiChoiceMenu => UIConfirmationMenu.instance;

    public void PopulateDetails(SaveAndLoadPage.MenuFunction function)
    {
        if (File.Exists(filePath))
        {
            VNGameSave file = VNGameSave.Load(filePath);
            PopulateDetailsFromFile(function, file);
        }
        else
        {
            PopulateDetailsFromFile(function, null);
        }
    }

    private void PopulateDetailsFromFile(SaveAndLoadPage.MenuFunction function, VNGameSave file) 
    { 
        if (file == null)
        {
            titleText.text = $"{fileNumber}. Empty File";
            deleteButton.gameObject.SetActive(false);
            loadButton.gameObject.SetActive(false);
            saveButton.gameObject.SetActive(function == SaveAndLoadPage.MenuFunction.save);
            previewImage.texture = SaveAndLoadPage.Instance.emptyFileImage;
        }

        else
        {
            titleText.text = $"{fileNumber}. {file.timestamp}";
            deleteButton.gameObject.SetActive(true);
            loadButton.gameObject.SetActive(function == SaveAndLoadPage.MenuFunction.load);
            saveButton.gameObject.SetActive(function == SaveAndLoadPage.MenuFunction.save);

            byte[] data = File.ReadAllBytes(file.screenshotPath);
            Texture2D screenshotPreview = new Texture2D(1, 1);
            ImageConversion.LoadImage(screenshotPreview, data);
            previewImage.texture = screenshotPreview;
        }
    }

    public void Delete()
    {
        uiChoiceMenu.Show("удалить это сохранение?", new UIConfirmationMenu.ConfirmationButton("да", OnConfirmDelete), new UIConfirmationMenu.ConfirmationButton("нет", null));
    }

    private void OnConfirmDelete()
    {
        File.Delete(filePath);
        PopulateDetails(SaveAndLoadPage.Instance.menuFunction);
    }

    public void Load()
    {
        VNGameSave file = VNGameSave.Load(filePath, false);
        SaveAndLoadPage.Instance.Close(closeAllMenus: true);

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == _MainMenu.main_menu_scene)
            _MainMenu.instance.LoadGame(file);
        /*else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == _MainMenu.dorms)
            _MainMenu.instance.LoadGame(file);
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == _MainMenu.library)
            _MainMenu.instance.LoadGame(file);
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == _MainMenu.cafe)
            _MainMenu.instance.LoadGame(file);
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == _MainMenu.subway)
            _MainMenu.instance.LoadGame(file);
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == _MainMenu.subway)
            _MainMenu.instance.LoadGame(file);*/
        else
            file.Activate();
           
    }

    public void Save()
    {
        var activeSave = VNGameSave.activeFile;
        activeSave.slotNumber = fileNumber;

        activeSave.Save();

        PopulateDetailsFromFile(SaveAndLoadPage.Instance.menuFunction, activeSave);
    }
}
