using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VisualNovel;

public class _MainMenu : MonoBehaviour
{
    public const string main_menu_scene = "Main Menu";
    public const string dorms = "Dorms";
    public const string library = "Library";
    public const string cafe = "Cafe";
    public const string main = "MainSt";
    public const string subway = "Subway";

    public static _MainMenu instance { get; private set; }

    public AudioClip menuMusic;
    public CanvasGroup mainPanel;
    private CanvasGroupController mainCG;

    [SerializeField] UIConfirmationMenu uiChoiceMenu => UIConfirmationMenu.instance;

    void Start()
    {
       // if (Randomizer.firstTry == true)
       // {
       //     Debug.Log($"{Randomizer.listOfOptions.Count}guh");
       // }

        /*for (int i = 0; i < PlayerPrefs.GetInt("random_count"); i++)
        {
            Randomizer.listOfOptions[i] = PlayerPrefs.GetInt("options" + i);
            Debug.Log($"{Randomizer.listOfOptions[i]} - {PlayerPrefs.GetInt("options" + i)}");
            
        }

        for (int i = 0; i < 3; i++)
        {
            Debug.Log($"{PlayerPrefs.GetInt("options" + i)}");
        }*/
        mainCG = new CanvasGroupController(this, mainPanel);

        AudioManager.instance.StopAllSoundEffects();
        AudioManager.instance.StopAllTracks();
        AudioManager.instance.PlayTrack(menuMusic, channel: 0, startingVolume: 1);
    }

    void Awake()
    {
        instance = this;
    }

    public void Click_StartNewGame()
    {
        uiChoiceMenu.Show(
            // Title
            "начать новую игру?", 
            // Yes
            new UIConfirmationMenu.ConfirmationButton("да", StartNewGame), 
            // No
            new UIConfirmationMenu.ConfirmationButton("нет", null));
    } 

    private void StartNewGame()
    {
        Debug.Log("ya mama");
        VNGameSave.activeFile = new VNGameSave();
        StartCoroutine(StartingGame());
    }

    public void LoadGame(VNGameSave file)
    {
        //Randomizer.firstTry = (PlayerPrefs.GetInt("firstTry") != 0);
        VNGameSave.activeFile = file;
        StartCoroutine(StartingGame());
    }

    private IEnumerator StartingGame()
    {
        mainCG.Hide(speed: 0.3f);
        AudioManager.instance.StopTrack(0);

        while (mainCG.isVisible)
            yield return null;

        VN_Configuration.activeConfig.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Dorms");
    }

    private void Update()
    {
        /*if (Input.GetKeyDown("p"))
        {
            PlayerPrefs.SetInt("random_count", Randomizer.backup.Count);
            PlayerPrefs.Save();
            for (int i = 0; i < Randomizer.backup.Count; i++)
            {
                PlayerPrefs.SetInt("options" + i, Randomizer.backup[i]);
                PlayerPrefs.Save();
            }
        }*/
    }
}
