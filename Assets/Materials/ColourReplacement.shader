Shader "Unlit/ColourReplacement"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorA ("Color A", Color) = (0, 0, 0, 1)
        _ColorAReplacement ("Color A replacement", Color) = (0, 0, 0, 1)
        _ColorB ("Color B", Color) = (1, 1, 1, 1)
        _ColorBReplacement ("Color B replacement", Color) = (1, 1, 1, 1)
        _Tolerance ("Tolerance",Float) = 0.05
        
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite On
        Cull Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _ColorA;
            fixed4 _ColorAReplacement;
            fixed4 _ColorB;
            fixed4 _ColorBReplacement;
            float _Tolerance;

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;  
                fixed4 color : COLOR;
                //fixed4 colorA : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                fixed4 colorA : COLOR1;
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
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed diff = col.r - _ColorA.r + col.g - _ColorA.g + col.b - _ColorA.b + col.a - _ColorA.a;
                if(diff>-_Tolerance && diff < _Tolerance)
                    col = _ColorAReplacement;
            
                diff = col.r - _ColorB.r + col.g - _ColorB.g + col.b - _ColorB.b + col.b -_ColorB.a;
                if(diff>-_Tolerance && diff < _Tolerance)
                    col = _ColorBReplacement;

                
                //col *= _ColorA;
                //col *= i.color;
                return col;
            }
            ENDCG
        }
    }
}
