using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterInputs : MonoBehaviour
{
    public Character character;
    public CharacterActions characterActions;
    public CharacterActionsInfo characterActionsInfo;
    public GameObject attackDirection;
    private float timeRestoreMovementMouse = 1f;
    public float restoreMovementMouse = 0;
    void OnEnable()
    {
        characterActions.Enable();
    }
    void OnDisable()
    {
        characterActions.Disable();
    }
    void Awake()
    {
        characterActions = new CharacterActions();
        InitInputs();
    }
    void Update()
    {
        ValidateShowDirection();
    }
    void InitInputs()
    {
        characterActions.CharacterInputs.MousePos.performed += OnMouseInput;
        characterActions.CharacterInputs.MousePos.canceled += OnMouseInput;
        characterActions.CharacterInputs.Movement.performed += OnMovementInput;
        characterActions.CharacterInputs.Movement.canceled += OnMovementInput;
        characterActions.CharacterInputs.MoveCamera.performed += OnMoveCamera;
        characterActions.CharacterInputs.ActiveSkill.performed += OnActiveSkill;
        characterActions.CharacterInputs.ActiveSkill.canceled += OnActiveSkill;
        characterActions.CharacterInputs.Pause.performed += OnPauseInput;
        characterActions.CharacterInputs.SecondaryAction.started += OnEnableSecondaryAction;        
        character.characterInputs.characterActions.CharacterInputs.ShowStats.started += OnShowStats;
    }
    void OnActiveSkill(InputAction.CallbackContext context)
    {
        if (context.action.IsPressed())
        {
            characterActionsInfo.isSkillsActive = true;
        }
        else
        {
            characterActionsInfo.isSkillsActive = false;
        }
    }
    void OnMovementInput(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        if (MathF.Abs(value.x) > 0.1f || MathF.Abs(value.y) > 0.1f)
        {
            characterActionsInfo.movement = value;
        }
        else
        {
            characterActionsInfo.movement = Vector2.zero;
        }
    }
    void OnPauseInput(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.startGame)
        {
            character.gameManagerHelper.ChangeScene(1);
        }
    }
    void OnMoveCamera(InputAction.CallbackContext context)
    {
        restoreMovementMouse = timeRestoreMovementMouse;
        if (GameManager.Instance.currentDevice == GameManager.TypeDevice.PC)
        {
            characterActionsInfo.moveCamera = GetMouseDirection().normalized;
        }
        else
        {
            characterActionsInfo.moveCamera = context.ReadValue<Vector2>();
        }
    }
    void OnMouseInput(InputAction.CallbackContext context)
    {
        characterActionsInfo.mousePos = context.ReadValue<Vector2>();
    }
    void OnEnableSecondaryAction(InputAction.CallbackContext context)
    {
        if (!characterActionsInfo.isSkillsActive)
        {
            characterActionsInfo.isSecondaryAction = !characterActionsInfo.isSecondaryAction;
            character.characterInfo.characterScripts.managementCharacterHud.ToggleSecondaryAction(characterActionsInfo.isSecondaryAction);
        }
    }
    void OnShowStats(InputAction.CallbackContext context)
    {
        characterActionsInfo.isShowStats = !characterActionsInfo.isShowStats;
        character.characterInfo.characterScripts.managementCharacterHud.ToggleShowStatistics(characterActionsInfo.isShowStats);
    }
    void ValidateShowDirection()
    {
        if (restoreMovementMouse > 0 && characterActions.CharacterInputs.BasicAttack.triggered || restoreMovementMouse > 0 && characterActions.CharacterInputs.UseSkill.triggered)
        {
            restoreMovementMouse = timeRestoreMovementMouse;
        }
        if (restoreMovementMouse > 0)
        {
            restoreMovementMouse -= Time.deltaTime;
            ValidateShowMouse(true);
        }
        else
        {
            characterActionsInfo.moveCamera = Vector2.zero;
            ValidateShowMouse(false);
        }
    }
    Vector2 GetMouseDirection()
    {
        Vector2 mousePos = characterActionsInfo.mousePos;
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 direction = mousePos - screenCenter;
        float normalizedX = direction.x / (Screen.width / 2f);
        float normalizedY = direction.y / (Screen.height / 2f);
        normalizedX = Mathf.Clamp(normalizedX, -1f, 1f);
        normalizedY = Mathf.Clamp(normalizedY, -1f, 1f);

        return new Vector2(normalizedX, normalizedY);
    }
    void ValidateShowMouse(bool showAttackDiection)
    {
        if (GameManager.Instance.currentDevice != GameManager.TypeDevice.PC)
        {
            if (showAttackDiection)
            {
                attackDirection.SetActive(true);
            }
            else
            {
                attackDirection.SetActive(false);
            }
        }
        else
        {
            attackDirection.SetActive(false);
        }
    }
    [Serializable] public class CharacterActionsInfo
    {
        public Vector2 movement = Vector2.zero;
        public Vector2 moveCamera = Vector2.zero;
        public Vector2 mousePos = new Vector2();
        public bool _isSecondaryAction;
        public Action<bool> OnSecondaryActionChange;
        public bool isSecondaryAction
        {
            get => _isSecondaryAction;
            set
            {
                if (_isSecondaryAction != value)
                {
                    _isSecondaryAction = value;
                    OnSecondaryActionChange?.Invoke(_isSecondaryAction);
                }
            }
        }
        public bool isSkillsActive = false;
        public bool isShowStats = false;
    }
}