using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ImpactEffect : MonoBehaviour
{
    // Assign the material using the ImpactEffect shader in the Inspector.
    public Material postProcessMaterial;

    // Controls whether the effect is active.
    private bool effectActive = false;

    // Public method to trigger the effect.
    public void TriggerEffect(bool enable)
    {
        Debug.Log("triggerimpact");
        effectActive = enable;
    }

    // OnRenderImage is called after the scene is rendered.
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Debug.Log("OnRenderImage called - Effect Active: " + effectActive);
    
        if (effectActive && postProcessMaterial != null)
        {
            Graphics.Blit(source, destination, postProcessMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }

}