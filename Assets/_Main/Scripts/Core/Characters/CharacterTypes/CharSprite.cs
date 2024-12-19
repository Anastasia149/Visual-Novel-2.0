using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Characters
{
    public class CharSprite : Character
    {
        private const string Sprite_Rendered_parent_Name = "Renderers";
        private CanvasGroup rootCG => root.GetComponent<CanvasGroup>();

        public List<CharacterSpriteLayer> layers = new List<CharacterSpriteLayer>();

        private string artAssetsDirectory = "";

        public override bool isVisible => isRevealing || rootCG.alpha == 1 ;
        public CharSprite(string name, CharacterConfigData config, GameObject prefab, string rootAssetsFolder) : base(name, config, prefab)
        {
            rootCG.alpha = Enable_on_Start ? 1 : 0;

            artAssetsDirectory = rootAssetsFolder + "/Images";

            GetLayers();

            Debug.Log($"Created Sprite Character: '{name}'");
        }

        private void GetLayers()
        {
            Transform rendererRoot = animator.transform.Find(Sprite_Rendered_parent_Name);

            if (rendererRoot == null)
                return;

            for (int i = 0; i < rendererRoot.transform.childCount; i++)
            {
                Transform child = rendererRoot.transform.GetChild(i);

                Image rendererImage = child.GetComponent<Image>();

                if (rendererImage != null )
                {
                    CharacterSpriteLayer layer = new CharacterSpriteLayer(rendererImage, i);
                    layers.Add(layer);
                    child.name = $"Layer: {i}";
                }
            }
        }

        public void SetSprite(Sprite sprite, int layer = 0)
        {
            layers[layer].SetSprite(sprite);
        }

        public Sprite GetSprite(string spriteName)
        {
            if (config.characterType == CharacterType.SpriteSheet)
                return null;
  
            else
                return Resources.Load<Sprite>($"{artAssetsDirectory}/{spriteName}");
            
        }
        #region Sprite transition (spritesheet)
        public Coroutine TransitionSprite(Sprite sprite, int layer = 0, float speed = 1)
        {
            CharacterSpriteLayer spriteLayer = layers[layer];

            return spriteLayer.TransitionSprite(sprite, speed);
        }

        public override IEnumerator ShowingOrHiding(bool show, float speedMultiplier = 1f)
        {
            float targetAlpha = show ? 1f : 0;
            CanvasGroup self = rootCG;

            while (self.alpha != targetAlpha)
            {
                self.alpha = Mathf.MoveTowards(self.alpha, targetAlpha, 3f * Time.deltaTime * speedMultiplier);
                yield return null;
            }

            co_revealing = null;
            co_hiding = null;
        }
        #endregion

        #region Sprite Color Changing
        public override void SetColor(Color color)
        {
            base.SetColor(color);

            color = displayColor;

            foreach (CharacterSpriteLayer layer in layers)
            {
                layer.StopChangingColor();
                layer.SetColor(color);
            }
        }
        public override IEnumerator ChangingColor(Color color, float speed)
        {
            foreach(CharacterSpriteLayer layer in layers)
                layer.TransitionColor(color, speed);

            yield return null;

            while(layers.Any(l => l.isChangingColor))
                yield return null;
            

            co_changingColor = null;
           
        }
        #endregion

        #region Character Highlighting
        public override IEnumerator Highlighting(float speedMultiplier, bool immediate = false)
        {
            Color targetColor = displayColor;

            foreach(CharacterSpriteLayer layer in layers)
            {
                if (immediate)
                    layer.SetColor(displayColor);
                else
                    layer.TransitionColor(targetColor, speedMultiplier);
            }
                

            yield return null;

            while (layers.Any(l => l.isChangingColor))
                yield return null;

            co_highlighting = null;
        }
        #endregion

        #region Character Flipping
        public override IEnumerator FaceDirection(bool faceLeft, float speedMultiplier, bool immediate)
        {
            foreach (CharacterSpriteLayer layer in layers)
            {
                if (faceLeft)
                    layer.FaceLeft(speedMultiplier, immediate);
                else
                    layer.FaceRight(speedMultiplier, immediate);
            }

            yield return null;

            while (layers.Any(l => l.isFlipping))
                yield return null;
            
            co_flipping = null;
        }
        #endregion

        #region Character Expression Changing
        public override void OnRecieveCastingExpression(int layer, string expression)
        {
            Sprite sprite = GetSprite(expression);

            if (sprite == null)
            {
                Debug.LogWarning($"Sprite '{expression}' could not be found for character '{name}'");
                return;
            }

            TransitionSprite(sprite, layer);
        }
        #endregion
    }
}