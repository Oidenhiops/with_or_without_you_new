using UnityEngine;

public class ManagementCharacterModelDirection : MonoBehaviour, ManagementCharacterModelDirection.ICharacterDirection
{
    public Character character;
    [SerializeField] float rayDistanceTarget = 10f;
    [SerializeField] LayerMask targetMask;
    [SerializeField] Character characterTarget;
    public Vector2 movementDirectionAnimation = new Vector2();
    public Vector2 movementCharacter = new Vector2();
    public GameObject directionPlayer;
    void Start()
    {
        rayDistanceTarget = character.characterInfo.isPlayer ? 10 : character.characterInfo.characterScripts.characterAttack.GetDistLostTarget();
    }
    public void ChangeModelDirection()
    {
        if (character.characterInfo.isPlayer && character.characterInputs != null)
        {
            movementCharacter = character.characterInputs.characterActionsInfo.movement;
            if (characterTarget != null)
            {
                LookToTarget();
            }
            else
            {
                if (character.characterInputs.characterActions.CharacterInputs.Target.triggered)
                {
                    ValidateLookToTarget();
                }
                if (character.characterInputs.characterActionsInfo.moveCamera == Vector2.zero)
                {
                    MoveWhitOutCamera();
                }
                else
                {
                    MoveWhitCamera();
                }
            }
        }
    }

    public void ValidateLookToTarget()
    {
        if (Physics.BoxCast(directionPlayer.transform.position, Vector3.one, directionPlayer.transform.forward, out RaycastHit objectHit, Quaternion.identity, rayDistanceTarget, targetMask))
        {
            characterTarget = objectHit.collider.GetComponent<Character>();
        }
    }

    private void LookToTarget()
    {
        movementDirectionAnimation = Camera.main.WorldToViewportPoint(characterTarget.transform.position) - Camera.main.WorldToViewportPoint(transform.position);
        character.characterInfo.characterScripts.characterAnimations.GetCharacterSprite().transform.localRotation =
            Quaternion.Euler(0, movementDirectionAnimation.x > 0 ? -180 : 0, 0);
        directionPlayer.transform.LookAt(new Vector3(characterTarget.transform.position.x, directionPlayer.transform.position.y, characterTarget.transform.position.z));
        if (!characterTarget.characterInfo.isActive || 
        Vector3.Distance(characterTarget.transform.position, transform.position) > rayDistanceTarget || 
        character.characterInfo.isPlayer && character.characterInputs.characterActions.CharacterInputs.Target.triggered)
        {
            characterTarget = null;
        }
    }

    void MoveWhitOutCamera()
    {
        if (character.characterInputs.characterActionsInfo.movement != Vector2.zero)
        {            
            if (character.characterInputs.characterActionsInfo.movement.x != 0)
            {
                movementDirectionAnimation.x = character.characterInputs.characterActionsInfo.movement.x;
            }
            if (character.characterInputs.characterActionsInfo.movement.y != 0)
            {
                movementDirectionAnimation.y = character.characterInputs.characterActionsInfo.movement.y;
            }
            character.characterInfo.characterScripts.characterAnimations.GetCharacterSprite().transform.localRotation = 
                Quaternion.Euler(0, movementDirectionAnimation.x > 0 ? -180 : 0, 0);
            float angle = Mathf.Atan2(character.characterInfo.characterScripts.characterMove.GetDirectionMove().x, character.characterInfo.characterScripts.characterMove.GetDirectionMove().z) * Mathf.Rad2Deg;
            directionPlayer.transform.rotation = Quaternion.Lerp(directionPlayer.transform.rotation, Quaternion.Euler(0, angle, 0f), 0.3f);
        }
    }
    void MoveWhitCamera()
    {
        if (character.characterInputs.characterActionsInfo.moveCamera != Vector2.zero)
        {
            movementDirectionAnimation = character.characterInputs.characterActionsInfo.moveCamera;
            character.characterInfo.characterScripts.characterAnimations.GetCharacterSprite().transform.localRotation =
                Quaternion.Euler(0, movementDirectionAnimation.x > 0 ? -180 : 0, 0);

            Vector3 inputs = new Vector3
            (
                movementDirectionAnimation.x,
                0,
                movementDirectionAnimation.y
            ).normalized;
            character.characterInfo.characterScripts.managementPlayerCamera.CamDirection(out Vector3 camForward, out Vector3 camRight);
            Vector3 camDirection = (inputs.x * camRight + inputs.z * camForward).normalized;
            Vector2 direction = new Vector2(camDirection.x, camDirection.z).normalized;
            character.characterInfo.characterScripts.characterAnimations.GetCharacterSprite().transform.localRotation =
                Quaternion.Euler(0, movementDirectionAnimation.x > 0 ? -180 : 0, 0);
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            directionPlayer.transform.rotation = Quaternion.Lerp(directionPlayer.transform.rotation, Quaternion.Euler(0, angle, 0f), 0.3f);
        }
    }
    void OnDrawGizmos()
    {
        if (directionPlayer != null)
        {
            Gizmos.color = Color.red;
            Vector3 endPoint = directionPlayer.transform.position + directionPlayer.transform.forward * rayDistanceTarget;
            Gizmos.DrawLine(directionPlayer.transform.position, endPoint);
            if (Physics.BoxCast(directionPlayer.transform.position, Vector3.one, directionPlayer.transform.forward, out RaycastHit objectHit, Quaternion.identity, rayDistanceTarget, targetMask))
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(objectHit.point, Vector3.one * 0.2f);
            }
            Gizmos.color = Color.yellow;
            Gizmos.matrix = Matrix4x4.TRS(directionPlayer.transform.position, Quaternion.LookRotation(directionPlayer.transform.forward), Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero + Vector3.forward * (rayDistanceTarget / 2), new Vector3(Vector3.one.x * 2, Vector3.one.y * 2, rayDistanceTarget));
        }
    }
    public Vector2 GetDirectionAnimation()
    {
        return movementDirectionAnimation;
    }
    public Character GetLookTarget()
    {
        return characterTarget;
    }
    public Vector2 GetDirectionMovementCharacter(){
        return movementCharacter;
    }
    public void SetTarget(GameObject target)
    {
        characterTarget = target.GetComponent<Character>();
    }
    public interface ICharacterDirection
    {
        public Vector2 GetDirectionMovementCharacter();
        public Vector2 GetDirectionAnimation();
        public Character GetLookTarget();
        public void SetTarget(GameObject target);
    }
}
