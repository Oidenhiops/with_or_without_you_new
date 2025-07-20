using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using AYellowpaper.SerializedCollections;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class Character : MonoBehaviour
{
    #region References
    public ManagementCharacterHud characterHud;
    public CharacterModelDirection characterModelDirection;
    public ManagementCharacterObjects characterObjects;
    public ManagementCharacterSkills characterSkills;
    public ManagementCharacterInteract characterInteract;
    public ManagementCharacterAttack characterAttack;
    public ManagementCharacterAnimations characterAnimations;
    public ManagementStatusEffect statusEffect;
    public CharacterMovement characterMove;
    public Dissolve dissolve;
    public Coroutine regenerateResources;
    public Coroutine hitStop;
    public Rigidbody rb;
    Collider[] hitColliders = new Collider[10];
    public event Action<bool> OnTakeDamage;
    #endregion
    public bool autoInit = false;
    public InitialDataSO initialData;
    public bool isPlayer = false;
    public bool _isActive;
    public Action<bool> OnIsActiveChange;
    public bool isActive
    {
        get => _isActive;
        set
        {
            if (_isActive != value)
            {
                _isActive = value;
                OnIsActiveChange?.Invoke(_isActive);
            }
        }
    }
    public bool isInitialize = false;
    public bool isGrounded => SetGrounded();
    public Color colorBlood = Color.white;
    public int level;
    public SerializedDictionary<TypeStatistics, Statistics> characterStatistics = new SerializedDictionary<TypeStatistics, Statistics>{
        {TypeStatistics.Hp, new Statistics (TypeStatistics.Hp, 0, new SerializedDictionary<StatusEffectSO.TypeStatusEffect, BuffStatistic>(), 0, 0, 0)},
        {TypeStatistics.Sp, new Statistics (TypeStatistics.Sp, 0, new SerializedDictionary<StatusEffectSO.TypeStatusEffect, BuffStatistic>(), 0, 0, 0)},
        {TypeStatistics.Mp, new Statistics (TypeStatistics.Mp, 0, new SerializedDictionary<StatusEffectSO.TypeStatusEffect, BuffStatistic>(), 0, 0, 0)},
        {TypeStatistics.Atk, new Statistics (TypeStatistics.Atk, 0, new SerializedDictionary<StatusEffectSO.TypeStatusEffect, BuffStatistic>(), 0, 0, 0)},
        {TypeStatistics.AtkSpd, new Statistics (TypeStatistics.AtkSpd, 0, new SerializedDictionary<StatusEffectSO.TypeStatusEffect, BuffStatistic>(), 0, 0, 0)},
        {TypeStatistics.Int, new Statistics (TypeStatistics.Int, 0, new SerializedDictionary<StatusEffectSO.TypeStatusEffect, BuffStatistic>(), 0, 0, 0)},
        {TypeStatistics.Def, new Statistics (TypeStatistics.Def, 0, new SerializedDictionary<StatusEffectSO.TypeStatusEffect, BuffStatistic>(), 0, 0, 0)},
        {TypeStatistics.Res, new Statistics (TypeStatistics.Res, 0, new SerializedDictionary<StatusEffectSO.TypeStatusEffect, BuffStatistic>(), 0, 0, 0)},
        {TypeStatistics.Spd, new Statistics (TypeStatistics.Spd, 0, new SerializedDictionary<StatusEffectSO.TypeStatusEffect, BuffStatistic>(), 0, 0, 0)},
        {TypeStatistics.Crit, new Statistics (TypeStatistics.Crit, 0, new SerializedDictionary<StatusEffectSO.TypeStatusEffect, BuffStatistic>(), 0, 0, 0)},
        {TypeStatistics.CritDmg, new Statistics (TypeStatistics.CritDmg, 0, new SerializedDictionary<StatusEffectSO.TypeStatusEffect, BuffStatistic>(), 0, 0, 0)},
    };
    void Awake()
    {
        if (autoInit) _ = InitializeCharacter();
    }
    public async Awaitable InitializeCharacter()
    {
        try
        {
            if (isPlayer && GameData.Instance.saveData.gameInfo.characterInfo.characterSelected != null)
            {
                initialData = GameData.Instance.saveData.gameInfo.characterInfo.characterSelected.initialDataSO.Clone();
                SetLevel(GameData.Instance.saveData.gameInfo.characterInfo.level);
            }
            else
            {
                initialData = GameData.Instance.charactersDBSO.GetRandomCharacter().Clone();
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
            isInitialize = true;
            if (!isPlayer)
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
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
            bool isCrit = false;
            int calculatedDamage;
            if (characterMakeDamage)
            {
                UnityEngine.Random.InitState(DateTime.Now.Millisecond);
                Statistics critValue = characterMakeDamage.GetStatisticByType(TypeStatistics.Crit);
                Statistics critDmg = characterMakeDamage.GetStatisticByType(TypeStatistics.CritDmg);
                isCrit = UnityEngine.Random.Range(0, 100) <= critValue.currentValue;
                float value = isCrit ? damage + damage * critDmg.currentValue / 100 : damage;
                calculatedDamage = (int)Mathf.Ceil(CalculateDamage(value, typeDamage));
            }
            else
            {
                calculatedDamage = (int)Mathf.Ceil(damage);
            }
            GameObject floatingText = Instantiate(Resources.Load<GameObject>("Prefabs/UI/FloatingText/FloatingText"), transform.position, Quaternion.identity);
            FloatingText floatingTextScript = floatingText.GetComponent<FloatingText>();
            _ = floatingTextScript.SendText(calculatedDamage.ToString(), color, isCrit);
            Destroy(floatingText, 2);
            GetStatisticByType(TypeStatistics.Hp).currentValue -= calculatedDamage;
            if (GetStatisticByType(TypeStatistics.Hp).currentValue <= 0)
            {
                GetStatisticByType(TypeStatistics.Hp).currentValue = 0;
                Die();
                OnTakeDamage?.Invoke(false);
            }
            else
            {
                OnTakeDamage?.Invoke(true);
                AudioManager.Instance.PlayASound(AudioManager.Instance.GetAudioClip("TakeDamage"), 1, true);
            }
            characterAnimations.MakeAnimation(CharacterAnimationsSO.TypeAnimation.None, "TakeDamage");
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
        var mp = GetStatisticByType(TypeStatistics.Mp);
        var sp = GetStatisticByType(TypeStatistics.Sp);
        while (GetStatisticByType(TypeStatistics.Hp).currentValue > 0)
        {
            yield return new WaitForSeconds(1);
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
        AudioManager.Instance.PlayASound(AudioManager.Instance.GetAudioClip(isPlayer ? "Die" : "DieEnemy"), 1, true);
        isActive = false;
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;
        GetComponent<Collider>().enabled = false;
        if (isPlayer)
        {
            StartCoroutine(ReloadScene());
        }
        else
        {
            characterHud.characterUi.generalHud.SetActive(false);
            Destroy(gameObject, 3);
        }
        OnIsActiveChange = null;
        characterAnimations.GetAnimation(CharacterAnimationsSO.TypeAnimation.None, "TakeDamage").loop = true;
        dissolve.DissolveObject();
        GameObject bloodInstance = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/BloodDieEffect/BloodDieEffect"), transform.position, Quaternion.identity);
        Destroy(bloodInstance, 10);
        bloodInstance.transform.position += Vector3.up / 2;
        var particleSystem = bloodInstance.GetComponent<ParticleSystem>();
        var mainModule = particleSystem.main;
        mainModule.startColor = colorBlood;
        StopCoroutine(regenerateResources);
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
                    initialData.characterStatistics[key].buffStatistic,
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
    public void SetLevel(int levelForSet)
    {
        level = levelForSet;
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
            float operationValue = basicsValue + GetBuffValue(characterStatistics[key]);
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
    float GetBuffValue(Statistics statistic)
    {
        float value = 0;
        foreach (var kv in statistic.buffStatistic)
        {
            value += kv.Value.value + kv.Value.valuePorcent * statistic.baseValue / 100;
        }
        return value;
    }
    IEnumerator ReloadScene()
    {
        yield return new WaitForSeconds(AudioManager.Instance.GetAudioClip("Die").length + 1);
        GameManager.Instance.ChangeSceneSelector(GameManager.TypeScene.HomeScene);
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
        public SerializedDictionary<StatusEffectSO.TypeStatusEffect, BuffStatistic> buffStatistic;
        public float objectValue = 0;
        public float currentValue = 0;
        public float maxValue = 0;
        public Statistics(TypeStatistics typeStatistics, float baseValue, SerializedDictionary<StatusEffectSO.TypeStatusEffect, BuffStatistic> buffStatistic, float objectValue, float currentValue, float maxValue)
        {
            this.typeStatistics = typeStatistics;
            this.baseValue = baseValue;
            this.buffStatistic = buffStatistic;
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
    [Serializable]
    public class BuffStatistic
    {
        public StatusEffectSO.TypeEffect typeEffect;
        public float value;
        public float valuePorcent;
        public BuffStatistic(StatusEffectSO.TypeEffect typeEffect, float value, float valuePorcent)
        {
            this.typeEffect = typeEffect;
            this.value = value;
            this.valuePorcent = valuePorcent;
        }
    }
}