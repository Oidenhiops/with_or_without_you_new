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
        CalculateDirectionForce();
        Jump();
        character.rb.linearVelocity = movementDirection;
    }
    void CalculateDirectionForce()
    {
        movementDirection.x *= character.GetStatisticByType(Character.TypeStatistics.Spd).currentValue;
        movementDirection.y = character.rb.linearVelocity.y;
        movementDirection.z *= character.GetStatisticByType(Character.TypeStatistics.Spd).currentValue;
        movementDirection += otherForceMovement;
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