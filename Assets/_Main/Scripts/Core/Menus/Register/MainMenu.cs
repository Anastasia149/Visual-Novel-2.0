using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using VisualNovel;

public class MainMenu : MonoBehaviour
{

    [ContextMenu("Play Game")]
    public void PlayGame()
    {
         SceneManager.LoadScene("Visual Novel");
    }

    [ContextMenu("Exit Game")]
    public void ExitGame()
    {
        Debug.Log("Игра закрылась");

#if UNITY_EDITOR
        // Завершение игры в редакторе
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Закрытие игры в собранной версии
        Application.Quit();
#endif
    }
}

