using UnityEngine;

public interface IPickable
{
    string PerkName { get; }
    Sprite PerkSprite { get; }
    bool IsConsumed { get; }

    void OnPickup();
    void UsePerk();
}