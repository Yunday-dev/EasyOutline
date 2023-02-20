Shader "EasyOutline/Outline"
{
    Properties
    {
        _VisibleColor ("VisibleColor", Color) = (1.0, 1.0, 1.0, 1.0)
        _InvisibleColor ("InvisibleColor", Color) = (1.0, 1.0, 1.0, 1.0)
        _Thickness ("Thickness", Range(0.0, 0.5)) = 0.1
    }
    SubShader
    {
        LOD 100
        Tags { "Queue" = "Transparent+110" "RenderType" = "Transparent" "DisableBatching" = "True" }

        Pass
        {
            Name "Draw Visible Outline"
            Cull off
            Zwrite Off
            ZTest Off
            Blend SrcAlpha OneMinusSrcAlpha
            Stencil
            {
                Ref 0
                Comp equal
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
            
            fixed4 _VisibleColor;
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
                return _VisibleColor;
            }
            ENDCG
        }
        Pass
        {
            Name "Draw Invisible Outline"
            Cull off
            Zwrite Off
            ZTest Off
            Blend SrcAlpha OneMinusSrcAlpha
            Stencil
            {
                Ref 1
                Comp equal
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
            
            fixed4 _InvisibleColor;
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
                return _InvisibleColor;
            }
            ENDCG
        }
    }
}
