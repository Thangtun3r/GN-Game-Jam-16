
using UnityEngine;
using System;

public class ImpactDebugger : MonoBehaviour
{
    [Header("Debug Options")]
    [SerializeField] private bool logImpact = true;
    [SerializeField] private bool drawImpactRay = true;
    [SerializeField] private Color impactColor = Color.red;
    [SerializeField] private float debugLineDuration = 0.5f;
    [SerializeField] private float impactThreshold = 2.0f;
    [SerializeField] private LayerMask impactLayer;

    [Header("Damage Settings")]
    [SerializeField] private int damageAmount = 10;

    private BaseTab baseTab;

    public static event Action OnImpactDetected;

    private void Start()
    {
        baseTab = GetComponentInParent<BaseTab>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision object is in the specified layer
        if (((1 << collision.gameObject.layer) & impactLayer) != 0)
        {
            float impactPower = collision.relativeVelocity.magnitude * collision.rigidbody.mass;

            if (impactPower > impactThreshold && baseTab.isThrowing)
            {
                OnImpactDetected?.Invoke();

                if (logImpact)
                {
                    Debug.Log($"Impact Detected! Object: {collision.gameObject.name}, Impact Power: {impactPower}");
                }

                if (drawImpactRay)
                {
                    Vector2 impactDirection = -collision.contacts[0].normal;
                    Debug.DrawRay(
                        collision.contacts[0].point, 
                        impactDirection * impactPower * 0.1f, 
                        impactColor, 
                        debugLineDuration
                    );
                }

                Debug.Log("Impact frame!");

                // All we do is call the manager
                EffectManager.Instance.TriggerScreenShake();
                EffectManager.Instance.TriggerSlowMotion();

                // Deal damage
                IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damageAmount);
                }
            }
        }
    }
}
