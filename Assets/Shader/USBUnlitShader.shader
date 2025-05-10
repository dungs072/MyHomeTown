Shader "Unlit / USBUnlitShader"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.BlendMode)]
        _ScrBlend("Source Factor", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]
        _DstBlend("Destination Factor", Float) = 1
        _MainTex("Main Text", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags{"Queue"="Transparent" "RenderType"="Transparent"}
        //Blend [_ScrBlend] [_DstBlend]
        //AlphaToMask On
        ColorMask RG
        Pass
        {
//  CGPROGRAM
//             #pragma vertex vert
//             #pragma fragment frag

//             sampler2D _MainTex;
//             float4 _Color;

//             struct appdata
//             {
//                 float4 vertex : POSITION;
//                 float2 uv : TEXCOORD0;
//             };

//             struct v2f
//             {
//                 float2 uv : TEXCOORD0;
//                 float4 vertex : SV_POSITION;
//             };

//             v2f vert(appdata v)
//             {
//                 v2f o;
//                 o.vertex = UnityObjectToClipPos(v.vertex);
//                 o.uv = v.uv;
//                 return o;
//             }

//             half4 frag(v2f i) : SV_Target
//             {
//                 half4 col = tex2D(_MainTex, i.uv) * _Color;
//                 return col;
//             }
//             ENDCG
        }
    }
}
