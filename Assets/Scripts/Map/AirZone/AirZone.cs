using UnityEngine;

public class AirZone : MonoBehaviour
{
    public string idForce = "AirZone";
    public float force;
    void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<Character>(out Character character))
        {
            character.characterInfo.characterScripts.characterMove.AddOtherForce(idForce, -transform.right.normalized * force, false, 0.1f);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Character>(out Character character))
        {
            character.characterInfo.characterScripts.characterMove.AddOtherForce(idForce, -transform.right.normalized * 0, false, 0);
        }
    }
}
