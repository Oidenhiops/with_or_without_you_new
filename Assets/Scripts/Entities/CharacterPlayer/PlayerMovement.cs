using UnityEngine;

public class PlayerMovement : CharacterMovement
{
    public PlayerInputs playerInputs;
    public PlayerCamera playerCamera;
    public override void Move()
    {
        direction = new Vector3
        (
            playerInputs.characterActionsInfo.movement.x,
            0,
            playerInputs.characterActionsInfo.movement.y
        ).normalized;
        playerCamera.CamDirection(out Vector3 camForward, out Vector3 camRight);
        Vector3 camDirection = (direction.x * camRight + direction.z * camForward).normalized;
        movementDirection = new Vector3
        (
            camDirection.x,
            0,
            camDirection.z
        ).normalized;
        DiscountOtherForces();
        CalculateDirectionForce(ref camDirection);
        Jump();
        character.rb.linearVelocity = movementDirection;
    }
    void CalculateDirectionForce(ref Vector3 camDirection)
    {
        unclampedValue = new Vector3
        (
            camDirection.x,
            0,
            camDirection.z
        ).normalized;
        unclampedValue.x *= character.GetStatisticByType(Character.TypeStatistics.Spd).currentValue;
        unclampedValue.y = character.rb.linearVelocity.y;
        unclampedValue.z *= character.GetStatisticByType(Character.TypeStatistics.Spd).currentValue;
        unclampedValue += otherForceMovement;

        movementDirection = new Vector3(Mathf.Clamp(unclampedValue.x, -maxVelocity, maxVelocity), Mathf.Clamp(unclampedValue.y, -maxVelocity, maxVelocity), Mathf.Clamp(unclampedValue.z, -maxVelocity, maxVelocity));
    }
    void Jump()
    {
        if (character.isGrounded &&
            !playerInputs.characterActionsInfo.isSkillsActive &&
            playerInputs.characterActions.CharacterInputs.Jump.triggered)
        {
            character.rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}