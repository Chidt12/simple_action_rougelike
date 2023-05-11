//////////////////////////////////////////////
/// 2D Shader Collection - by VETASOFT 2018 //
//////////////////////////////////////////////


//////////////////////////////////////////////

Shader "2DShaderCollection/Animated_PingPong"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
AnimatedPingPongOffsetUV_1_OffsetX_1("AnimatedPingPongOffsetUV_1_OffsetX_1", Range(-1, 1)) = 0.182
AnimatedPingPongOffsetUV_1_OffsetY_1("AnimatedPingPongOffsetUV_1_OffsetY_1", Range(-1, 1)) = 0
AnimatedPingPongOffsetUV_1_ZoomX_1("AnimatedPingPongOffsetUV_1_ZoomX_1", Range(1, 10)) = 1
AnimatedPingPongOffsetUV_1_ZoomY_1("AnimatedPingPongOffsetUV_1_ZoomY_1", Range(1, 10)) = 1
AnimatedPingPongOffsetUV_1_Speed_1("AnimatedPingPongOffsetUV_1_Speed_1", Range(-1, 1)) = 0.354
_LerpUV_Fade_1("_LerpUV_Fade_1", Range(0, 1)) = 1
_SpriteFade("SpriteFade", Range(0, 1)) = 1.0

// required for UI.Mask
[HideInInspector]_StencilComp("Stencil Comparison", Float) = 8
[HideInInspector]_Stencil("Stencil ID", Float) = 0
[HideInInspector]_StencilOp("Stencil Operation", Float) = 0
[HideInInspector]_StencilWriteMask("Stencil Write Mask", Float) = 255
[HideInInspector]_StencilReadMask("Stencil Read Mask", Float) = 255
[HideInInspector]_ColorMask("Color Mask", Float) = 15

}

SubShader
{

Tags {"Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
ZWrite Off Blend SrcAlpha OneMinusSrcAlpha Cull Off

// required for UI.Mask
Stencil
{
Ref [_Stencil]
Comp [_StencilComp]
Pass [_StencilOp]
ReadMask [_StencilReadMask]
WriteMask [_StencilWriteMask]
}

Pass
{

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

struct appdata_t{
float4 vertex   : POSITION;
float4 color    : COLOR;
float2 texcoord : TEXCOORD0;
};

struct v2f
{
float2 texcoord  : TEXCOORD0;
float4 vertex   : SV_POSITION;
float4 color    : COLOR;
};

sampler2D _MainTex;
float _SpriteFade;
float AnimatedPingPongOffsetUV_1_OffsetX_1;
float AnimatedPingPongOffsetUV_1_OffsetY_1;
float AnimatedPingPongOffsetUV_1_ZoomX_1;
float AnimatedPingPongOffsetUV_1_ZoomY_1;
float AnimatedPingPongOffsetUV_1_Speed_1;
float _LerpUV_Fade_1;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}


float2 AnimatedPingPongOffsetUV(float2 uv, float offsetx, float offsety, float zoomx, float zoomy, float speed)
{
float time = sin(_Time * 100* speed)  * 0.1;
speed *= time * 25;
uv += float2(offsetx, offsety)*speed;
uv = uv * float2(zoomx, zoomy);
return uv;
}
float4 frag (v2f i) : COLOR
{
float2 AnimatedPingPongOffsetUV_1 = AnimatedPingPongOffsetUV(i.texcoord,AnimatedPingPongOffsetUV_1_OffsetX_1,AnimatedPingPongOffsetUV_1_OffsetY_1,AnimatedPingPongOffsetUV_1_ZoomX_1,AnimatedPingPongOffsetUV_1_ZoomY_1,AnimatedPingPongOffsetUV_1_Speed_1);
i.texcoord = lerp(i.texcoord,AnimatedPingPongOffsetUV_1,_LerpUV_Fade_1);
float4 _MainTex_1 = tex2D(_MainTex,i.texcoord);
float4 FinalResult = _MainTex_1;
FinalResult.rgb *= i.color.rgb;
FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
return FinalResult;
}

ENDCG
}
}
Fallback "Sprites/Default"
}
