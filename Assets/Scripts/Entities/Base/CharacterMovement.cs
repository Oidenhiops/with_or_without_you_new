using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public Character character;
    public Vector3 movementDirection;
    public SerializedDictionary<string, OtherForceMovements> otherForceMovements = new SerializedDictionary<string, OtherForceMovements>();
    protected Vector3 otherForceMovement;
    protected Vector3 direction;
    protected float jumpForce = 3;
    void Update()
    {
        if (character.isActive && GameManager.Instance.startGame) Move();
    }
    public virtual void Move() { Debug.LogError("Movement not implemented"); }
    public void AddOtherForce(string id, Vector3 direction, bool canDiscount, float time)
    {
        UpdateOtherForceMovement(id, new OtherForceMovements
        {
            initialForceDirection = direction,
            forceDirection = direction,
            canDiscount = canDiscount,
            totalTime = time,
            elapsedTime = time,
        });
    }
    void UpdateOtherForceMovement(string id, OtherForceMovements otherForce)
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
    protected void DiscountOtherForces()
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

                UpdateOtherForceMovement(force.Key, force.Value);

                if (force.Value.forceDirection.magnitude <= 0.01f)
                {
                    force.Value.forceDirection = Vector3.zero;
                }
                UpdateOtherForceMovement(force.Key, force.Value);
            }
        }
    }
    public virtual void SetPositionTarget(Transform position) { }
    public virtual void SetCanMoveState(bool state) { }
    public virtual void SetTarget(Transform targetPos) { }
    [Serializable]
    public class OtherForceMovements
    {
        public Vector3 initialForceDirection = new Vector3();
        public Vector3 forceDirection = new Vector3();
        public bool canDiscount = false;
        public float totalTime = 0;
        public float elapsedTime = 0;
    }
}
