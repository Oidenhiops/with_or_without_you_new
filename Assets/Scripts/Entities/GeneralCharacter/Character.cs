using System;
using System.Collections;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class Character : MonoBehaviour
{
    #region References
    public ManagementCharacterHud characterHud;
    public PlayerModelDirection characterModelDirection;
    public ManagementCharacterObjects characterObjects;
    public ManagementCharacterSkills characterSkills;
    public ManagementCharacterInteract characterInteract;
    public ManagementCharacterAttack characterAttack;
    public ManagementCharacterAnimations characterAnimations;
    public ManagementStatusEffect statusEffect;
    public CharacterMovement characterMove;
    public GameManagerHelper gameManagerHelper;
    public Dissolve dissolve;
    public Coroutine regenerateResources;
    public Coroutine hitStop;
    public Rigidbody rb;
    Collider[] hitColliders = new Collider[10];
    #endregion
    public bool autoInit = false;
    public InitialDataSO initialData;
    public bool isPlayer = false;
    public bool isActive = false;
    public bool isGrounded => SetGrounded();
    public Color colorBlood = Color.white;
    public SerializedDictionary<TypeStatistics, Statistics> characterStatistics = new SerializedDictionary<TypeStatistics, Statistics>{
        {TypeStatistics.Hp, new Statistics (TypeStatistics.Hp, 0, 0, 0, 0, 0)},
        {TypeStatistics.Sp, new Statistics (TypeStatistics.Sp, 0, 0, 0, 0, 0)},
        {TypeStatistics.Mp, new Statistics (TypeStatistics.Mp, 0, 0, 0, 0, 0)},
        {TypeStatistics.Atk, new Statistics (TypeStatistics.Atk, 0, 0, 0, 0, 0)},
        {TypeStatistics.AtkSpd, new Statistics (TypeStatistics.AtkSpd, 0, 0, 0, 0, 0)},
        {TypeStatistics.Int, new Statistics (TypeStatistics.Int, 0, 0, 0, 0, 0)},
        {TypeStatistics.Def, new Statistics (TypeStatistics.Def, 0, 0, 0, 0, 0)},
        {TypeStatistics.Res, new Statistics (TypeStatistics.Res, 0, 0, 0, 0, 0)},
        {TypeStatistics.Spd, new Statistics (TypeStatistics.Spd, 0, 0, 0, 0, 0)},
        {TypeStatistics.Crit, new Statistics (TypeStatistics.Crit, 0, 0, 0, 0, 0)},
        {TypeStatistics.CritDmg, new Statistics (TypeStatistics.CritDmg, 0, 0, 0, 0, 0)},
    };
    void Awake()
    {
        if (autoInit) _ = InitializeCharacter();
    }
    public async Awaitable InitializeCharacter()
    {
        try
        {
            if (isPlayer)
            {
                if (GameData.Instance.saveData.gameInfo.characterInfo.characterSelected != null)
                {
                    initialData = GameData.Instance.saveData.gameInfo.characterInfo.characterSelected.Clone();
                }
                else
                {
                    initialData = initialData.Clone();
                }
            }

            await InitializeStatistics();
            await InitializeAnimations();
            await InitializeObjects();
            await InitializeSkills();
            await InitializeScriptsEvents();
            regenerateResources = StartCoroutine(RegenerateResources());

            if (isPlayer && characterHud != null)
            {
                characterHud.RefreshCurrentStatistics();
            }
            rb.isKinematic = false;
            isActive = true;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            await Awaitable.NextFrameAsync();
        }
    }
    async Awaitable InitializeAnimations()
    {
        try
        {
            characterAnimations.SetInitialData(initialData);
            await Awaitable.NextFrameAsync();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            await Awaitable.NextFrameAsync();
        }
    }
    async Awaitable InitializeObjects()
    {
        try
        {
            if (characterObjects != null) characterObjects.InitializeObjects();
            await Awaitable.NextFrameAsync();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            await Awaitable.NextFrameAsync();
        }
    }
    async Awaitable InitializeSkills()
    {
        try
        {
            if (isPlayer && characterSkills != null && GameData.Instance.saveData.gameInfo.characterInfo.characterSelected != null) characterSkills.InitializeSkills();
            await Awaitable.NextFrameAsync();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            await Awaitable.NextFrameAsync();
        }
    }
    async Awaitable InitializeScriptsEvents()
    {
        try
        {
            if (characterHud) characterHud.InitializeHud();
            if (characterObjects) characterObjects.InitializeObjectsEvents();
            if (characterSkills) characterSkills.InitializeSkillsEvents();
            if (characterInteract) characterInteract.InitializeInteractsEvents();
            await Awaitable.NextFrameAsync();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            await Awaitable.NextFrameAsync();
        }
    }
    public void TakeDamage(float damage, Color color, float timeHitStop, TypeDamage typeDamage, Character characterMakeDamage)
    {
        if (isActive)
        {
            int calculatedDamage;
            if (characterMakeDamage)
            {
                UnityEngine.Random.InitState(DateTime.Now.Millisecond);
                Statistics critValue = characterMakeDamage.GetStatisticByType(TypeStatistics.Crit);
                Statistics critDmg = characterMakeDamage.GetStatisticByType(TypeStatistics.CritDmg);
                bool isCrit = UnityEngine.Random.Range(0, 100) <= critValue.currentValue;
                float value = isCrit ? damage + damage * critDmg.currentValue : damage;
                calculatedDamage = (int)Mathf.Ceil(CalculateDamage(value, typeDamage));
            }
            else
            {
                calculatedDamage = (int)Mathf.Ceil(damage);
            }

            if (timeHitStop > 0)
            {
                if (hitStop != null)
                {
                    StopCoroutine(hitStop);
                }
                Time.timeScale = 0;
                hitStop = StartCoroutine(ApplyHitStop(timeHitStop));
            }
            GameObject floatingText = Instantiate(Resources.Load<GameObject>("Prefabs/UI/FloatingText/FloatingText"), transform.position, Quaternion.identity);
            FloatingText floatingTextScript = floatingText.GetComponent<FloatingText>();
            _ = floatingTextScript.SendText(calculatedDamage.ToString(), color);
            Destroy(floatingText, 2);
            GetStatisticByType(TypeStatistics.Hp).currentValue -= calculatedDamage;
            if (GetStatisticByType(TypeStatistics.Hp).currentValue <= 0)
            {
                GetStatisticByType(TypeStatistics.Hp).currentValue = 0;
                Die();
            }
            characterAnimations.MakeAnimation(CharacterAnimationsSO.TypeAnimation.None, "TakeDamage");
            AudioManager.Instance.PlayASound(AudioManager.Instance.GetAudioClip("TakeDamage"), 1, true);
        }
    }
    float CalculateDamage(float damage, TypeDamage typeDamage)
    {
        if (typeDamage == TypeDamage.TrueDamage)
            return damage;

        TypeStatistics targetStat = typeDamage == TypeDamage.Physic ? TypeStatistics.Def : TypeStatistics.Res;
        Statistics def = GetStatisticByType(targetStat);
        float reduction = def.currentValue / (def.currentValue + 200);
        return damage * (1 - reduction);
    }
    public IEnumerator RegenerateResources()
    {
        var hp = GetStatisticByType(TypeStatistics.Hp);
        var mp = GetStatisticByType(TypeStatistics.Mp);
        var sp = GetStatisticByType(TypeStatistics.Sp);
        while (hp.currentValue > 0)
        {
            yield return new WaitForSeconds(1);
            if (hp.currentValue < hp.maxValue)
            {
                hp.currentValue += (int)Mathf.Ceil(hp.baseValue * 0.001f);
                if (hp.currentValue > hp.maxValue)
                {
                    hp.currentValue = hp.maxValue;
                }
            }
            if (mp.currentValue < mp.maxValue)
            {
                mp.currentValue += (int)Mathf.Ceil(mp.baseValue * 0.01f);
                if (mp.currentValue > mp.maxValue)
                {
                    mp.currentValue = mp.maxValue;
                }
            }
            if (sp.currentValue < sp.maxValue)
            {
                sp.currentValue += (int)Mathf.Ceil(sp.baseValue * 0.05f);
                if (sp.currentValue > sp.maxValue)
                {
                    sp.currentValue = sp.maxValue;
                }
            }
        }
    }
    IEnumerator ApplyHitStop(float timeHitStop)
    {
        yield return new WaitForSecondsRealtime(timeHitStop);
        Time.timeScale = 1;
    }
    void Die()
    {
        AudioManager.Instance.PlayASound(AudioManager.Instance.GetAudioClip("Die"), 1, true);
        isActive = false;
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;
        GetComponent<Collider>().enabled = false;
        if (isPlayer)
        {
            Invoke("ReloadScene", AudioManager.Instance.GetAudioClip("Die").length + 1);
        }
        characterAnimations.GetAnimation(CharacterAnimationsSO.TypeAnimation.None, "TakeDamage").loop = true;
        dissolve.DissolveObject();
        GameObject bloodInstance = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/BloodDieEffect/BloodDieEffect"), transform.position, Quaternion.identity);
        bloodInstance.transform.position += Vector3.up / 2;
        var particleSystem = bloodInstance.GetComponent<ParticleSystem>();
        var mainModule = particleSystem.main;
        mainModule.startColor = colorBlood;
    }
    public async Awaitable InitializeStatistics()
    {
        try
        {
            for (int i = 0; i < characterStatistics.Count; i++)
            {
                TypeStatistics key = characterStatistics.Keys.ElementAt(i);
                characterStatistics[key] = new Statistics(
                    initialData.characterStatistics[key].typeStatistics,
                    initialData.characterStatistics[key].baseValue,
                    initialData.characterStatistics[key].buffValue,
                    initialData.characterStatistics[key].objectValue,
                    initialData.characterStatistics[key].currentValue,
                    initialData.characterStatistics[key].maxValue
                );
                if (characterStatistics[key].maxValue == 0)
                {
                    characterStatistics[key].maxValue = characterStatistics[key].baseValue;
                    characterStatistics[key].currentValue = characterStatistics[key].baseValue;
                }
            }
            await Awaitable.NextFrameAsync();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            await Awaitable.NextFrameAsync();
        }
    }
    public Statistics GetStatisticByType(TypeStatistics typeStatistics)
    {
        if (characterStatistics.TryGetValue(typeStatistics, out Statistics statistics))
        {
            return statistics;
        }
        return null;
    }
    public void RefreshCurrentStatistics()
    {
        foreach (var key in characterStatistics.Keys.ToList())
        {
            float basicsValue = characterStatistics[key].baseValue + characterStatistics[key].objectValue;
            float operationValue = basicsValue + basicsValue * characterStatistics[key].buffValue / 100;
            float finalValue = characterStatistics[key].typeStatistics == TypeStatistics.AtkSpd ? operationValue : Mathf.Ceil(operationValue);
            characterStatistics[key].maxValue = finalValue;
            if (characterStatistics[key].typeStatistics != TypeStatistics.Hp && characterStatistics[key].typeStatistics != TypeStatistics.Mp && characterStatistics[key].typeStatistics != TypeStatistics.Sp)
            {
                characterStatistics[key].currentValue = finalValue;
            }
            else if (characterStatistics[key].currentValue > characterStatistics[key].maxValue)
            {
                characterStatistics[key].currentValue = characterStatistics[key].maxValue;
            }
        }
    }
    protected bool SetGrounded()
    {
        int hitCount = Physics.OverlapBoxNonAlloc(
            transform.position,
            new Vector3(0.5f, 0.1f, 0.5f) / 2,
            hitColliders,
            Quaternion.identity,
            LayerMask.GetMask("Map")
        );
        return hitCount > 0;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(0.5f, 0.1f, 0.5f));
    }
    [Serializable]
    public class Statistics
    {
        public TypeStatistics typeStatistics;
        public float baseValue = 0;
        public float buffValue = 0;
        public float objectValue = 0;
        public float currentValue = 0;
        public float maxValue = 0;
        public Statistics(TypeStatistics typeStatistics, float baseValue, float buffValue, float objectValue, float currentValue, float maxValue)
        {
            this.typeStatistics = typeStatistics;
            this.baseValue = baseValue;
            this.buffValue = buffValue;
            this.objectValue = objectValue;
            this.currentValue = currentValue;
            this.maxValue = maxValue;
        }
    }
    public enum TypeStatistics
    {
        None = 0,
        Hp = 1,
        Mp = 2,
        Sp = 3,
        Atk = 4,
        AtkSpd = 5,
        Int = 6,
        Def = 7,
        Res = 8,
        Spd = 9,
        Crit = 10,
        CritDmg = 11,
    }
    public enum TypeDamage
    {
        None = 0,
        Physic = 1,
        Magic = 2,
        TrueDamage = 3,
    }
}