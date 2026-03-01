Shader "Unlit/ChromaKeyVideo"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _KeyColor ("Key Color", Color) = (0, 1, 0, 1)
        _Threshold ("Threshold", Range(0,1)) = 0.35
        _Smooth ("Smooth", Range(0,1)) = 0.1
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _KeyColor;
            float _Threshold;
            float _Smooth;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // Distanz zwischen Pixel-Farbe und Key-Farbe
                float3 diff = col.rgb - _KeyColor.rgb;
                float dist = length(diff);

                // Alpha: alles nahe der Key-Farbe wird ausgeblendet
                float alpha = smoothstep(_Threshold, _Threshold + _Smooth, dist);
                col.a *= alpha;

                return col;
            }
            ENDCG
        }
    }
}
