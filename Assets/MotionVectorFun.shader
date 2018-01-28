Shader "Hidden/MotionVectorFun"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MotionMultiplier ("Motion Multiplier", float) = 1.0
		_ColorInputTexture ("Color Input Texture", 2D) = "black" {}
	}

	CGINCLUDE

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

	v2f vert (appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;
		return o;
	}
	
	sampler2D _MainTex;
	sampler2D _FrameBuffer;
	sampler2D _ColorInputTexture;
	sampler2D_half _CameraMotionVectorsTexture;
	sampler2D _CameraDepthNormals;

	float _MotionMultiplier;
	float _Mix;

	fixed4 displaceWithMotionVectors (v2f i) : SV_Target
	{
		float depth;
		float3 normals;

		float2 uv = i.uv;

		//#if UNITY_UV_STARTS_AT_TOP
		//uv.y = 1-uv.y;
		//#endif

		DecodeDepthNormal(tex2D(_CameraDepthNormals, uv), depth, normals);
		float4 motionVectors = tex2D(_CameraMotionVectorsTexture, uv);
		float4 scaledMotionVectors =  motionVectors * _MotionMultiplier;

		float2 displacedScreenUV =  uv + scaledMotionVectors.xy;

		float4 frameBuffer = tex2D(_FrameBuffer, displacedScreenUV);
		fixed4 col = tex2D(_MainTex, displacedScreenUV);

		float4 colorTexture = tex2D(_ColorInputTexture, displacedScreenUV + float2(_Time.y * 0.1, 0));

		float4 mixed = lerp(col, frameBuffer, _Mix);

		mixed = lerp(mixed, colorTexture, _Mix);

		//if(length(mixed.rgb) < 0.001)
		//{
		//	mixed = 0.1;
		//}
		return mixed;
		
		
		return col;
	}	

	fixed4 passItOn	 (v2f i) : SV_Target
	{
		fixed4 col = tex2D(_MainTex, i.uv);

		return col;
	}

	ENDCG

	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment passItOn
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment displaceWithMotionVectors
			ENDCG
		}
	}
}
