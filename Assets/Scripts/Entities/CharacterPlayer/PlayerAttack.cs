public class PlayerAttack : ManagementCharacterAttack
{
    public override void ValidateAttack()
    {
        if (character.characterInputs.characterActions.CharacterInputs.BasicAttack.triggered)
        {
            ValidateAttackMobile();
        }
    }
    public override void ValidateAttackMobile()
    {
        if (character.characterInfo.characterScripts.characterAnimations != null &&
            character.characterInfo.characterScripts.characterAnimations.ValidateAnimationEnd("TakeDamage") &&
            ValidateAllAnimationsAttackEnd() &&
            cooldownAttack <= 0 &&
            character.characterInfo.GetStatisticByType(costsAttack.typeStatistics).currentValue - costsAttack.baseValue >= 0)
        {
            character.characterInfo.GetStatisticByType(costsAttack.typeStatistics).currentValue -= costsAttack.baseValue;
            StartAttack();
        }
    }
}
