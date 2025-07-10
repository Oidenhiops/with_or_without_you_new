using UnityEngine;

public class DeadZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Character>(out Character character))
        {
            character.characterInfo.TakeDamage
            (
                character.characterInfo.GetStatisticByType(Character.TypeStatistics.Hp).maxValue,
                Color.white,
                0,
                Character.TypeDamage.TrueDamage,
                null
            );
        }
        else
        {
            Destroy(other.gameObject);
        }
    }
}
