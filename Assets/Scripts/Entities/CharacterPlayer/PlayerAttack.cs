public class PlayerAttack : ManagementCharacterAttack
{
    public PlayerInputs playerInputs;
    public override void ValidateAttack()
    {
        if (playerInputs.characterActions.CharacterInputs.BasicAttack.triggered)
        {
            ValidateAttackMobile();
        }
    }
    public override void ValidateAttackMobile()
    {
        if (!character.isActive || !GameManager.Instance.startGame) return;
        
        if (character.characterAnimations != null &&
            character.characterAnimations.ValidateAnimationEnd("TakeDamage") &&
            ValidateAllAnimationsAttackEnd() &&
            cooldownAttack <= 0 &&
            character.GetStatisticByType(costsAttack.typeStatistics).currentValue - costsAttack.baseValue >= 0)
        {
            character.GetStatisticByType(costsAttack.typeStatistics).currentValue -= costsAttack.baseValue;
            StartAttack();
        }
    }
}
