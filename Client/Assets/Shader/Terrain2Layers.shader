Shader "Custom/Terrain2Layers" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)

		_MainTex ("Texture 0 (RGB)", 2D) = "white" {}
		_Glossiness("Tex0 Smoothness", Range(0,1)) = 0.5

		_Tex1("Texture 1 (RGB)", 2D) = "white" {}
		_GlossinessTex1("Tex1 Smoothness", Range(0,1)) = 0.5

		_Layer1 ("Layer1 (RGB)", 2D) = "white" {}

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf StandardSpecular fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _Tex1;
		sampler2D _Layer1;

		struct Input {
			float2 uv_MainTex;
			float2 uv_Tex1;
			float2 uv_Layer1;
		};

		half _Glossiness;
		half _GlossinessTex1;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandardSpecular o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;

			fixed4 tex1 = tex2D(_Tex1, IN.uv_Tex1) * _Color;

			fixed4 layer1 = tex2D(_Layer1, IN.uv_Layer1);

			o.Albedo = lerp(c, tex1, layer1.r);
			o.Smoothness = lerp(_Glossiness, _GlossinessTex1, layer1.r);
			o.Alpha = lerp(c.a, tex1.a, layer1.r);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
