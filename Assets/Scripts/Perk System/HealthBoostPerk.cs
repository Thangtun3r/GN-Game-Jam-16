using UnityEngine;

public class HealthBoostPerk : PerkBase
{
    [SerializeField] private int healAmount = 25;

    protected override void OnUsePerk()
    {
        // Put your effect logic here. For example, healing the player:
        Debug.Log($"Using [HealthBoostPerk], healing player by {healAmount} points.");
        // ... implement actual healing
    }
}