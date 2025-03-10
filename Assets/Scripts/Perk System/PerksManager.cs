using UnityEngine;
using UnityEngine.UI;

public class PerksManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image perkSlotImage; 
    // The UI Image showing the currently held perk

    private IPickable currentPerk;

    public bool HasPerk => currentPerk != null;

    private void Start()
    {
        ClearPerkSlot();
    }

    public bool TryPickupPerk(IPickable perk)
    {
        if (HasPerk)
        {
            // Already have a perk, ignore
            return false;
        }

        currentPerk = perk;
        perk.OnPickup();  // e.g., remove from scene

        // Update UI
        perkSlotImage.sprite = perk.PerkSprite;
        perkSlotImage.enabled = true;

        return true;
    }

    public void UseCurrentPerk()
    {
        if (!HasPerk) 
            return;

        currentPerk.UsePerk();

        // If the perk is consumed on use, clear it from our slot
        if (currentPerk.IsConsumed)
        {
            ClearPerkSlot();
        }
    }

    private void ClearPerkSlot()
    {
        currentPerk = null;
        perkSlotImage.sprite = null;
        perkSlotImage.enabled = false;
    }
}