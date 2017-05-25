Shader "Tango/ColorPointCloud"
{

Properties
{
    _PointSize("Point Size", Float) = 5.0
}

SubShader
{

CGINCLUDE

#include "UnityCG.cginc"

struct appdata
{
    float4 vertex : POSITION;
};

struct v2f
{
    float4 vertex : SV_POSITION;
    float4 color : COLOR;
    float size : PSIZE;
};

float _PointSize;

v2f vert(appdata v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.size = _PointSize;
    //o.color = mul(depthCameraTUnityWorld, v.vertex);
    float4 vpos = mul(UNITY_MATRIX_MV, v.vertex);
    float dist = length(vpos.xyz);
    o.color.r = (3.f - dist) / 3.f;
    o.color.g = dist;
    o.color.b = dist / 3.f;
    return o;
}

fixed4 frag(v2f i) : SV_Target
{
    return i.color;
}

ENDCG

Pass 
{
    CGPROGRAM
    #pragma vertex vert
    #pragma fragment frag
    ENDCG
}

}

}