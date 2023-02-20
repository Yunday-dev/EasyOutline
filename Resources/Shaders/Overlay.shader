Shader "EasyOutline/Overlay"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _VisibleColor ("VisibleColor", Color) = (1.0, 1.0, 1.0, 1.0)
        _InvisibleColor ("InvisibleColor", Color) = (1.0, 1.0, 1.0, 1.0)
        _Thickness ("Thickness", Range(0.0, 0.5)) = 0.1
    }
    SubShader
    {
        Tags {"Queue" = "Transparent+100" "RenderType" = "Transparent+100" }
        LOD 100

        Pass
        {
            Name "Draw Visible Overlay"
            Cull Back
            ZWrite Off
            ZTest Off
            Blend SrcAlpha OneMinusSrcAlpha
            Stencil
            {
                Ref 2
                Comp Equal
            }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
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
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _InvisibleColor;
            }
            ENDCG
        }
        Pass
        {
            Name "Draw Visible Overlay"
            Cull Back
            ZWrite Off
            ZTest On
            Blend SrcAlpha OneMinusSrcAlpha
            Stencil
            {
                Ref 3
                Comp Equal
            }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
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
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _VisibleColor;
            }
            ENDCG
        }
    }
}
