Shader "Custom/CenterEdgeScrollTexture"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color",Color) = (1,1,1,1)
		_Distort("Distort", vector) = (0.5, 0.5, 1.0, 1.0)
		_OuterRadius("Outer Radius", float) = 0.5
		_InnterRadius("Inner Radius", float) = -0.5
		_Hardness("Hardness", float) = 1.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

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

			sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			float4 _Color, _Distort;
			float _OuterRadius, _InnerRadius, _Hardness;
			v2f vert (appdata IN)
			{
				v2f o;
				float4 pos = UnityObjectToClipPos(IN.vertex);
				o.vertex = pos;
				o.uv = IN.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				float2 tc = i.uv;
				float2 p = -1.0 + 2.0 * tc;
				float len = length(p);
				float2 uv = tc + (p / len) * cos(len * 12.0 - _Time.y * 10.0) * 0.03;
				fixed4 col = tex2D(_MainTex, uv);
				return col * _Color;
			}
			ENDCG
		}
	}
}
