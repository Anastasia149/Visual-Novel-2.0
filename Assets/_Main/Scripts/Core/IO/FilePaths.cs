using System.Globalization;
using UnityEngine;

public class FilePaths
{
    private const string home_directory_symbol = "~/";

    public static readonly string root = $"{Application.dataPath}/gameData/";

    //Runtime paths
    public static readonly string gameSaves = $"{runtimePath}Save Files/";

    // Resources paths
    public static readonly string resources_font = "Fonts/";

    public static readonly string resources_graphics = "Graphics/";
    public static readonly string resources_backgroundImages = $"{resources_graphics}BG Images/";
    public static readonly string resources_blendTextures = $"{resources_graphics}Transition Effects/";

    public static readonly string resources_audio = "Audio/";
    public static readonly string resources_sfx = $"{resources_audio}SFX/";
    public static readonly string resources_voices = $"{resources_audio}Voices/";
    public static readonly string resources_music = $"{resources_audio}Music/";
    public static readonly string resources_ambiance = $"{resources_audio}Ambiance/";

    public static readonly string resources_dialogueFiles = $"Dialogue Files/";

    public static string GetPathToResources(string defaultpath, string resourceName)
    {
        if (resourceName.StartsWith(home_directory_symbol)) 
            return resourceName.Substring(home_directory_symbol.Length);

        return defaultpath + resourceName;
    }

    public static string runtimePath
    {
        get
        {
            #if UNITY_EDITOR
                return "Assets/appdata/";
            #else
                return Application.persistentDataPath + "/appdata/";
            #endif
        }
    }
}
  