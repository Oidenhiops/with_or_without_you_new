using System.Collections;
using UnityEngine;

public class ManagementCharacterAnimations : MonoBehaviour
{
    [SerializeField] Character character;
    [SerializeField] Mesh originalMesh;
    [SerializeField] GameObject characterSprite;
    [SerializeField] MeshRenderer meshRendererCharacter;
    [SerializeField] MeshRenderer meshRendererHand;
    public CharacterAnimationsSO.CharacterAnimationsInfo characterAnimationsInfo;
    public CharacterAnimationsSO.AnimationsInfo currentAnimation = new CharacterAnimationsSO.AnimationsInfo();
    public GameObject leftHand;
    public GameObject rightHand;
    [SerializeField] CharacterAnimationsEffectsInfo characterAnimationsEffectsInfo;
    public bool isUp = false;
    public void Update()
    {
        if (character.isActive && GameManager.Instance.startGame)
        {
            if (!currentAnimation.needAnimationEnd)
            {
                if (character.characterModelDirection.movementCharacter != Vector2.zero)
                {
                    if (!GetCurrentAnimation("Walk"))
                    {
                        MakeAnimation(CharacterAnimationsSO.TypeAnimation.None, "Walk");
                    }
                }
                else
                {
                    if (!GetCurrentAnimation("Idle"))
                    {
                        MakeAnimation(CharacterAnimationsSO.TypeAnimation.None, "Idle");
                    }
                }
            }
            if (meshRendererHand && meshRendererHand.gameObject.activeSelf)
            {
                SetHandsPos();
            }
        }
    }
    public void SetInitialData(InitialDataSO initialData)
    {
        StopAllCoroutines();
        characterSprite.transform.parent.transform.localScale = Vector3.one * GetScaleFactor(initialData.characterAnimations.animationsInfo.animations[0].spritesInfoDown[0].generalSprite.rect.width);
        meshRendererCharacter.material.SetTexture("_BaseTexture", initialData.atlas);
        if (meshRendererHand && initialData.atlasHand){
            meshRendererHand.gameObject.SetActive(true);
            meshRendererHand.material.SetTexture("_BaseTexture", initialData.atlasHand);
        }
        else {
            if (meshRendererHand){
                meshRendererHand.gameObject.SetActive(false);
            }
        }
        characterAnimationsInfo = new CharacterAnimationsSO.CharacterAnimationsInfo
        {
            animations = initialData.characterAnimations.animationsInfo.animations,
            baseSpritePerTime = initialData.characterAnimations.animationsInfo.baseSpritePerTime,
            currentSpritePerTime = initialData.characterAnimations.animationsInfo.currentSpritePerTime,
            currentSpriteIndex = 0
        };
        GetAnimation(CharacterAnimationsSO.TypeAnimation.None, "GeneralAttack").speedSpritesPerTimeMultplier = character.GetStatisticByType(Character.TypeStatistics.AtkSpd).currentValue;
        currentAnimation = GetAnimation(CharacterAnimationsSO.TypeAnimation.None, "Idle");
        StartCoroutine(AnimateSprite());
    }
    float GetScaleFactor(float size)
    {
        float baseScale = 64f;
        return size / baseScale;
    }
    void MakeAnimationEffect(TypeAnimationsEffects typeAnimationsEffects, float duration)
    {
        string nombreMetodo = typeAnimationsEffects.ToString();
        var metodo = GetType().GetMethod(nombreMetodo, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        if (metodo != null)
        {
            if (metodo.ReturnType == typeof(IEnumerator))
            {
                StartCoroutine((IEnumerator)metodo.Invoke(this, new object[] { duration }));
            }
            else
            {
                metodo.Invoke(this, new object[] { duration });
            }
        }
    }
    AnimationEffectInfo GetAnimationEffectInfo(TypeAnimationsEffects typeAnimationsEffects)
    {
        foreach (AnimationEffectInfo animationInfo in characterAnimationsEffectsInfo.animationEffectsInfo)
        {
            if (animationInfo.typeAnimationsEffects == typeAnimationsEffects) return animationInfo;
        }
        return null;
    }
    void SetTextureFromAtlas(Sprite spriteFromAtlas, MeshRenderer meshRenderer)
    {
        Vector2[] uvs = originalMesh.uv;
        Texture2D texture = spriteFromAtlas.texture;
        meshRenderer.material.mainTexture = texture;
        Rect spriteRect = spriteFromAtlas.rect;
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i].x = Mathf.Lerp(spriteRect.x / texture.width, (spriteRect.x + spriteRect.width) / texture.width, uvs[i].x);
            uvs[i].y = Mathf.Lerp(spriteRect.y / texture.height, (spriteRect.y + spriteRect.height) / texture.height, uvs[i].y);
        }
        meshRenderer.GetComponent<MeshFilter>().mesh.uv = uvs;
    }
    bool GetCurrentAnimation(string typeAnimation)
    {
        return currentAnimation.animationName == typeAnimation;
    }
    public bool ValidateAnimationEnd(string typeAnimation)
    {
        return currentAnimation.animationName != typeAnimation;
    }
    public void MakeAnimation(CharacterAnimationsSO.TypeAnimation typeAnimation, string animationName)
    {
        StopAllCoroutines();
        currentAnimation = GetAnimation(typeAnimation, animationName);
        if (typeAnimation == CharacterAnimationsSO.TypeAnimation.Attack)
        {
            currentAnimation.speedSpritesPerTimeMultplier = character.GetStatisticByType(Character.TypeStatistics.AtkSpd).currentValue;
        }
        characterAnimationsInfo.currentSpritePerTime = characterAnimationsInfo.baseSpritePerTime / currentAnimation.speedSpritesPerTimeMultplier;
        characterAnimationsInfo.currentSpriteIndex = 0;
        StartCoroutine(AnimateSprite());
    }
    public GameObject GetCharacterSprite()
    {
        return characterSprite;
    }
    public CharacterAnimationsSO.AnimationsInfo GetAnimation(CharacterAnimationsSO.TypeAnimation typeAnimation, string animationName)
    {
        switch (typeAnimation)
        {
            case CharacterAnimationsSO.TypeAnimation.None:
                foreach (CharacterAnimationsSO.AnimationsInfo animation in characterAnimationsInfo.animations)
                {
                    if (animation.animationName == animationName)
                    {
                        return animation;
                    }
                }
                break;
            default:
                return AnimationExist(typeAnimation, animationName);
        }
        return null;
    }
    public CharacterAnimationsSO.AnimationsInfo AnimationExist(CharacterAnimationsSO.TypeAnimation typeAnimation, string animationName)
    {
        CharacterAnimationsSO.AnimationsInfo defaultAnimation = new CharacterAnimationsSO.AnimationsInfo();
        switch (typeAnimation)
        {
            case CharacterAnimationsSO.TypeAnimation.Attack:

                foreach (CharacterAnimationsSO.AnimationsInfo animation in characterAnimationsInfo.animations)
                {
                    if (animation.animationName == "GeneralAttack") defaultAnimation = animation;                    
                    if (animation.animationName == animationName) return animation;
                }
                break;
            case CharacterAnimationsSO.TypeAnimation.Skill:
                foreach (CharacterAnimationsSO.AnimationsInfo animation in characterAnimationsInfo.animations)
                {
                    if (animation.animationName == "DefaultSkillAttack") defaultAnimation = animation;
                    if (animation.animationName == animationName) return animation;
                }
                break;
        }
        return defaultAnimation;
    }
    public CharacterAnimationsSO.AnimationsInfo GetCurrentAnimation()
    {
        return currentAnimation;
    }
    public CharacterAnimationsSO.CharacterAnimationsInfo GetAnimationsInfo()
    {
        return characterAnimationsInfo;
    }
    #region AnimationsEffects
    IEnumerator Shake(float duration)
    {
        float tiempoTranscurrido = 0f;
        Vector3 initialPos = characterSprite.transform.localPosition;
        AnimationEffectInfo effectInfo = GetAnimationEffectInfo(TypeAnimationsEffects.Shake);

        while (tiempoTranscurrido < duration)
        {
            float desplazamientoX = Mathf.Sin(Time.time * effectInfo.frequency) * effectInfo.amplitude;
            characterSprite.transform.localPosition = initialPos + new Vector3(desplazamientoX, 0, 0);
            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }
        initialPos.x = 0f;
        characterSprite.transform.localPosition = initialPos;
    }
    IEnumerator Blink(float duration)
    {
        float tiempoTranscurrido = 0f;
        Material material = characterSprite.GetComponent<MeshRenderer>().material;
        AnimationEffectInfo effectInfo = GetAnimationEffectInfo(TypeAnimationsEffects.Blink);
        duration = characterAnimationsInfo.currentSpritePerTime * GetAnimation(CharacterAnimationsSO.TypeAnimation.None, "TakeDamage").spritesInfoUp.Length;
        while (tiempoTranscurrido < duration)
        {
            if (material.color == Color.white)
            {
                material.SetColor("_Color", effectInfo.colorBlink);
            }
            else
            {
                material.SetColor("_Color", Color.white);
            }
            tiempoTranscurrido += characterAnimationsInfo.currentSpritePerTime;
            yield return new WaitForSeconds(characterAnimationsInfo.currentSpritePerTime);
        }
        material.SetColor("_Color", Color.white);
    }
    #endregion
    IEnumerator AnimateSprite()
    {
        if (currentAnimation.animationsEffects.Length > 0)
        {
            for (int i = 0; i < currentAnimation.animationsEffects.Length; i++)
            {
                MakeAnimationEffect(currentAnimation.animationsEffects[i], characterAnimationsInfo.currentSpritePerTime * currentAnimation.spritesInfoUp.Length);
            }
        }
        while (true)
        {
            isUp = character.characterModelDirection.movementDirectionAnimation.y > 0;
            SetTextureFromAtlas(
                isUp ? 
                    currentAnimation.spritesInfoUp[characterAnimationsInfo.currentSpriteIndex].generalSprite :
                    currentAnimation.spritesInfoDown[characterAnimationsInfo.currentSpriteIndex].generalSprite,
                meshRendererCharacter
            );
            yield return new WaitForSeconds(characterAnimationsInfo.currentSpritePerTime);
            characterAnimationsInfo.currentSpriteIndex++;
            if (characterAnimationsInfo.currentSpriteIndex > currentAnimation.spritesInfoUp.Length - 1){
                if (currentAnimation.loop)
                {
                    characterAnimationsInfo.currentSpriteIndex = 0;
                }
                else
                {
                    if (currentAnimation.linkAnimation != "")
                    {
                        MakeAnimation(currentAnimation.typeAnimation, currentAnimation.linkAnimation);
                    }
                    else
                    {
                        MakeAnimation(CharacterAnimationsSO.TypeAnimation.None, "Idle");
                    }
                }
            }
        }
    }
    void SetHandsPos()
    {
        if (meshRendererHand)
        {
            meshRendererHand.gameObject.SetActive(true);
            if (characterAnimationsInfo.currentSpriteIndex < currentAnimation.spritesInfoUp.Length &&
                    currentAnimation.spritesInfoUp.Length > 0 &&
                    currentAnimation.spritesInfoUp[0].handSprite)
            {
                SetTextureFromAtlas(
                    isUp ?
                        currentAnimation.spritesInfoUp[characterAnimationsInfo.currentSpriteIndex].generalSprite :
                        currentAnimation.spritesInfoDown[characterAnimationsInfo.currentSpriteIndex].generalSprite,
                    meshRendererHand
                );
                switch (isUp)
                {
                    case true:
                        Vector3 spriteLeftUpPos = character.characterModelDirection.movementDirectionAnimation.x > 0 ?
                                                    currentAnimation.spritesInfoUp[characterAnimationsInfo.currentSpriteIndex].leftHandPosDR :
                                                    currentAnimation.spritesInfoUp[characterAnimationsInfo.currentSpriteIndex].leftHandPosDL;
                        Vector3 spriteRightUpPos = character.characterModelDirection.movementDirectionAnimation.x > 0 ?
                                                    currentAnimation.spritesInfoUp[characterAnimationsInfo.currentSpriteIndex].rightHandPosDR :
                                                    currentAnimation.spritesInfoUp[characterAnimationsInfo.currentSpriteIndex].rightHandPosDL;
                        leftHand.transform.localPosition = spriteLeftUpPos;
                        leftHand.transform.localRotation = currentAnimation.spritesInfoUp[characterAnimationsInfo.currentSpriteIndex].leftHandRotation;
                        rightHand.transform.localPosition = spriteRightUpPos;
                        rightHand.transform.localRotation = currentAnimation.spritesInfoUp[characterAnimationsInfo.currentSpriteIndex].rightHandRotation;
                        break;
                    case false:
                        Vector3 spriteLeftDownPos = character.characterModelDirection.movementDirectionAnimation.x > 0 ?
                                                    currentAnimation.spritesInfoDown[characterAnimationsInfo.currentSpriteIndex].leftHandPosDR :
                                                    currentAnimation.spritesInfoDown[characterAnimationsInfo.currentSpriteIndex].leftHandPosDL;
                        Vector3 spriteRightDownPos = character.characterModelDirection.movementDirectionAnimation.x > 0 ?
                                                    currentAnimation.spritesInfoDown[characterAnimationsInfo.currentSpriteIndex].rightHandPosDR :
                                                    currentAnimation.spritesInfoDown[characterAnimationsInfo.currentSpriteIndex].rightHandPosDL;
                        leftHand.transform.localPosition = spriteLeftDownPos;
                        leftHand.transform.localRotation = currentAnimation.spritesInfoDown[characterAnimationsInfo.currentSpriteIndex].leftHandRotation;
                        rightHand.transform.localPosition = spriteRightDownPos;
                        rightHand.transform.localRotation = currentAnimation.spritesInfoDown[characterAnimationsInfo.currentSpriteIndex].rightHandRotation;
                        break;
                }
            }
        }
    }
    IEnumerator ValidateFrameToInstance(int frame)
    {
        while (characterAnimationsInfo.currentSpriteIndex != frame)
        {
            yield return null;
        }
        currentAnimation.instance = Instantiate(currentAnimation.instanceObj, characterSprite.transform.position, Quaternion.identity, characterSprite.transform);
        currentAnimation.instance.transform.SetParent(characterSprite.transform);
        currentAnimation.instance.transform.localPosition = Vector3.zero;
        currentAnimation.instance.transform.SetParent(transform);
        currentAnimation.instance.transform.localRotation = Quaternion.Euler(0, 0, 0);
        currentAnimation.instance.transform.GetChild(0).transform.localRotation = characterSprite.transform.localRotation;
        currentAnimation.instance.GetComponent<IAnimationInstance>().SetInfoForAnimation(character.characterModelDirection.movementDirectionAnimation, characterAnimationsInfo);
    }
    [System.Serializable] public class CharacterAnimationsEffectsInfo
    {
        public AnimationEffectInfo[] animationEffectsInfo = new AnimationEffectInfo[]
        {
            new AnimationEffectInfo()
            {
                typeAnimationsEffects = TypeAnimationsEffects.Shake,
                amplitude = 0.1f,
                frequency = 100.0f,
            },
            new AnimationEffectInfo()
            {
                typeAnimationsEffects = TypeAnimationsEffects.Blink,
                amplitude = 0.1f,
                frequency = 100.0f,
            }
        };
    }
    [System.Serializable] public class AnimationEffectInfo
    {
        public TypeAnimationsEffects typeAnimationsEffects;
        public float amplitude = 0;
        public float frequency = 0;
        public Color colorBlink = Color.white;
    }
    public enum TypeAnimationsEffects
    {
        None = 0,
        Shake = 1,
        Blink = 2
    }
    public interface IAnimationInstance
    {
        public void SetInfoForAnimation(Vector2 movement, CharacterAnimationsSO.CharacterAnimationsInfo characterAnimationsInfo);
    }
}
