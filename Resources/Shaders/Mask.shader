Shader "EasyOutline/Mask"
{
    Properties
    {
        _Thickness ("Thickness", Range(0.0, 0.5)) = 0.1
    }
    SubShader
    {
        LOD 100
        Tags {"Queue" = "Transparent+100" "RenderType" = "Transparent" }

        Pass
        {
            Name "Selection Mask - Mesh"
            Cull off
            ZWrite Off
            ZTest Off
            ColorMask 0
            
            Stencil
            {
                Ref 1
                Comp Greater
                Pass IncrSat
            }
        }
        Pass
        {
            Name "Selection Mask - Outline ZTest"
            Cull off
            ZWrite Off
            ZTest On
            ColorMask 0
            
            Stencil
            {
                Ref 0
                Comp Equal
                Zfail IncrSat
            }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float3 smoothNormal : TEXCOORD2;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };
            
            fixed4 _OutlineColor;
            float _Thickness;

            v2f vert (appdata v)
            {
                v2f o;
                float4 newPos = v.vertex + float4(v.smoothNormal, 0.0f) * _Thickness;
                o.vertex = UnityObjectToClipPos(newPos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return 0;
            }
            ENDCG
        }
        Pass
        {
            Name "Selection Mask - Mesh Visible Area"
            Cull Back
            ZWrite Off
            ZTest Off
            ColorMask 0
            
            Stencil
            {
                Ref 1
                Comp Equal
                Pass IncrSat
            }
        }
        Pass
        {
            Name "Selection Mask - Mesh Invisible Area"
            Cull Back
            ZWrite Off
            ZTest On
            ColorMask 0
            
            Stencil
            {
                Ref 2
                Comp Equal
                Pass IncrSat
            }
        }
    }
}