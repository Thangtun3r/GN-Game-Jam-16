using UnityEngine;

public abstract class PerkBase : MonoBehaviour, IPickable
{
    [Header("Base Perk Settings")]
    [SerializeField] private string perkName;
    [SerializeField] private Sprite perkSprite;

    // If true, this perk is consumed right after OnUsePerk() is called once
    [SerializeField] private bool isSingleUse = true;

    protected bool isConsumed;

    public string PerkName => perkName;
    public Sprite PerkSprite => perkSprite;
    public bool IsConsumed => isConsumed;

    // Called when the PerksManager picks up this perk
    public virtual void OnPickup()
    {
        // By default, do nothing else here. 
        // We'll "fake destroy" in derived classes if needed.
        Debug.Log($"[{perkName}] OnPickup called.");
    }

    // Called when the player decides to use (activate) the perk
    public void UsePerk()
    {
        // Perform the effect
        OnUsePerk();

        // If single-use, mark consumed so PerksManager knows it’s no longer available
        if (isSingleUse)
        {
            isConsumed = true;
        }
    }

    // Force derived classes to define how the perk's effect actually works
    protected abstract void OnUsePerk();
}