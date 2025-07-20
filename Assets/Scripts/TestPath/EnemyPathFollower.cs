using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class EnemyPathFollower : CharacterMovement
{
    public Character targetCharacter;
    public Transform _target;
    public Action<Transform> OnTargetChange;
    public Transform target
    {
        get => _target;
        set
        {
            if (_target != value)
            {
                _target = value;
                OnTargetChange?.Invoke(_target);
            }
        }
    }
    public float stoppingDistance = 0.1f;
    public float recalcularDistanciaMinima = 1f;
    public float frecuenciaChequeo = 0.5f;

    private NavMeshPath path;
    public int currentCornerIndex = 0;
    private bool hasPath = false;
    private Vector3 ultimaPosicionObjetivo;
    Vector3 targetCorner;
    public Transform testTarget;
    void Awake()
    {
        path = new NavMeshPath();

        if (target != null) ultimaPosicionObjetivo = target.position;

        OnTargetChange += OnTargetChangeValue;
        InvokeRepeating(nameof(ChequearSiRecalcularPath), 0f, frecuenciaChequeo);
    }
    void OnTargetChangeValue(Transform target)
    {
        character.characterModelDirection.SetTarget(target);
        if (target && target.TryGetComponent<Character>(out Character targetCharacterFd))
        {
            targetCharacter = targetCharacterFd;
        }
        else if (!target)
        {
            targetCharacter = null;
            character.characterModelDirection.movementCharacter = Vector2.zero;
        }
    }
    public override void Move() { }

    void FixedUpdate()
    {
        if (targetCharacter && !targetCharacter.isActive)
        {
            targetCharacter = null;
            target = null;
            character.characterModelDirection.SetTarget(null);
        }
        else if (targetCharacter && targetCharacter.isActive && !targetCharacter.characterModelDirection.characterTarget)
        {
            character.characterModelDirection.SetTarget(target);
        }

        if (hasPath && path.corners.Length > 0)
        {
            targetCorner = path.corners[currentCornerIndex];
            direction = (targetCorner - transform.position).normalized;

            if (Vector3.Distance(transform.position, targetCorner) < stoppingDistance)
            {
                currentCornerIndex++;

                if (currentCornerIndex >= path.corners.Length)
                {
                    hasPath = false;
                    movementDirection = new Vector3(0, character.rb.linearVelocity.y, 0);
                }
            }
        }
        else
        {
            direction = Vector3.zero;
            targetCorner = Vector3.zero;
            hasPath = false;
            movementDirection = new Vector3(0, character.rb.linearVelocity.y, 0);
        }
        CalculateDirectionForce(ref direction);
        DiscountOtherForces();
        character.rb.linearVelocity = movementDirection;
        character.characterModelDirection.movementCharacter = new Vector2(direction.x, direction.z);
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
    void ChequearSiRecalcularPath()
    {
        if (target == null) return;

        float distanciaAlObjetivo = Vector3.Distance(target.position, ultimaPosicionObjetivo);

        if (distanciaAlObjetivo > recalcularDistanciaMinima)
        {
            CalcularNuevoPath();
            ultimaPosicionObjetivo = target.position;
        }
    }
    void CalcularNuevoPath()
    {
        if (NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path))
        {
            if (path.corners.Length > 0)
            {
                currentCornerIndex = 0;
                hasPath = true;
            }
            else
            {
                hasPath = false;
            }
        }
        else
        {
            hasPath = false;
        }
    }
    public override void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    void OnDrawGizmos()
    {
        if (path != null && path.corners.Length > 1)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);
                Gizmos.DrawSphere(path.corners[i], 0.1f);
            }

            Gizmos.DrawSphere(path.corners[^1], 0.1f);
        }

        if (target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
}