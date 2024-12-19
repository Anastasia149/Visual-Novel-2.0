using UnityEngine;

public class GraphicPanelManager : MonoBehaviour
{
    public static GraphicPanelManager instance { get; private set; }

    public const float Default_Transition_Speed = 3f;

    [field:SerializeField] public GraphicPanel[] allPanels { get; private set; }



    private void Awake()
    {
        instance = this;
    }

    public GraphicPanel GetPanel(string name)
    {
        name = name.ToLower();

        foreach (var panel in allPanels)
        {
            if (panel.panelName.ToLower() == name)
                return panel;
        }

        return null;
    }
}
