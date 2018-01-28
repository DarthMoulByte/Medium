Shader "Custom/Spline" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_GlowTex ("Glow", 2D) = "black" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_ScrollSpeed ("Scroll Speed", Vector) = (0,1,0,0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _GlowTex;

		struct Input {
			float2 uv_MainTex;
			float2 uv_GlowTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float4 _ScrollSpeed;

		void surf (Input IN, inout SurfaceOutputStandard o) {

			float2 scrollingUV = IN.uv_MainTex + _Time.y * _ScrollSpeed;
			float2 scrollingUVGlow = IN.uv_GlowTex + _Time.y * _ScrollSpeed;

			fixed4 c = tex2D (_MainTex, scrollingUV);
			fixed4 glow = tex2D (_GlowTex, scrollingUVGlow);

			o.Albedo = c.rgb;
			o.Emission = glow;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
