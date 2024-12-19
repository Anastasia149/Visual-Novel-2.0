using Dialogue;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;

namespace Characters 
{
    public abstract class Character
    {

        #region Variables / Character characterManager / Dialogue system
        public const bool Enable_on_Start = false;
        private const float Unhighlighted_Darken_Strength = 0.65f;
        public const bool Default_Orientation = true;
        public const string Animation_Refresh_Trigger = "Refresh";

        public string name = "";
        public string displayName = "";
        public RectTransform root = null;
        public CharacterConfigData config;
        public Animator animator;
        public Color color { get; protected set; } = Color.white;
        protected Color displayColor => highlighted ? highlightedColor : unhighlightedColor;
        protected Color highlightedColor => color;
        protected Color unhighlightedColor => new Color(color.r * Unhighlighted_Darken_Strength, color.g * Unhighlighted_Darken_Strength, color.b * Unhighlighted_Darken_Strength, color.a);
        public bool highlighted { get; protected set; } = true;
        protected bool facingLeft = Default_Orientation;
        public int priority { get; protected set; }

        public Vector2 targetPosition { get; private set; }

        protected CharacterManager characterManager => CharacterManager.instance;

        public DialogueSystem dialogueSystem => DialogueSystem.instance;
        #endregion

        #region Coroutines
        protected Coroutine co_revealing, co_hiding;
        protected Coroutine co_moving;
        protected Coroutine co_changingColor;
        protected Coroutine co_highlighting;
        protected Coroutine co_flipping;

        // Showing / hiding characters

        public bool isRevealing => co_revealing != null;
        public bool isHiding => co_hiding != null;

        // Moving characters
      
        public bool isMoving => co_moving != null;

        // Change character colors
        
        public bool isChangingColor => co_changingColor != null;

        // Highlighting characters

       
        public bool isHighlighting => (highlighted && co_highlighting != null);
        public bool isUnHighlighting => (!highlighted && co_highlighting != null);

        // Character visibility
        public virtual bool isVisible { get; set; }

        // Flipping characters
        
        public bool isFacingLeft => facingLeft;
        public bool isFacingRight => !facingLeft;
        public bool isFlipping => co_flipping != null;
        #endregion

        #region Constructor
        public Character(string name, CharacterConfigData config, GameObject prefab)
        {
            this.name = name;
            displayName = name;
            this.config = config;

            if (prefab != null)
            {
                GameObject ob = Object.Instantiate(prefab, characterManager.characterPanel);
                ob.name = characterManager.FormatCharacterPath(characterManager.characterPrefabNameFormat, name);
                ob.SetActive(true);
                root = ob.GetComponent<RectTransform>();
                animator = root.GetComponentInChildren<Animator>();
            }
        }
        #endregion

        #region Dialogue - Character text appearance / Display name
        // makes dialogue a list
        public Coroutine Say(string dialogue) => Say(new List<string> { dialogue });

        public Coroutine Say(List<string> dialogue)
        {
            dialogueSystem.ShowSpeakerName(name);
            UpdateTextCustomizationsOnScreen();
            return dialogueSystem.Say(dialogue);
        }

        public void SetNameColor(Color color) => config.nameColor = color;

        public void SetDialogueColor(Color color) => config.dialogueColor = color;

        public void SetNameFont(TMP_FontAsset font) => config.nameFont = font;

        public void SetDialogueFont(TMP_FontAsset font) => config.dialogueFont = font;

        public void ResetConfigurationData() => config = CharacterManager.instance.GetCharacterConfig(name);

        public void UpdateTextCustomizationsOnScreen() => dialogueSystem.ApplySpeakerDataToDialogueContainer(config);
        #endregion

        #region Visibility Coroutines
        public virtual Coroutine Show(float speedMultiplier = 1f)
        {
            if (isRevealing)
                characterManager.StopCoroutine(co_revealing);

            if (isHiding)
                characterManager.StopCoroutine(co_hiding);

            co_revealing = characterManager.StartCoroutine(ShowingOrHiding(true, speedMultiplier));

            return co_revealing;
        }

        public virtual Coroutine Hide(float speedMultiplier = 1f)
        {
            if (isHiding)
                characterManager.StopCoroutine(co_hiding);

            if (isRevealing)
                characterManager.StopCoroutine(co_revealing);

            co_hiding = characterManager.StartCoroutine(ShowingOrHiding(false, speedMultiplier));

            return co_hiding;
        }
        public virtual IEnumerator ShowingOrHiding(bool show, float speedMultiplier)
        {
            Debug.Log("Show/Hide can not be called from a base character type.");
            yield return null;
        }
        #endregion

        #region Sprite Movement
        public virtual void SetPosition(Vector2 position)
        {
            if (root == null)
                return;

            (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConvertUITargetPositionToRelativeCharacterAnchorTargets(position);

            root.anchorMin = minAnchorTarget;
            root.anchorMax = maxAnchorTarget;

            targetPosition = position;
        }

        public virtual Coroutine MoveToPostion(Vector2 position, float speed = 2f, bool smooth = false)
        {
            if (root == null)
                return null;

            if (isMoving)
                characterManager.StopCoroutine(co_moving);

            co_moving = characterManager.StartCoroutine(MovingToPosition(position, speed, smooth));

            targetPosition = position;

            return co_moving;
        }

        private IEnumerator MovingToPosition(Vector2 position, float speed, bool smooth)
        {
            (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConvertUITargetPositionToRelativeCharacterAnchorTargets(position);
            Vector2 padding = root.anchorMax - root.anchorMin;

            while (root.anchorMin != minAnchorTarget || root.anchorMax != maxAnchorTarget)
            {
                root.anchorMin = smooth ?
                    Vector2.Lerp(root.anchorMin, minAnchorTarget, speed * Time.deltaTime)
                    : Vector2.MoveTowards(root.anchorMin, minAnchorTarget, speed * Time.deltaTime * 0.35f);

                root.anchorMax = root.anchorMin + padding;

                if (smooth && Vector2.Distance(root.anchorMin, minAnchorTarget) <= 0.001f)
                {
                    root.anchorMin = minAnchorTarget;
                    root.anchorMax = maxAnchorTarget;
                    break;
                }

                yield return null;
            }

            co_moving = null;
        }

        protected (Vector2, Vector2) ConvertUITargetPositionToRelativeCharacterAnchorTargets(Vector2 position)
        {
            Vector2 padding = root.anchorMax - root.anchorMin;

            float maxX = 1f - padding.x;
            float maxY = 1f - padding.y;

            Vector2 minAnchorTarget = new Vector2(maxX * position.x, maxY * position.y);
            Vector2 maxAnchorTarget = minAnchorTarget + padding;

            return (minAnchorTarget, maxAnchorTarget);
        }
        #endregion

        #region Character color change
        public virtual void SetColor (Color color)
        {
            this.color = color;
        }

        public Coroutine TransitionColor(Color color, float speed = 1f)
        {
            this.color = color;

            if (isChangingColor)
                characterManager.StopCoroutine(co_changingColor);

            co_changingColor = characterManager.StartCoroutine(ChangingColor(displayColor,speed));

            return co_changingColor;

        }

        public virtual IEnumerator ChangingColor(Color color, float speed)
        {
            Debug.Log("Color changing is not applicable on this character type");
            yield return null;
        }
        #endregion

        #region Character Highlighting
        public Coroutine Highlight(float speed = 1f, bool immediate = false)
        {
            if (isHighlighting || isUnHighlighting)
                characterManager.StopCoroutine(co_highlighting);

            highlighted = true;
            co_highlighting = characterManager.StartCoroutine(Highlighting(speed, immediate));

            return co_highlighting;
        }
        public Coroutine UnHighlight(float speed = 1f, bool immediate = false)
        {
            if (isHighlighting || isUnHighlighting)
                characterManager.StopCoroutine(co_highlighting);

            highlighted = false;
            co_highlighting = characterManager.StartCoroutine(Highlighting(speed, immediate));

            return co_highlighting;
        }

        public virtual IEnumerator Highlighting(float speedMultiplier, bool immediate = false)
        {
            Debug.Log("highlighting is not available on this character type!");
            yield return null;
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

            facingLeft = true;
            co_flipping = characterManager.StartCoroutine(FaceDirection(facingLeft, speed, immediate));
        
            return co_flipping;
        }

        public Coroutine FaceRight(float speed = 1, bool immediate = false)
        {
            if (isFlipping)
                characterManager.StopCoroutine(co_flipping);

            facingLeft = false;
            co_flipping = characterManager.StartCoroutine(FaceDirection(facingLeft, speed, immediate));

            return co_flipping;
        }

        public virtual IEnumerator FaceDirection(bool faceLeft, float speedMultiplier, bool immediate)
        {
            Debug.Log("Cannot flip a character of this type");
            yield return null;
        }
        #endregion

        #region Character Priority
        public void SetPriority(int priority, bool autoSortCharactersOnUI = true)
        {
            this.priority = priority;

            if (autoSortCharactersOnUI)
                characterManager.SortCharacters();
        }
        #endregion

        #region Character Animation

        // Trigger anim state
        public Coroutine Animate(string animation)
        {
            animator.SetTrigger(animation);

            //return Animate(animation);
            return null;
        }

        // Bool anim state
        public Coroutine Animate(string animation, bool state)
        {
            animator.SetBool(animation, state);
            animator.SetTrigger(Animation_Refresh_Trigger);

            //return Animate(animation, state);
            return null;
        }
        #endregion

        #region Character Expression

        public virtual void OnSort(int sortingindex)
        {
            return;
        }

        public virtual void OnRecieveCastingExpression(int layer, string expression)
        {
            return;
        }
        #endregion

        #region Char config data
        public enum CharacterType
        {
            Text,
            Sprite,
            SpriteSheet
        }
        #endregion
    }
}
