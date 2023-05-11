//////////////////////////////////////////////
/// 2D Shader Collection - by VETASOFT 2018 //
//////////////////////////////////////////////


//////////////////////////////////////////////

Shader "2DShaderCollection/InfiniteZoom"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
AnimatedInfiniteZoomUV_AnimatedInfiniteZoomUV_Zoom_1("AnimatedInfiniteZoomUV_AnimatedInfiniteZoomUV_Zoom_1", Range(-1, 4)) = 1
AnimatedInfiniteZoomUV_AnimatedInfiniteZoomUV_PosX_1("AnimatedInfiniteZoomUV_AnimatedInfiniteZoomUV_PosX_1", Range(-1, 2)) = 0
AnimatedInfiniteZoomUV_AnimatedInfiniteZoomUV_PosY_1("AnimatedInfiniteZoomUV_AnimatedInfiniteZoomUV_PosY_1", Range(-1, 2)) = 0
AnimatedInfiniteZoomUV_AnimatedInfiniteZoomUV_Intensity_1("AnimatedInfiniteZoomUV_AnimatedInfiniteZoomUV_Intensity_1", Range(0, 4)) = 1
AnimatedInfiniteZoomUV_AnimatedInfiniteZoomUV_Speed_1("AnimatedInfiniteZoomUV_AnimatedInfiniteZoomUV_Speed_1", Range(-10, 10)) = 1
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
float AnimatedInfiniteZoomUV_AnimatedInfiniteZoomUV_Zoom_1;
float AnimatedInfiniteZoomUV_AnimatedInfiniteZoomUV_PosX_1;
float AnimatedInfiniteZoomUV_AnimatedInfiniteZoomUV_PosY_1;
float AnimatedInfiniteZoomUV_AnimatedInfiniteZoomUV_Intensity_1;
float AnimatedInfiniteZoomUV_AnimatedInfiniteZoomUV_Speed_1;
float _LerpUV_Fade_1;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}


float2 AnimatedInfiniteZoomUV(float2 uv, float zoom2, float posx, float posy, float radius, float speed)
{
uv+=float2(posx,posy);
float2 muv = uv;
float atans = (atan2(uv.x - 0.5, uv.y - 0.5) + 3.1415) / (3.1415 * 2.);
float time = _Time * speed*10;
uv -= 0.5;
 uv *= (1. / pow(4., frac(time / 2.)));
uv += 0.5;
float2 tri = abs(1. - (uv * 2.));
 float zoom = min(pow(2., floor(-log2(tri.x))), pow(2., floor(-log2(tri.y))));
 float zoom_id = log2(zoom) + 1.;
 float div = ((pow(2., ((-zoom_id) - 1.)) * ((-2.) + pow(2., zoom_id))));
 float2 uv2 = (((uv) - (div)) * zoom);
 uv2 = lerp(muv * radius, uv2 * radius, zoom2);
 return uv2;
}
float4 frag (v2f i) : COLOR
{
float2 AnimatedInfiniteZoomUV_1 = AnimatedInfiniteZoomUV(i.texcoord,AnimatedInfiniteZoomUV_AnimatedInfiniteZoomUV_Zoom_1,AnimatedInfiniteZoomUV_AnimatedInfiniteZoomUV_PosX_1,AnimatedInfiniteZoomUV_AnimatedInfiniteZoomUV_PosY_1,AnimatedInfiniteZoomUV_AnimatedInfiniteZoomUV_Intensity_1,AnimatedInfiniteZoomUV_AnimatedInfiniteZoomUV_Speed_1);
i.texcoord = lerp(i.texcoord,AnimatedInfiniteZoomUV_1,_LerpUV_Fade_1);
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
