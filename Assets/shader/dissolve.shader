Shader "Custom/dissolve"
{
	Properties
	{
		_MainTex("Main Tex",2D) = "White"{}
		_MainTex2("Main Tex 2", 2D) = "White" {}
		_Color ("Color", Color) = (1,1,1,1)
		_DissolveTex("Dissolve Tex",2D)= "White"{}
		_DissolveAmount("Cut Out", Range(0.0, 1.0)) = 1.0
	}

	SubShader
	{
		Pass
		{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata {
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
					float3 normal : NORMAL;
				};

				struct v2f {
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
				};

				sampler2D _MainTex;
				sampler2D _MainTex2;
				float4 _Color;
				sampler2D _DissolveTex;
				float _DissolveAmount;

				v2f vert (appdata IN) {
					v2f OUT;

					OUT.vertex = UnityObjectToClipPos(IN.vertex);
					OUT.uv = IN.uv;

					return OUT;
				}

				fixed4 frag (v2f IN) : SV_TARGET {
					float4 dissolveColor = tex2D(_DissolveTex, IN.uv);
					fixed4 col = (dissolveColor.r - _DissolveAmount) < 0.0 ? tex2D(_MainTex2, IN.uv) : tex2D(_MainTex, IN.uv);
					//clip(dissolveColor.rgb - _DissolveAmount);

					return col * _Color;					
				}
			ENDCG
		}
	}
}
