// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/VotanicXR/vCastPanoramicPostProcess"
{
	Properties
	{
		_MainTex("Main Texture", 2DArray) = "grey" {}
	}

	HLSLINCLUDE

	#pragma target 4.5

	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
	#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
	#include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/FXAA.hlsl"
	#include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/RTUpscale.hlsl"

	TEXTURE2D_X(_MainTex);
	float _Enable;
	float _RenderSpan;
	float _DisplaySpan;
	float _KeepAspectRatio;
	float _DisplayRectWidth;
	float _DisplayRectHeight;

	float4x4 unity_MatrixVP;
	float4x4 unity_ObjectToWorld;
	float4x4 unity_ObjectToClip;

	struct appdata
	{
		uint vertexID : SV_VertexID;
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
		UNITY_SETUP_INSTANCE_ID(v);
		float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
		o.vertex = mul(unity_MatrixVP, worldPos);
		o.uv = mul(unity_ObjectToClip, v.vertex) / float2(_DisplayRectWidth, _DisplayRectHeight); // also remove influence from display rect
		return o;
	}
		
	float4 frag (v2f i) : SV_Target
	{
		// declare variables
		float azim, height, scaleFactor, aspectRatioCompenstaion;
		float get_s, get_t;
		float4 col;

		if (_Enable > 0.5)
		{
			// calculate scale factor needed for render and display angle differences
			scaleFactor = tan(_DisplaySpan / 2) / tan(_RenderSpan / 2);

			// do if aspect ratio compansation is necessary
			if (_KeepAspectRatio > 0.5)
			{
				aspectRatioCompenstaion = _DisplaySpan / 2 / tan(_DisplaySpan / 2);
			}
			else
			{
				aspectRatioCompenstaion = 1;
			}

			// compute the expanded cylinderical unwarp coordinates
			azim = (i.uv.x - 0.5) * _DisplaySpan;
			height = (i.uv.y - 0.5) * scaleFactor;

			get_s = tan(azim) / tan(_RenderSpan / 2.0) / 2.0 + 0.5;
			get_t = height / cos(azim) * aspectRatioCompenstaion + 0.5;

			// retrieve target value from calculated coordinates, create horizontal black border at out of bound positions
			//if (abs(get_t - 0.5) > 0.5) // change to this conditional to see the wrapped border, debug purpose only
			if (abs(i.uv.y - 0.5) > 0.5 * cos(_DisplaySpan / 2) / aspectRatioCompenstaion / scaleFactor)
			{
				col = float4(0.0, 0.0, 0.0, 1.0);
			}
			else
			{
				col = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, float2(get_s, get_t) * float2(_DisplayRectWidth, _DisplayRectHeight)); // reintroduce influence from display rect
			}
		}
		else
		{
			col = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, i.uv);
		}
		return col;
	}
	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDHLSL
		}
	}

	Fallback Off
}
