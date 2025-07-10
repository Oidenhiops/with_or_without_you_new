using System;
using System.Collections;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class Character : MonoBehaviour
{
    public bool autoInit;
    public GameManagerHelper gameManagerHelper;
    public CharacterInputs characterInputs;
    public CharacterInfo characterInfo;
    void Awake()
    {
        if (TryGetComponent<ICharacterMove>(out ICharacterMove iCharacterMove)) characterInfo.characterScripts.characterMove = iCharacterMove;
    }
    void Start()
    {
        if (autoInit) _ = InitializeCharacter();
    }
    void Update()
    {
        if (!GameManager.Instance.isPause)
        {
            if (characterInfo.isActive)
            {
                HandleHud();
                HandleAnimate();
                if (GameManager.Instance.startGame)
                {
                    HandleModelDirection();
                    HandleMove();
                    HandleAttack();
                    HandleObjects();
                    HandleSkills();
                    HandleInteract();
                }
            }
        }
    }
    public async Awaitable InitializeCharacter()
    {
        try
        {
            if (characterInfo.isPlayer)
            {
                if (GameData.Instance.saveData.gameInfo.characterInfo.characterSelected != null)
                {
                    characterInfo.initialData = GameData.Instance.saveData.gameInfo.characterInfo.characterSelected.Clone();
                }
                else
                {
                    characterInfo.initialData = characterInfo.initialData.Clone();
                }
            }

            await characterInfo.InitializeStatistics();
            await InitializeAnimations();
            await InitializeObjects();
            await InitializeSkills();
            await InitializeScriptsEvents();
            characterInfo.regenerateResources = StartCoroutine(characterInfo.RegenerateResources());

            if (characterInfo.isPlayer && characterInfo.characterScripts.managementCharacterHud != null)
            {
                characterInfo.characterScripts.managementCharacterHud.RefreshCurrentStatistics();
            }
            characterInfo.rb.isKinematic = false;
            characterInfo.isActive = true;
        }
        catch(Exception e)
        {
            Debug.LogError(e);
            await Awaitable.NextFrameAsync();
        }
    }
    async Awaitable InitializeAnimations()
    {
        try
        {
            characterInfo.characterScripts.characterAnimations.SetInitialData(characterInfo.initialData);
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
            if (characterInfo.characterScripts.managementCharacterObjects != null) characterInfo.characterScripts.managementCharacterObjects.InitializeObjects();
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
            if (characterInfo.isPlayer && characterInfo.characterScripts.managementCharacterSkills != null && GameData.Instance.saveData.gameInfo.characterInfo.characterSelected != null) characterInfo.characterScripts.managementCharacterSkills.InitializeSkills();
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
            if (characterInfo.characterScripts.managementCharacterHud) characterInfo.characterScripts.managementCharacterHud.InitializeHud();
            if (characterInfo.characterScripts.managementCharacterObjects) characterInfo.characterScripts.managementCharacterObjects.InitializeObjectsEvents();
            if (characterInfo.characterScripts.managementCharacterSkills) characterInfo.characterScripts.managementCharacterSkills.InitializeSkillsEvents();
            if (characterInfo.characterScripts.managementCharacterInteract) characterInfo.characterScripts.managementCharacterInteract.InitializeInteractsEvents();
            await Awaitable.NextFrameAsync();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            await Awaitable.NextFrameAsync();
        }
    }
    void HandleMove()
    {
        if (characterInfo.characterScripts.characterMove != null)
        {
            characterInfo.characterScripts.characterMove.Move();
        }
    }
    void HandleAttack()
    {
        characterInfo.characterScripts.characterAttack.ValidateAttack();
    }
    public void HandleAttackMobile()
    {
        characterInfo.characterScripts.characterAttack.ValidateAttackMobile();
    }
    void HandleObjects()
    {
        if (characterInfo.characterScripts.managementCharacterObjects) characterInfo.characterScripts.managementCharacterObjects.HandleObjects();
    }
    void HandleSkills()
    {
        if (characterInfo.characterScripts.managementCharacterSkills) characterInfo.characterScripts.managementCharacterSkills.HandleSkills();
    }
    void HandleHud()
    {
        if (characterInfo.characterScripts.managementCharacterHud) characterInfo.characterScripts.managementCharacterHud.HandleHud();
    }
    void HandleInteract()
    {
        if (characterInfo.isPlayer && characterInfo.characterScripts.managementCharacterInteract) characterInfo.characterScripts.managementCharacterInteract.Interact();
    }
    void HandleAnimate()
    {
        characterInfo.characterScripts.characterAnimations.Animate();
    }
    void HandleModelDirection()
    {
        characterInfo.characterScripts.managementCharacterModelDirection.ChangeModelDirection();
    }
    [Serializable] public class CharacterInfo
    {
        public CharacterScripts characterScripts;
        public InitialDataSO initialData;
        public Coroutine hitStop;
        public Coroutine regenerateResources;
        public bool isPlayer = false;
        public bool isActive = false;
        public Rigidbody rb;
        public bool isGrounded => SetGrounded();
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
        public Color colorBlood = Color.white;
        #region Functions
        public Statistics GetStatisticByType(TypeStatistics typeStatistics)
        {
            if (characterStatistics.TryGetValue(typeStatistics, out Statistics statistics))
            {
                return statistics;
            }
            return null;
        }
        public async Awaitable InitializeStatistics()
        {
            try
            {
                for (int i = 0; i < characterStatistics.Count; i++)
                {
                    TypeStatistics key = characterStatistics.Keys.ElementAt(i);
                    characterStatistics[key] = new Statistics(
                        initialData.characterInfo.characterStatistics[key].typeStatistics,
                        initialData.characterInfo.characterStatistics[key].baseValue,
                        initialData.characterInfo.characterStatistics[key].buffValue,
                        initialData.characterInfo.characterStatistics[key].objectValue,
                        initialData.characterInfo.characterStatistics[key].currentValue,
                        initialData.characterInfo.characterStatistics[key].maxValue
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
        public void TakeDamage(float damage, Color color, float timeHitStop, TypeDamage typeDamage, Character characterMakeDamage)
        {
            if (isActive)
            {
                int calculatedDamage;
                if (characterMakeDamage)
                {
                    UnityEngine.Random.InitState(DateTime.Now.Millisecond);
                    Statistics critValue = characterMakeDamage.characterInfo.GetStatisticByType(TypeStatistics.Crit);
                    Statistics critDmg = characterMakeDamage.characterInfo.GetStatisticByType(TypeStatistics.CritDmg);
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
                        characterScripts.owner.StopCoroutine(hitStop);
                    }
                    Time.timeScale = 0;
                    hitStop = characterScripts.owner.StartCoroutine(ApplyHitStop(timeHitStop));
                }
                GameObject floatingText = Instantiate(Resources.Load<GameObject>("Prefabs/UI/FloatingText/FloatingText"), characterScripts.owner.transform.position, Quaternion.identity);
                FloatingText floatingTextScript = floatingText.GetComponent<FloatingText>();
                _ = floatingTextScript.SendText(calculatedDamage.ToString(), color);
                Destroy(floatingText, 2);
                GetStatisticByType(TypeStatistics.Hp).currentValue -= calculatedDamage;
                if (GetStatisticByType(TypeStatistics.Hp).currentValue <= 0)
                {
                    GetStatisticByType(TypeStatistics.Hp).currentValue = 0;
                    Die();
                }
                characterScripts.characterAnimations.MakeAnimation(CharacterAnimationsSO.TypeAnimation.None, "TakeDamage");
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
        void Die()
        {
            AudioManager.Instance.PlayASound(AudioManager.Instance.GetAudioClip("Die"), 1, true);
            isActive = false;
            characterScripts.owner.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            characterScripts.owner.GetComponent<Rigidbody>().isKinematic = true;
            characterScripts.owner.GetComponent<Collider>().enabled = false;
            if (isPlayer)
            {
                characterScripts.owner.Invoke("ReloadScene", AudioManager.Instance.GetAudioClip("Die").length + 1);
            }
            characterScripts.characterAnimations.GetAnimation(CharacterAnimationsSO.TypeAnimation.None, "TakeDamage").loop = true;
            characterScripts.dissolve.DissolveObject();
            GameObject bloodInstance = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/BloodDieEffect/BloodDieEffect"), characterScripts.owner.transform.position, Quaternion.identity);
            bloodInstance.transform.position += Vector3.up / 2;
            var particleSystem = bloodInstance.GetComponent<ParticleSystem>();
            var mainModule = particleSystem.main;
            mainModule.startColor = colorBlood;
        }
        Collider[] hitColliders = new Collider[10];
        protected bool SetGrounded()
        {
            int hitCount = Physics.OverlapBoxNonAlloc(
                characterScripts.owner.transform.position,
                new Vector3(0.5f, 0.1f, 0.5f) / 2,
                hitColliders,
                Quaternion.identity,
                LayerMask.GetMask("Map")
            );
            return hitCount > 0;
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
        #endregion
    }
    void OnDrawGizmos()
    {
        Gizmos.color = characterInfo.isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(0.5f, 0.1f, 0.5f));
    }
    [Serializable] public class Statistics
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
    [Serializable] public class CharacterScripts
    {
        public ManagementCharacterHud managementCharacterHud;
        public ManagementCharacterModelDirection managementCharacterModelDirection;
        public ManagementCharacterObjects managementCharacterObjects;
        public ManagementCharacterSkills managementCharacterSkills;
        public ManagementPlayerCamera managementPlayerCamera;
        public ManagementCharacterInteract managementCharacterInteract;
        public ManagementCharacterAttack characterAttack;
        public ManagementCharacterAnimations characterAnimations;
        public ManagementStatusEffect managementStatusEffect;
        public Dissolve dissolve;
        public ICharacterMove characterMove;
        public MonoBehaviour owner;
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
    public interface ICharacterMove
    {
        public void Move();
        public Rigidbody GetRigidbody();
        public void SetPositionTarget(Transform position);
        public void SetCanMoveState(bool state);
        public void SetTarget(Transform targetPos);
        public Vector3 GetDirectionMove();
    }
}