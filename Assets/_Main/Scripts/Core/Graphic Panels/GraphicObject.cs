using System.Collections;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GraphicObject
{
    #region const string
    private const string name_format = " Graphic - [{0}]";
    private const string default_ui_material = "Default UI Material";
    private const string material_path = "Materials/layerTransitionMaterial";
    private const string material_field_color =         "_Color";
    private const string material_field_maintex =       "_MainTex";
    private const string material_field_blendtex =      "_BlendTex";
    private const string material_field_blend =         "_Blend";
    private const string material_field_alpha =         "_Alpha";
    #endregion

    #region variables
    public RawImage renderer;

    private GraphicLayer layer;

    public bool isVideo { get { return video != null; } }
    public VideoPlayer video = null;
    public AudioSource audio = null;

    public string graphicPath = "";
    public string graphicName { get; private set; }

    private Coroutine co_fadingIn = null;
    private Coroutine co_fadingOut = null;

    #endregion

    #region constructor
    public GraphicObject(GraphicLayer layer, string graphicPath, Texture tex, bool immediate)
    {
        this.graphicPath = graphicPath;
        this.layer = layer;

        GameObject ob = new GameObject();
        ob.transform.SetParent(layer.panel);
        renderer = ob.AddComponent<RawImage>();

        graphicName = tex.name;

        InitGraphic(immediate);

        renderer.name = string.Format(name_format, graphicName);
        renderer.material.SetTexture(material_field_maintex, tex);
    }
    #endregion

    #region initializes graphic
    private void InitGraphic(bool immediate)
    {
        renderer.transform.localPosition = Vector3.zero;
        renderer.transform.localScale = Vector3.one;

        RectTransform rect = renderer.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.one;

        renderer.material = GetTransitionMaterial();

        float startingOpacity = immediate ? 1.0f : 0.0f;

        renderer.material.SetFloat(material_field_blend, startingOpacity);
        renderer.material.SetFloat(material_field_alpha, startingOpacity);
    }
    #endregion

    #region Obtains material
    private Material GetTransitionMaterial()
    {
        Material mat = Resources.Load<Material>(material_path);

        if (mat != null)
            return new Material(mat);

        return null;
    }
    #endregion

    #region Fading
    GraphicPanelManager panelManager => GraphicPanelManager.instance;
    public Coroutine FadeIn(float speed = 1f, Texture blend = null)
    {
        if (co_fadingOut != null)
            panelManager.StopCoroutine(co_fadingOut);

        if (co_fadingIn != null)
            return co_fadingIn;

        co_fadingIn = panelManager.StartCoroutine(Fading(1f, speed, blend));

        return co_fadingIn;
    }

    public Coroutine FadeOut(float speed = 1f, Texture blend = null)
    {
        if (co_fadingIn != null)
            panelManager.StopCoroutine(co_fadingIn);

        if (co_fadingOut != null)
            return co_fadingOut;

        co_fadingOut = panelManager.StartCoroutine(Fading(0f, speed, blend));

        return co_fadingOut;
    }

    private IEnumerator Fading(float target, float speed, Texture blend)
    {
        bool isBlending = blend != null;
        bool fadingIn = target > 0;

        if (renderer.material.name == default_ui_material)
        {
            Texture tex = renderer.material.GetTexture(material_field_maintex);
            renderer.material = GetTransitionMaterial();
            renderer.material.SetTexture(material_field_maintex, tex);
        }

        renderer.material.SetTexture(material_field_blendtex, blend);
        renderer.material.SetFloat(material_field_alpha, isBlending ? 1 : fadingIn ? 0 : 1);
        renderer.material.SetFloat(material_field_blend, isBlending ? fadingIn ? 0 : 1 : 1);


        string opacityParam = isBlending ? material_field_blend : material_field_alpha;

        while (renderer.material.GetFloat(opacityParam) != target)
        {
            float opacity = Mathf.MoveTowards(renderer.material.GetFloat(opacityParam), target, speed * GraphicPanelManager.Default_Transition_Speed * Time.deltaTime);
            renderer.material.SetFloat(opacityParam, opacity);
            yield return null;
        }

        co_fadingIn = null;
        co_fadingOut = null;

        if (target == 0)
            Destroy();

        else
        {
            DestroyBackgroundGraphicOnLayer();
            renderer.texture = renderer.material.GetTexture(material_field_maintex);
            renderer.material = null;
        }
    }

    #endregion

    #region Destroy graphic
    public void Destroy()
    {
        if (layer.currentGraphic != null && layer.currentGraphic.renderer == renderer)
            layer.currentGraphic = null;

        if (layer.oldGraphics.Contains(this))
            layer.oldGraphics.Remove(this);

        Object.Destroy(renderer.gameObject);
    }

    private void DestroyBackgroundGraphicOnLayer()
    {
        layer.DestroyOldGraphics();
    }
    #endregion


}
