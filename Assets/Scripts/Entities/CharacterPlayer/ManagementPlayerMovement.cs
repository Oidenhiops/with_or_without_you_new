using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class ManagementPlayerMovement : MonoBehaviour, Character.ICharacterMove
{
    public Character character;
    Vector3 movementDirection;
    public SerializedDictionary<string, OtherForceMovements> otherForceMovements = new SerializedDictionary<string, OtherForceMovements>();
    public Vector3 otherForceMovement;
    public OtherForceMovements addForce;
    public string addId;
    float jumpForce = 3;
    float speed = 0;
    public void Move()
    {
        Vector3 inputs = new Vector3
        (
            character.characterInputs.characterActionsInfo.movement.x,
            0,
            character.characterInputs.characterActionsInfo.movement.y
        ).normalized;
        character.characterInfo.characterScripts.managementPlayerCamera.CamDirection(out Vector3 camForward, out Vector3 camRight);
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
        character.characterInfo.rb.linearVelocity = movementDirection;
    }
    void CalculateDirectionForce()
    {
        speed = character.characterInfo.GetStatisticByType(Character.TypeStatistics.Spd).currentValue;
        movementDirection.x *= speed;
        movementDirection.y = character.characterInfo.rb.linearVelocity.y;
        movementDirection.z *= speed;
        movementDirection += otherForceMovement;
    }
    void Jump()
    {
        if (character.characterInfo.isGrounded &&
            !character.characterInputs.characterActionsInfo.isSkillsActive &&
            character.characterInputs.characterActions.CharacterInputs.Jump.triggered)
        {
            character.characterInfo.rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    public Rigidbody GetRigidbody()
    {
        return character.characterInfo.rb;
    }
    public void SetPositionTarget(Transform position) { }
    public void SetCanMoveState(bool state) { }
    public void SetTarget(Transform targetPos) { }
    public void SetOtherForceMovement(string id, OtherForceMovements otherForce)
    {
        if (otherForceMovements.TryGetValue(id, out OtherForceMovements findedForceMovement))
        {
            findedForceMovement.initialForceDirection = otherForce.initialForceDirection;
            findedForceMovement.forceDirection = otherForce.forceDirection;
            findedForceMovement.canDiscount = otherForce.canDiscount;
            findedForceMovement.elapsedTime = otherForce.elapsedTime;
        }
        else
        {
            otherForceMovements.Add(id, new OtherForceMovements
            {
                initialForceDirection = otherForce.initialForceDirection,
                forceDirection = otherForce.forceDirection,
                canDiscount = otherForce.canDiscount,
                totalTime = otherForce.totalTime,
                elapsedTime = otherForce.elapsedTime
            });
        }

        otherForceMovement = Vector3.zero;

        foreach (KeyValuePair<string, OtherForceMovements> force in otherForceMovements)
        {
            otherForceMovement += force.Value.forceDirection;
        }
    }
    public void DiscountOtherForces()
    {
        if (otherForceMovements.Count == 0) return;

        foreach (KeyValuePair<string, OtherForceMovements> force in otherForceMovements)
        {
            if (force.Value.elapsedTime <= 0)
            {
                otherForceMovements.Remove(force.Key);
                return;
            }

            force.Value.elapsedTime -= Time.deltaTime;

            if (force.Value.canDiscount)
            {
                float ratio = force.Value.elapsedTime / force.Value.totalTime;
                ratio = Mathf.Clamp01(ratio);

                force.Value.forceDirection = force.Value.initialForceDirection * ratio;

                SetOtherForceMovement(force.Key, force.Value);

                if (force.Value.forceDirection.magnitude <= 0.01f)
                {
                    force.Value.forceDirection = Vector3.zero;
                }
                SetOtherForceMovement(force.Key, force.Value);
            }
        }
    }
    public void AddOtherForce(string id, Vector3 direction, bool canDiscount, float time)
    {
        SetOtherForceMovement(id, new OtherForceMovements
        {
            initialForceDirection = direction,
            forceDirection = direction,
            canDiscount = canDiscount,
            totalTime = time,
            elapsedTime = time,
        });
    }
    public Vector3 GetDirectionMove()
    {
        return movementDirection;
    }
    [Serializable] public class OtherForceMovements
    {
        public Vector3 initialForceDirection = new Vector3();
        public Vector3 forceDirection = new Vector3();
        public bool canDiscount = false;
        public float totalTime = 0;
        public float elapsedTime = 0;
    }
}