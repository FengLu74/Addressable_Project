// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Card_Biankuang"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_NormalTexture("NormalTexture", 2D) = "bump" {}
		_NormalTexture1("NormalTexture", 2D) = "bump" {}
		_Mask_texture("Mask_texture", 2D) = "white" {}
		_Float0("Float 0", Float) = 1
		_Float1("Float 0", Float) = -3
		_Float2("Float 2", Float) = 360
		_Float3("Float 3", Float) = 180
		_TextureSample1("Texture Sample 1", 2D) = "white" {}
		_Float4("Float 4", Float) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Off
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _TextureSample1;
		uniform float _Float2;
		uniform float _Float3;
		uniform sampler2D _Mask_texture;
		uniform float4 _Mask_texture_ST;
		uniform sampler2D _NormalTexture1;
		uniform float _Float1;
		uniform sampler2D _NormalTexture;
		uniform float4 _NormalTexture_ST;
		uniform float _Float0;
		uniform float _Float4;
		uniform float _Cutoff = 0.5;


		inline float3 ASESafeNormalize(float3 inVec)
		{
			float dp3 = max( 0.001f , dot( inVec , inVec ) );
			return inVec* rsqrt( dp3);
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Normal = float3(0,0,1);
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = Unity_SafeNormalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 worldToObjDir53 = ASESafeNormalize( mul( unity_WorldToObject, float4( ase_worldViewDir, 0 ) ).xyz );
			float cos54 = cos( ( ( _Float2 / _Float3 ) * UNITY_PI ) );
			float sin54 = sin( ( ( _Float2 / _Float3 ) * UNITY_PI ) );
			float2 rotator54 = mul( worldToObjDir53.xy - float2( 0,0 ) , float2x2( cos54 , -sin54 , sin54 , cos54 )) + float2( 0,0 );
			float Viewdir65 = abs( (frac( rotator54.x )*2.0 + -1.0) );
			float2 uv_Mask_texture = i.uv_texcoord * _Mask_texture_ST.xy + _Mask_texture_ST.zw;
			float4 tex2DNode8 = tex2D( _Mask_texture, uv_Mask_texture );
			float Mask13 = tex2DNode8.a;
			float3 temp_cast_1 = (Viewdir65).xxx;
			float2 uv_TexCoord6 = i.uv_texcoord * float2( 3,3 );
			float3 lerpResult40 = lerp( float3( uv_TexCoord6 ,  0.0 ) , ase_worldViewDir , float3( 0.3,0.3,0.3 ));
			float fresnelNdotV46 = dot( normalize( normalize( (WorldNormalVector( i , UnpackScaleNormal( tex2D( _NormalTexture1, lerpResult40.xy ), _Float1 ) )) ) ), temp_cast_1 );
			float fresnelNode46 = ( 0.0 + 1.0 * pow( max( 1.0 - fresnelNdotV46 , 0.0001 ), 3.0 ) );
			float3 temp_cast_4 = (Viewdir65).xxx;
			float2 uv_NormalTexture = i.uv_texcoord * _NormalTexture_ST.xy + _NormalTexture_ST.zw;
			float fresnelNdotV11 = dot( normalize( normalize( (WorldNormalVector( i , UnpackScaleNormal( tex2D( _NormalTexture, uv_NormalTexture ), _Float0 ) )) ) ), temp_cast_4 );
			float fresnelNode11 = ( 0.0 + 1.0 * pow( max( 1.0 - fresnelNdotV11 , 0.0001 ), 3.0 ) );
			float couston75 = ( ( fresnelNode46 * fresnelNode11 ) * Mask13 );
			float2 appendResult77 = (float2(( ( pow( Viewdir65 , 1.0 ) * Mask13 ) + couston75 ) , _Float4));
			float4 ramp80 = tex2D( _TextureSample1, appendResult77 );
			float4 maincolor85 = tex2DNode8;
			o.Emission = ( ( ( ramp80 * 6.0 ) + maincolor85 ) * Mask13 ).rgb;
			o.Alpha = Mask13;
			clip( Mask13 - _Cutoff );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.GetLocalVarNode;14;-675.2826,37.97973;Inherit;False;13;Mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;62;-978.4103,611.5375;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;63;-816.6971,616.2586;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;2;False;2;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;71;-1802.897,1363.242;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;-1620.004,1399.176;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;70;-1966.431,1353.315;Inherit;False;65;Viewdir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;73;-1815.367,1496.857;Inherit;False;13;Mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;64;-596.7583,610.3307;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;77;-1098.72,1458.394;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;75;-87.55807,-185.1565;Inherit;False;couston;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-244.7163,-49.21704;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;78;-1662.903,1566.536;Inherit;False;75;couston;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;80;-524.5486,1393.732;Inherit;False;ramp;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;66;161.5706,-147.5333;Inherit;False;80;ramp;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;65;-467.7393,605.1174;Inherit;False;Viewdir;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-1685.184,-40.88494;Inherit;False;Property;_Float0;Float 0;4;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-442.0034,-372.6241;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;48;-1703.361,-163.7724;Inherit;False;0;4;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-269.7153,-194.7389;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;74;-1399.34,1396.306;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;69;-891.0014,1378.287;Inherit;True;Property;_TextureSample1;Texture Sample 1;8;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;79;-1312.746,1619.017;Inherit;False;Property;_Float4;Float 4;9;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;81;-578.4966,796.1464;Inherit;False;80;ramp;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;84;-447.8967,1045.946;Inherit;False;85;maincolor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;86;-108.3442,912.2612;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;83;-349.6968,833.8464;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;115.2563,983.761;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;87;-108.3439,1087.761;Inherit;False;13;Mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;82;-541.4966,915.1464;Inherit;False;Constant;_Float5;Float 5;11;0;Create;True;0;0;0;False;0;False;6;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;68;608.4992,-57.73309;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Card_Biankuang;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;True;Opaque;;AlphaTest;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.LerpOp;40;-1556.129,-722.6465;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0.3,0.3,0.3;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldNormalVector;47;-1043.702,-724.3758;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;45;-1549.591,-593.563;Inherit;False;Property;_Float1;Float 0;5;0;Create;True;0;0;0;False;0;False;-3;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;6;-1936.168,-744.2205;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;3,3;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FresnelNode;46;-799.4751,-784.0807;Inherit;True;Standard;HalfVector;ViewDir;True;True;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;10;-886.1058,-187.7255;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.FresnelNode;11;-721.6553,-230.3298;Inherit;True;Standard;WorldNormal;ViewDir;True;True;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;39;-1784.882,-589.2932;Inherit;False;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SamplerNode;43;-1359.475,-717.7159;Inherit;True;Property;_NormalTexture1;NormalTexture;2;0;Create;True;0;0;0;False;0;False;4;4bdadecd23c587c46b03e6bdfe3c16bd;42ab2f5554c3a2f4590f4cadf302ed13;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-1288.817,-174.4295;Inherit;True;Property;_NormalTexture;NormalTexture;1;0;Create;True;0;0;0;False;0;False;43;df0ece2033840a345a16417a9b160b8e;42ab2f5554c3a2f4590f4cadf302ed13;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;13;-820.7763,249.7193;Inherit;True;Mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;8;-1386.13,173.7298;Inherit;True;Property;_Mask_texture;Mask_texture;3;0;Create;True;0;0;0;False;0;False;-1;79814bd5fc874714d803293addc07a4b;79814bd5fc874714d803293addc07a4b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;85;-980.7175,148.2264;Inherit;False;maincolor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;58;-1776.279,831.9846;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PiNode;60;-1867.169,1033.83;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;61;-1103.458,601.8094;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;55;-2042.806,797.5294;Inherit;False;Property;_Float2;Float 2;6;0;Create;True;0;0;0;False;0;False;360;360;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;56;-1945.075,888.6432;Inherit;False;Property;_Float3;Float 3;7;0;Create;True;0;0;0;False;0;False;180;180;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;52;-1864.874,568.53;Inherit;False;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TransformDirectionNode;53;-1613.572,606.4365;Inherit;False;World;Object;True;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RotatorNode;54;-1320.288,615.9709;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;59;-1645.918,844.4699;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
WireConnection;62;0;61;0
WireConnection;63;0;62;0
WireConnection;71;0;70;0
WireConnection;72;0;71;0
WireConnection;72;1;73;0
WireConnection;64;0;63;0
WireConnection;77;0;74;0
WireConnection;77;1;79;0
WireConnection;75;0;12;0
WireConnection;80;0;69;0
WireConnection;65;0;64;0
WireConnection;49;0;46;0
WireConnection;49;1;11;0
WireConnection;12;0;49;0
WireConnection;12;1;14;0
WireConnection;74;0;72;0
WireConnection;74;1;78;0
WireConnection;69;1;77;0
WireConnection;86;0;83;0
WireConnection;86;1;84;0
WireConnection;83;0;81;0
WireConnection;83;1;82;0
WireConnection;88;0;86;0
WireConnection;88;1;87;0
WireConnection;68;2;88;0
WireConnection;68;9;13;0
WireConnection;68;10;13;0
WireConnection;40;0;6;0
WireConnection;40;1;39;0
WireConnection;47;0;43;0
WireConnection;46;0;47;0
WireConnection;46;4;65;0
WireConnection;10;0;4;0
WireConnection;11;0;10;0
WireConnection;11;4;65;0
WireConnection;43;1;40;0
WireConnection;43;5;45;0
WireConnection;4;1;48;0
WireConnection;4;5;5;0
WireConnection;13;0;8;4
WireConnection;85;0;8;0
WireConnection;58;0;55;0
WireConnection;58;1;56;0
WireConnection;61;0;54;0
WireConnection;53;0;52;0
WireConnection;54;0;53;0
WireConnection;54;2;59;0
WireConnection;59;0;58;0
WireConnection;59;1;60;0
ASEEND*/
//CHKSM=BB32E6DFE14B445BE98CC6F6A465EEAAC504A07C