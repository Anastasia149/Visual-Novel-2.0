using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Commands;

namespace History
{
    [System.Serializable]
    public class GraphicData
    {
        public string panelName;
        public List<LayerData> layers;

        [System.Serializable]
        public class LayerData
        {
            public int depth = 0;
            public string graphicName;
            public string graphicPath;

            public LayerData(GraphicLayer layer)
            {
                depth = layer.layerDepth;

                if (layer.currentGraphic == null)
                    return;

                var graphic = layer.currentGraphic;

                graphicName = graphic.graphicName;
                graphicPath = graphic.graphicPath;
            }
        }

        public static List<GraphicData> Capture()
        {
            List<GraphicData> graphicPanels = new List<GraphicData>();

            foreach (var panel in GraphicPanelManager.instance.allPanels)
            {
                if (panel.isClear)
                    continue;

                GraphicData data = new GraphicData();

                data.panelName = panel.panelName;
                data.layers = new List<LayerData>();

                foreach (var layer in panel.layers)
                {
                    LayerData entry = new LayerData(layer);
                    data.layers.Add(entry);
                }

                graphicPanels.Add(data);
            }

            return graphicPanels;
        }

        public static void Apply(List<GraphicData> data)
        {
            List<string> cache = new List<string>();

            foreach (var panelData in data)
            {
                var panel = GraphicPanelManager.instance.GetPanel(panelData.panelName);

                foreach (var layerData in panelData.layers)
                {
                    var layer = panel.GetLayer(layerData.depth, createIfDoesNotExist: true);
                    if (layer.currentGraphic == null || layer.currentGraphic.graphicName != layerData.graphicName)
                    {
                        Texture tex = HistoryCache.LoadImage(layerData.graphicName);
                        if (tex == null)
                        {
                            // Загрузка из новой папки Resources
                            string pathToGraphic = GetPathToGraphic(layerData.graphicName);      
                            tex = Resources.Load<Texture>(pathToGraphic);
                        }

                        if (tex != null)
                            layer.SetTexture(tex, filePath: layerData.graphicPath, immediate: true);
                        else
                            Debug.LogWarning($"History State: could not load image from path '{layerData.graphicPath}'.");
                    }
                }

                cache.Add(panel.panelName);
            }

            foreach (var panel in GraphicPanelManager.instance.allPanels)
            {
                if (!cache.Contains(panel.panelName))
                    panel.Clear(immediate: true);
            }
        }

        private static string GetPathToGraphic(string graphicName)
        {
            // Формируем путь к файлу в папке Assets/_MAIN/Resources
            return $"_MAIN/Resources/{graphicName}";
        }
    }
}
