using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Characters
{ 
    public class CharacterSpriteLayer
    {

    #region Variables
        private CharacterManager characterManager => CharacterManager.instance;

        private const float defaultTransitionSpeed = 3f;
        private float transitionSpeedMultiplier = 1;
        public int layer { get; private set; } = 0;
        public Image renderer { get; private set; } = null;
        public CanvasGroup rendererCG => renderer.GetComponent<CanvasGroup>();

        private List<CanvasGroup> oldRenderers = new List<CanvasGroup>();

        private Coroutine co_transitioninglayer = null;
        private Coroutine co_levelingAlpha = null;
        private Coroutine co_changingColor = null;
        private Coroutine co_flipping = null;

        private bool isFacingLeft = Character.Default_Orientation;

        private bool isTransitioningLayer => co_transitioninglayer != null;
        private bool islevelingAlpha => co_levelingAlpha != null;
        public bool isChangingColor => co_changingColor != null;
        public bool isFlipping => co_flipping != null;
        #endregion

    #region Set sprite & render layer
        public CharacterSpriteLayer(Image defaultRenderer, int layer = 0) 
        {
            renderer = defaultRenderer;
            this.layer = layer;
        }

        public void SetSprite(Sprite sprite)
        {
            renderer.sprite = sprite;
        }
        #endregion

    #region Sprite Transitioning (spritesheet) ((unused))
        public Coroutine TransitionSprite(Sprite sprite, float speed = 1)
        {
            if (sprite = renderer.sprite)
                return null;

            if (isTransitioningLayer)
                characterManager.StopCoroutine(co_transitioninglayer);

            co_transitioninglayer = characterManager.StartCoroutine(TransitioningSprite(sprite, speed));

            return co_transitioninglayer;
        }

        public IEnumerator TransitioningSprite(Sprite sprite, float speedmultiplier) 
        {
            transitionSpeedMultiplier = speedmultiplier;

            Image newRenderer = CreateRenderer(renderer.transform.parent);
            newRenderer.sprite = sprite;

            yield return TryStartLevelingAlphas();

            co_transitioninglayer = null;
        }

        private Image CreateRenderer(Transform parent)
        {
            Image newRenderer = Object.Instantiate(renderer, parent);
            oldRenderers.Add(rendererCG);

            newRenderer.name = renderer.name;
            renderer = newRenderer;
            renderer.gameObject.SetActive(true);
            rendererCG.alpha = 0;

            return newRenderer;

        }

        private Coroutine TryStartLevelingAlphas()
        {
            if (islevelingAlpha)
                characterManager.StopCoroutine(co_levelingAlpha);

            co_levelingAlpha = characterManager.StartCoroutine(RunAlphaLeveling());

            return co_levelingAlpha;

        }

        private IEnumerator RunAlphaLeveling()
        {
            while (rendererCG.alpha < 1 || oldRenderers.Any(oldCG => oldCG.alpha > 0))
            {
                float speed = defaultTransitionSpeed * transitionSpeedMultiplier * Time.deltaTime;
                rendererCG.alpha = Mathf.MoveTowards(rendererCG.alpha, 1, speed);
            
                for (int i = oldRenderers.Count - 1; i >= 0; i--)
                {
                    CanvasGroup oldCG = oldRenderers[i];
                    oldCG.alpha = Mathf.MoveTowards(oldCG.alpha, 0, speed);

                    if (oldCG.alpha <= 0)
                    {
                        oldRenderers.RemoveAt(i);
                        Object.Destroy(oldCG.gameObject);
                    }
                }

                yield return null;
            }

            co_transitioninglayer = null;
        }
        #endregion

    #region Character color change
        public void SetColor(Color color)
        {
            renderer.color = color;

            foreach (CanvasGroup oldCG in oldRenderers)
            {
                oldCG.GetComponent<Image>().color = color;
            }
        }

        public Coroutine TransitionColor(Color color, float speed)
        {
            if (isChangingColor)
                characterManager.StopCoroutine(co_changingColor);

            co_changingColor = characterManager.StartCoroutine(ChangingColor(color, speed));

            return co_changingColor;

        }
        public void StopChangingColor()
        {
            if (!isChangingColor)
                return;

            characterManager.StopCoroutine(co_changingColor);

            co_changingColor = null;
        }

        private IEnumerator ChangingColor(Color color, float speedMultiplier)
        {
            Color oldColor = renderer.color;
            List<Image> oldImages = new List<Image>();

            foreach(var oldCG in oldRenderers)
                oldImages.Add(oldCG.GetComponent<Image>());
            

            float colorPercent = 0;
            while(colorPercent < 1)
            {
                colorPercent += defaultTransitionSpeed * speedMultiplier * Time.deltaTime;

                renderer.color = Color.Lerp(oldColor, color, colorPercent);

                for (int i = oldImages.Count - 1; i >= 0; i--)
                {
                    Image image = oldImages[i];
                    if (image != null)
                        image.color = renderer.color;
                    else
                        oldImages.RemoveAt(i);
                }
                yield return null;
            }

            co_changingColor = null;
        }
        #endregion

    #region Character Flipping
        public Coroutine Flip(float speed = 1, bool immediate = false)
        {
            if (isFacingLeft)
                return FaceRight(speed, immediate);
            else
                return FaceLeft(speed, immediate);
        }

        public Coroutine FaceLeft(float speed = 1, bool immediate = false)
        {
            if (isFlipping)
                characterManager.StopCoroutine(co_flipping);

            isFacingLeft = true;
            co_flipping = characterManager.StartCoroutine(FaceDirection(isFacingLeft, speed, immediate));

            return co_flipping;
        }

        public Coroutine FaceRight(float speed = 1, bool immediate = false)
        {
            if (isFlipping)
                characterManager.StopCoroutine(co_flipping);

            isFacingLeft = false;
            co_flipping = characterManager.StartCoroutine(FaceDirection(isFacingLeft, speed, immediate));

            return co_flipping;
        }

        private IEnumerator FaceDirection(bool faceLeft, float speedMultiplier, bool immediate)
        {
            float xScale = faceLeft ? 1 : -1;
            Vector3 newScale = new Vector3(xScale, 1, 1);

            if (!immediate)
            {
                Image newRenderer = CreateRenderer(renderer.transform.parent);

                newRenderer.transform.localScale = newScale;

                transitionSpeedMultiplier = speedMultiplier;
                TryStartLevelingAlphas();

                while (islevelingAlpha)
                    yield return null;

            }
            else
            {
                renderer.transform.localScale = newScale;
            }

            co_flipping = null;
        }
        #endregion
    }
}
