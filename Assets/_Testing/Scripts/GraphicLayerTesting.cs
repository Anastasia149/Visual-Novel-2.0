using Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicLayerTesting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Running());
    }

    IEnumerator Running()
    {
        GraphicPanel panel = GraphicPanelManager.instance.GetPanel("Background");
        GraphicLayer layer0 = panel.GetLayer(0, true);
        GraphicLayer layer1 = panel.GetLayer(1, true);

        layer0.SetTexture("Graphics/BG Images/2");
        layer1.SetTexture("Graphics/Bg Images/Spaceshipinterior");

        yield return new WaitForSeconds(1);

        GraphicPanel cinematic = GraphicPanelManager.instance.GetPanel("Cinematic");
        GraphicLayer cinLayer = cinematic.GetLayer(0, true);

        cinLayer.SetTexture("Graphics/Gallery/pup");

        yield return DialogueSystem.instance.Say("narrator", "guh");

        cinLayer.Clear();

        yield return new WaitForSeconds(5);

        panel.Clear();
    }
}
