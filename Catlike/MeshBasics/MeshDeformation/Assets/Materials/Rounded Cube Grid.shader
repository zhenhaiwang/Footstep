Shader "Custom/Rounded Cube Grid" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		[KeywordEnum(X, Y, Z)] _Faces("Faces", Float) = 0	// We need to use different vertex coordinates for them.
															// So we have a choice to make, which we can support by adding a keyword enumeration shader property.
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Depending on which option you select, Unity will enable a custom shader keyword for the material.
		// We have to tell the shader to create versions of itself for each keyword that we wish to support.
		#pragma shader_feature _FACES_X _FACES_Y _FACES_Z

		#pragma surface surf Standard fullforwardshadows vertex:vert

		#pragma target 3.0

		sampler2D _MainTex;

		// The important bit is that it defines an input structure which expects coordinates for the main texture.
		// These coordinates are used in the surf function, which is invoked for each fragment that is rendered.
		// As we don't have such coordinates, we have to replace uv_MainTex with something else.
		struct Input {
			float2 cubeUV;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		UNITY_INSTANCING_CBUFFER_START(Props)

		UNITY_INSTANCING_CBUFFER_END

		// As the UV are defined per vertex, we have to add a function that is invoked per vertex.
		// To check that our shader works, start with directly using the XY coordinates of the vertex position as UV.
		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);

			// It is possible to check which keyword is defined, which enables us to write different code for each option.

			// We can now use the vertex color instead of its position.
			// As the shader interprets vertex color channels as values in the 0–1 range, we have to undo this conversion by multiplying with 255.

			#if defined(_FACES_X)
				o.cubeUV = v.color.yz * 255;	//o.cubeUV = v.vertex.yz;
			#elif defined(_FACES_Y)
				o.cubeUV = v.color.xz * 255;	//o.cubeUV = v.vertex.xz;
			#elif defined(_FACES_Z)
				o.cubeUV = v.color.xy * 255;	//o.cubeUV = v.vertex.xy;
			#endif
		}

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D (_MainTex, IN.cubeUV) * _Color;
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
