using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterAnimations", menuName = "ScriptableObjects/Character/CharacterAnimationsSO", order = 1)]
public class CharacterAnimationsSO : ScriptableObject
{
    public CharacterAnimationsInfo animationsInfo;
    public NewAnimationInfo newAnimationInfo;
    [NaughtyAttributes.Button] public void NewAnimation()
    {
        AnimationsInfo appendAnimation = new AnimationsInfo();
        appendAnimation.typeAnimation = newAnimationInfo.typeAnimation;
        appendAnimation.animationName = newAnimationInfo.animationName;
        appendAnimation.linkAnimation = newAnimationInfo.linkAnimation;
        appendAnimation.spritesInfoDown = new SpritesInfo[newAnimationInfo.spriteDown.Length];
        appendAnimation.spritesInfoUp = new SpritesInfo[newAnimationInfo.spriteUp.Length];
        appendAnimation.animationsEffects = newAnimationInfo.animationsEffects;
        appendAnimation.needAnimationEnd = newAnimationInfo.needAnimationEnd;
        appendAnimation.loop = newAnimationInfo.loop;
        appendAnimation.needInstance = newAnimationInfo.needInstance;
        appendAnimation.frameToInstance = newAnimationInfo.frameToInstance;
        appendAnimation.speedSpritesPerTimeMultplier = newAnimationInfo.speedSpritesPerTimeMultplier;
        appendAnimation.instanceObj = newAnimationInfo.instanceObj;
        appendAnimation.instance = newAnimationInfo.instance;
        for (int i = 0; i < appendAnimation.spritesInfoDown.Length; i++)
        {
            appendAnimation.spritesInfoDown[i] = new SpritesInfo();
            appendAnimation.spritesInfoUp[i] = new SpritesInfo();
            appendAnimation.spritesInfoDown[i].generalSprite = newAnimationInfo.spriteDown[i];
            appendAnimation.spritesInfoDown[i].handSprite = newAnimationInfo.handSpriteDown;
            appendAnimation.spritesInfoUp[i].generalSprite = newAnimationInfo.spriteUp[i];
            appendAnimation.spritesInfoUp[i].handSprite = newAnimationInfo.handSpriteUp;
        }
        List<AnimationsInfo> currentAnimationsList = animationsInfo.animations.ToList();
        currentAnimationsList.Add(appendAnimation);
        animationsInfo.animations = currentAnimationsList.ToArray();
        newAnimationInfo = new NewAnimationInfo();
    }
    [System.Serializable]   public class CharacterAnimationsInfo
    {
        public float baseSpritePerTime = 0.1f;
        public float currentSpritePerTime = 0.1f;
        public int currentSpriteIndex = 0;
        public AnimationsInfo[] animations;
    }
    [System.Serializable]   public class AnimationsInfo
    {
        public TypeAnimation typeAnimation;
        public string animationName;
        public string linkAnimation;
        public SpritesInfo[] spritesInfoDown;
        public SpritesInfo[] spritesInfoUp;
        public ManagementCharacterAnimations.TypeAnimationsEffects[] animationsEffects;
        public bool needAnimationEnd = false;
        public bool loop = false;
        public bool needInstance = false;
        public int frameToInstance = 0;
        public float speedSpritesPerTimeMultplier = 1;
        public GameObject instanceObj;
        public GameObject instance;
    }
    [System.Serializable]   public class HandTransformInfo
    {
        public Vector3 pos;
        public Quaternion rotation;
    }
    [System.Serializable]   public class SpritesInfo
    {
        public Sprite generalSprite;
        public Sprite handSprite;
        public Vector3 leftHandPosDL;
        public Vector3 leftHandPosDR;
        public Quaternion leftHandRotation;
        public Vector3 rightHandPosDL;
        public Vector3 rightHandPosDR;
        public Quaternion rightHandRotation;
    }
    [System.Serializable]   public class NewAnimationInfo
    {
        public TypeAnimation typeAnimation;
        public string animationName;
        public string linkAnimation;
        public Sprite[] spriteDown;
        public Sprite handSpriteDown;
        public Sprite[] spriteUp;
        public Sprite handSpriteUp;
        public ManagementCharacterAnimations.TypeAnimationsEffects[] animationsEffects;
        public bool needAnimationEnd = false;
        public bool loop = false;
        public bool needInstance = false;
        public int frameToInstance = 0;
        public float speedSpritesPerTimeMultplier = 1;
        public GameObject instanceObj;
        public GameObject instance;
    }
    public enum TypeAnimation
    {
        None = 0,
        Attack = 1,
        Skill = 2
    }
}
