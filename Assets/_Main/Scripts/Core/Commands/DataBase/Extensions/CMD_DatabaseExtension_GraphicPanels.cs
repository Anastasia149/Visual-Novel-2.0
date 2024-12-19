using System;
using System.Collections;
using UnityEngine;

namespace Commands
{
    public class CMD_DatabaseExtension_GraphicPanels : CMD_DatabaseExtension
    {
        #region Param identifiers
        private static string[] param_panel = new string[] { "-p", "-panel" };
        private static string[] param_layer = new string[] { "-l", "-layer" };
        private static string[] param_media = new string[] { "-m", "-media" };
        private static string[] param_speed = new string[] { "-spd", "-speed" };
        private static string[] param_immediate = new string[] { "-i", "-immediate" };
        private static string[] param_blendtex = new string[] { "-b", "-blend" };

        private const string home_directory_symbol = "~/";
        #endregion

        #region Extend
        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("setlayermedia", new Func<string[], IEnumerator>(SetLayerMedia));
            database.AddCommand("clearlayermedia", new Func<string[], IEnumerator>(ClearLayerMedia));
        }
        #endregion

        #region Set layer Media
        private static IEnumerator SetLayerMedia(string[] data)
        {
            // parameters available to function
            string panelName = "";
            int layer = 0;
            string mediaName = "";
            float transitionSpeed = 0;
            bool immediate = false;
            string blendTexName = "";

            string pathToGraphic = "";
            UnityEngine.Object graphic = null;
            Texture blendTex = null;

            // Get the parameters
            var parameters = ConvertDataToParameters(data);

            // try to get the panel that this media is applied to
            parameters.TryGetValue(param_panel, out panelName);
            GraphicPanel panel = GraphicPanelManager.instance.GetPanel(panelName);
            if (panel == null)
            {
                Debug.LogError($"Unable to grab panel '{panelName}' because it is not a valid panel please check the panel name and adjust the command");
                yield break;
            }

            // Try to get the layer to apply to this graphic to
            parameters.TryGetValue(param_layer, out layer, defaultValue: 0);

            // Try to get the graphic
            parameters.TryGetValue(param_media, out mediaName);

            // Try to get if this is an immediate effect or not
            parameters.TryGetValue(param_immediate, out immediate, defaultValue: false);

            // Try to get the soeed of the transition if it is not an immediate effect
            if (!immediate)
                parameters.TryGetValue(param_speed, out transitionSpeed, defaultValue: 1);

            // Try to get the blending texture for the media is we are using one
            parameters.TryGetValue(param_blendtex, out blendTexName);

            //pathToGraphic = FilePaths.GetPathToResources(FilePaths.resources_backgroundImages, mediaName);        May be dupes i dunno
            //graphic = Resources.Load<Texture>(pathToGraphic);

            // Run logic
            pathToGraphic = GetpathToGraphic(FilePaths.resources_backgroundImages, mediaName);
            graphic = Resources.Load<Texture>(pathToGraphic);

            if (graphic == null)
            {
                Debug.LogError($"Could not find the media called '{mediaName}' in the Resources directories. Please specify the full path within resources and make sure that the file exists.");
                yield break;
            }

            if (!immediate && blendTexName != string.Empty)
                blendTex = Resources.Load<Texture>(FilePaths.resources_blendTextures + blendTexName);

            // Try to get the layer
            GraphicLayer graphicLayer = panel.GetLayer(layer, createIfDoesNotExist: true);

            if (graphic is Texture)
            {
                if (!immediate)
                    CommandManager.instance.AddTerminationActionToCurrentProcess(() => { graphicLayer?.SetTexture(graphic as Texture, filePath: pathToGraphic, immediate: true); });

                yield return graphicLayer.SetTexture(graphic as Texture, transitionSpeed, blendTex, pathToGraphic, immediate);
            }

        }
        #endregion

        #region Clear layer media
        private static IEnumerator ClearLayerMedia(string[] data)
        {
            // parameters available to function
            string panelName = "";
            int layer = 0;
            float transitionSpeed = 0;
            bool immediate = false;
            string blendTexName = "";

            Texture blendTex = null;

            // Get the parameters
            var parameters = ConvertDataToParameters(data);

            // try to get the panel that this media is applied to
            parameters.TryGetValue(param_panel, out panelName);
            GraphicPanel panel = GraphicPanelManager.instance.GetPanel(panelName);
            if (panel == null)
            {
                Debug.LogError($"Unable to grab panel '{panelName}' because it is not a valid panel please check the panel name and adjust the command");
                yield break;
            }

            // Try to get the layer to apply to this graphic to
            parameters.TryGetValue(param_layer, out layer, defaultValue: -1);

            // Try to get if this is an immediate effect or not
            parameters.TryGetValue(param_immediate, out immediate, defaultValue: false);

            // Try to get the soeed of the transition if it is not an immediate effect
            if (!immediate)
                parameters.TryGetValue(param_speed, out transitionSpeed, defaultValue: 1);

            // Try to get the blending texture for the media is we are using one
            parameters.TryGetValue(param_blendtex, out blendTexName);

            if (!immediate && blendTexName != string.Empty)
                blendTex = Resources.Load<Texture>(FilePaths.resources_blendTextures + blendTexName);

            if (layer == -1)
                panel.Clear(transitionSpeed, blendTex, immediate);
            else
            {
                GraphicLayer graphicLayer = panel.GetLayer(layer);
                if (graphicLayer == null)
                {
                    Debug.LogError($"Could not clear layer [{layer}] on panel '{panel.panelName}'");
                    yield break;
                }
                graphicLayer.Clear(transitionSpeed, blendTex, immediate);
            }
        }
        #endregion

        public static string GetpathToGraphic(string defaultPath, string graphicName)
        {
            if (graphicName.StartsWith(home_directory_symbol))
                return graphicName.Substring(home_directory_symbol.Length);
            
            return defaultPath + graphicName;
        }
    }
}
