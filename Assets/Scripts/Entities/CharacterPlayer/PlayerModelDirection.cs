using UnityEngine;

public class PlayerModelDirection : CharacterModelDirection
{
    public PlayerInputs playerInputs;
    public PlayerCamera playerCamera;
    public override void ChangeModelDirection()
    {
        if (character.isPlayer && playerInputs != null)
        {
            movementCharacter = playerInputs.characterActionsInfo.movement;
            if (characterTarget != null)
            {
                LookToTarget();
            }
            else
            {
                if (playerInputs.characterActions.CharacterInputs.Target.triggered)
                {
                    ValidateLookToTarget();
                }
                if (playerInputs.characterActionsInfo.moveCamera == Vector2.zero)
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
        character.characterAnimations.GetCharacterSprite().transform.localRotation =
            Quaternion.Euler(0, movementDirectionAnimation.x > 0 ? -180 : 0, 0);
        directionPlayer.transform.LookAt(new Vector3(characterTarget.transform.position.x, directionPlayer.transform.position.y, characterTarget.transform.position.z));
        if (!characterTarget.isActive || 
        Vector3.Distance(characterTarget.transform.position, transform.position) > rayDistanceTarget || 
        character.isPlayer && playerInputs.characterActions.CharacterInputs.Target.triggered)
        {
            characterTarget = null;
        }
    }
    void MoveWhitOutCamera()
    {
        if (playerInputs.characterActionsInfo.movement != Vector2.zero)
        {            
            if (playerInputs.characterActionsInfo.movement.x != 0)
            {
                movementDirectionAnimation.x = playerInputs.characterActionsInfo.movement.x;
            }
            if (playerInputs.characterActionsInfo.movement.y != 0)
            {
                movementDirectionAnimation.y = playerInputs.characterActionsInfo.movement.y;
            }
            character.characterAnimations.GetCharacterSprite().transform.localRotation = 
                Quaternion.Euler(0, movementDirectionAnimation.x > 0 ? -180 : 0, 0);
            float angle = Mathf.Atan2(character.characterMove.movementDirection.x, character.characterMove.movementDirection.z) * Mathf.Rad2Deg;
            directionPlayer.transform.rotation = Quaternion.Lerp(directionPlayer.transform.rotation, Quaternion.Euler(0, angle, 0f), 0.3f);
        }
    }
    void MoveWhitCamera()
    {
        if (playerInputs.characterActionsInfo.moveCamera != Vector2.zero)
        {
            movementDirectionAnimation = playerInputs.characterActionsInfo.moveCamera;
            character.characterAnimations.GetCharacterSprite().transform.localRotation =
                Quaternion.Euler(0, movementDirectionAnimation.x > 0 ? -180 : 0, 0);

            Vector3 inputs = new Vector3
            (
                movementDirectionAnimation.x,
                0,
                movementDirectionAnimation.y
            ).normalized;
            playerCamera.CamDirection(out Vector3 camForward, out Vector3 camRight);
            Vector3 camDirection = (inputs.x * camRight + inputs.z * camForward).normalized;
            Vector2 direction = new Vector2(camDirection.x, camDirection.z).normalized;
            character.characterAnimations.GetCharacterSprite().transform.localRotation =
                Quaternion.Euler(0, movementDirectionAnimation.x > 0 ? -180 : 0, 0);
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            directionPlayer.transform.rotation = Quaternion.Lerp(directionPlayer.transform.rotation, Quaternion.Euler(0, angle, 0f), 0.3f);
        }
    }
}
