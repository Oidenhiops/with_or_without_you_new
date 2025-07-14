using UnityEngine;

public class PlayerMovement : CharacterMovement
{
    public PlayerInputs playerInputs;
    public PlayerCamera playerCamera;
    public override void Move()
    {
        Vector3 inputs = new Vector3
        (
            playerInputs.characterActionsInfo.movement.x,
            0,
            playerInputs.characterActionsInfo.movement.y
        ).normalized;
        playerCamera.CamDirection(out Vector3 camForward, out Vector3 camRight);
        Vector3 camDirection = (inputs.x * camRight + inputs.z * camForward).normalized;
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
        speed = character.GetStatisticByType(Character.TypeStatistics.Spd).currentValue;
        movementDirection.x *= speed;
        movementDirection.y = character.rb.linearVelocity.y;
        movementDirection.z *= speed;
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