Shader "Custom/ImpactEffect" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        // Set _Saturation to 0 for full grayscale, 1 for full color.
        _Saturation ("Saturation", Range(0, 1)) = 0
        // Tint color multiplies the resulting image; use white for no tint.
        _TintColor ("Tint Color", Color) = (1,1,1,1)
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        Pass {
            // Always render without depth or culling.
            ZTest Always Cull Off ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            sampler2D _MainTex;
            float4 _TintColor;
            float _Saturation;
            
            fixed4 frag(v2f_img i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv);
                // Calculate grayscale value using luminosity.
                float gray = dot(col.rgb, float3(0.299, 0.587, 0.114));
                // Interpolate between original color and grayscale.
                fixed3 outputColor = lerp(float3(gray, gray, gray), col.rgb, _Saturation);
                // Apply tint.
                outputColor *= _TintColor.rgb;
                return fixed4(outputColor, col.a);
            }
            ENDCG
        }
    }
}
