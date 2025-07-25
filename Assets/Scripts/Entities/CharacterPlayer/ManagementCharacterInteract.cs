using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

public class ManagementCharacterInteract : MonoBehaviour
{
    public PlayerInputs playerInputs;
    RaycastHit[] hitBuffer = new RaycastHit[20];
    List<GameObject> interactables = new List<GameObject>(20);
    public Character character;
    public GameObject[] _currentInteracts;
    public event Action<GameObject[]> OnInteractsChanged;
    public GameObject[] currentInteracts
    {
        get => _currentInteracts;
        set
        {
            if (_currentInteracts.Length != value.Length)
            {
                _currentInteracts = value;
                OnInteractsChanged?.Invoke(_currentInteracts);
            }
        }
    }
    Vector3 offset = new Vector3(0, 0.5f, 0);
    Vector3 size = new Vector3(1.5f, 1.5f, 1.5f);
    public LayerMask layerMask;
    public GameObject currentInteract;
    public bool isRefreshInteracts;
    public int _currentInteractIndex;
    public int currentInteractIndex 
    {
        get => _currentInteractIndex;
        set 
        {
            if (_currentInteractIndex != value)
            {
                _currentInteractIndex = value;
                UpdateScrollInteract();
            }
        }
    }
    public void InitializeInteractsEvents()
    {
        OnInteractsChanged += HandleInteracts;
        playerInputs.characterActions.CharacterInputs.Interact.performed += OnInteract;
    }
    void OnDestroy()
    {
        OnInteractsChanged -= HandleInteracts;
        playerInputs.characterActions.CharacterInputs.Interact.performed -= OnInteract;
    }
    public void Update()
    {
        if (character.isPlayer && GameManager.Instance.startGame)
        {
            CheckInteracts();
        }
    }
    void OnInteract(InputAction.CallbackContext context)
    {
        if (character.isActive && !playerInputs.characterActionsInfo.isSkillsActive && currentInteract && context.action.triggered)
        {
            HanldeInteracObject();
        }
    }
    void HanldeInteracObject()
    {
        currentInteract.GetComponent<ManagementInteract>().Interact(character);
    }
    void HandleInteracts(GameObject[] obj)
    {
        _= RefreshIteracts(obj);
    }
    async Task RefreshIteracts (GameObject[] objects)
    {
        isRefreshInteracts = true;
        await character.characterHud.RefreshInteracts(objects);
        if (objects.Length > 0)
        {
            character.characterHud.characterUi.interactUi.bannerInteract.SetActive(true);
        }
        else
        {
            currentInteract = null;
            character.characterHud.characterUi.interactUi.bannerInteract.SetActive(false);
        }
        UpdateScrollInteract();
        EventSystem.current.SetSelectedGameObject(character.characterHud.characterUi.interactUi.interacts[currentInteractIndex].bannerInteract.gameObject);
        isRefreshInteracts = false;
    }
    public void UpdateScrollInteract()
    {
        character.characterHud.UpdateScrollInteract();
    }
    public void CheckInteracts()
    {
        int count = Physics.BoxCastNonAlloc(
            transform.position + offset,
            size / 2,
            Vector3.up,
            hitBuffer,
            Quaternion.identity,
            0,
            layerMask
        );

        interactables.Clear();

        if (!character.statusEffect.StatusEffectExist(StatusEffectSO.TypeStatusEffect.Disarm))
        {
            for (int i = 0; i < count; i++)
            {
                var hit = hitBuffer[i];
                var interact = hit.collider.GetComponent<ManagementInteract>();
                if (interact != null && interact.canInteract)
                {
                    interactables.Add(hit.collider.gameObject);
                }
            }
        }

        currentInteracts = interactables.ToArray();
    }
    void OnDrawGizmos()
    {
        Gizmos.color = currentInteracts.Length > 0 ? Color.cyan : Color.blue;
        Gizmos.DrawWireCube
        (
            transform.position + offset,
            size
        );
    }
}
