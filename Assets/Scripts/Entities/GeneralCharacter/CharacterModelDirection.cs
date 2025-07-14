using UnityEngine;

public class CharacterModelDirection : MonoBehaviour
{
    public Character character;
    [SerializeField] protected float rayDistanceTarget = 10f;
    [SerializeField] protected LayerMask targetMask;
    [SerializeField] protected Character characterTarget;
    public Vector2 movementDirectionAnimation = new Vector2();
    public Vector2 movementCharacter = new Vector2();
    public GameObject directionPlayer;
    void Start()
    {
        rayDistanceTarget = character.isPlayer ? 10 : character.characterAttack.GetDistLostTarget();
    }
    void Update()
    {
        if (character.isActive && GameManager.Instance.startGame) ChangeModelDirection();
    }
    public virtual void ChangeModelDirection() { Debug.LogError("Not Implemented Model Direction"); }
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
    public void SetTarget(GameObject target)
    {
        characterTarget = target.GetComponent<Character>();
    }
}
