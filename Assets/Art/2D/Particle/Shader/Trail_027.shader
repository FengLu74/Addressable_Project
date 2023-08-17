// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Trail_027"
{
	Properties
	{
		_Flow("Flow", 2D) = "white" {}
		_Flow1("Flow", 2D) = "white" {}
		_Flow2("Flow", 2D) = "white" {}
		_Speed_flow("Speed_flow", Vector) = (1,0,0,0)
		_Start_Power("Start_Power", Float) = 3
		_Speed_noise("Speed_noise", Vector) = (1,0.5,0,0)
		_TuoWeiChangDu("TuoWeiChangDu", Float) = -0.5
		_Color1("Color 1", Color) = (0,0.6226397,1,0)
		_Color0("Color 0", Color) = (1,0,0.6024933,0)
		_liangdu("liangdu", Float) = 4
		_JianBian("JianBian", Float) = 1.5
		_TuoWeiControler_1("TuoWeiControler_1", Float) = -0.39
		_TuoWeiControler_2("TuoWeiControler_2", Float) = 0.01
		_Alpha("Alpha", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform half4 _Color0;
		uniform half4 _Color1;
		uniform half _JianBian;
		uniform half _liangdu;
		uniform sampler2D _Flow1;
		uniform sampler2D _Flow;
		uniform half2 _Speed_flow;
		uniform half _TuoWeiControler_1;
		uniform half _TuoWeiControler_2;
		uniform sampler2D _Flow2;
		uniform half2 _Speed_noise;
		uniform half _TuoWeiChangDu;
		uniform half _Start_Power;
		uniform half _Alpha;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			half u118 = i.uv_texcoord.x;
			half4 lerpResult151 = lerp( _Color0 , _Color1 , ( u118 * _JianBian ));
			half2 panner114 = ( 1.0 * _Time.y * _Speed_flow + i.uv_texcoord);
			half2 UV147 = i.uv_texcoord;
			half2 panner131 = ( 1.0 * _Time.y * _Speed_noise + UV147);
			half clampResult135 = clamp( ( ( ( 1.0 - i.uv_texcoord.x ) + _TuoWeiChangDu ) * 1.0 ) , 0.0 , 1.0 );
			half v119 = i.uv_texcoord.y;
			half mask124 = ( i.uv_texcoord.y * ( u118 * 2.0 ) * ( 1.0 - u118 ) * ( 1.0 - v119 ) * _Start_Power );
			half temp_output_89_0 = ( i.vertexColor.a * tex2D( _Flow1, ( ( ( ( tex2D( _Flow, panner114 ).r + _TuoWeiControler_1 ) * _TuoWeiControler_2 ) * ( 1.0 - u118 ) * 30.0 ) + UV147 ) ).r * ( ( tex2D( _Flow2, panner131 ).r + 0.5 ) - clampResult135 ) * mask124 * _Alpha );
			half4 appendResult102 = (half4(( lerpResult151 * _liangdu * i.vertexColor ).rgb , temp_output_89_0));
			o.Emission = appendResult102.xyz;
			o.Alpha = temp_output_89_0;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Unlit alpha:fade keepalpha fullforwardshadows noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 

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
				float3 worldPos : TEXCOORD2;
				half4 color : COLOR0;
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
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.color = v.color;
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
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.vertexColor = IN.color;
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
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
Node;AmplifyShaderEditor.CommentaryNode;143;-1917.378,-1326.412;Inherit;False;1124.671;705.6717;Comment;11;137;134;141;130;142;131;129;132;133;135;140;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;19;-1959.182,-599.6878;Inherit;False;974.8003;716.3999;mask;8;118;119;121;122;123;124;160;167;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;6;-1661.954,122.8807;Inherit;False;609.2848;166.2953;Time;3;125;126;127;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;125;-1617.121,187.6445;Inherit;False;Constant;_Float3;Float 3;3;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;126;-1470.496,190.7314;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;30;-1948.31,-2059.441;Inherit;True;Property;_Flow;Flow;0;0;Create;True;0;0;0;False;0;False;555847d3c04f3c649bbb6157c80f4dc8;70c887586beeaaf4683cc15cc2715b79;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;29;-1644.31,-1835.441;Inherit;True;Property;_TextureSample1;Texture Sample 0;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;52;-1324.31,-1835.441;Inherit;True;True;True;True;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;114;-1932.31,-1851.441;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;36;-2140.311,-1819.441;Inherit;False;Property;_Speed_flow;Speed_flow;3;0;Create;True;0;0;0;False;0;False;1,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;141;-1141.724,-1248.325;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;130;-1442.938,-1276.412;Inherit;True;Property;_TextureSample0;Texture Sample 0;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;131;-1692.271,-1273.976;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;129;-1666.009,-1061.649;Inherit;True;Property;_Flow2;Flow;2;0;Create;True;0;0;0;False;0;False;555847d3c04f3c649bbb6157c80f4dc8;70c887586beeaaf4683cc15cc2715b79;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;-780.3099,-1819.441;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;66;-534.9124,-1831.521;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;115;-2300.917,-874.1714;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;147;-2039.251,-960.322;Inherit;False;UV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;87;-1502.21,-2226.641;Inherit;True;Property;_Flow1;Flow;1;0;Create;True;0;0;0;False;0;False;9951c27af6c06fa4fbd0687c2909cfdb;6ce99efdba3b85d47904f4e8c2582066;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.FunctionNode;133;-1568.749,-874.7407;Inherit;True;ConstantBiasScale;-1;;8;63208df05c83e8e49a48ffbdce2e43a0;0;3;3;FLOAT;0;False;1;FLOAT;-0.2;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;151;-54.60394,-2337.999;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;153;6.22071,-2044.188;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;102;513.7612,-1972.667;Inherit;True;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;154;282.2512,-2256.772;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;152;-357.5606,-2111.457;Inherit;False;118;u;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;157;-164.5227,-2115.607;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2.9;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;89;191.5056,-1864.953;Inherit;True;5;5;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;88;-266.4964,-1865.537;Inherit;True;Property;_TextureSample2;Texture Sample 2;5;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;145;-746.334,-1559.71;Inherit;True;147;UV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;70;-1115.993,-1390.457;Inherit;False;Constant;_Float2;Float 2;4;0;Create;True;0;0;0;False;0;False;30;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;65;-1145.067,-1594.898;Inherit;True;118;u;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;159;-932.6968,-1576.156;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;161;-1352.059,-1603.695;Inherit;False;Property;_TuoWeiControler_1;TuoWeiControler_1;11;0;Create;True;0;0;0;False;0;False;-0.39;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;43;-1073.075,-1832.632;Inherit;True;ConstantBiasScale;-1;;9;63208df05c83e8e49a48ffbdce2e43a0;0;3;3;FLOAT;0;False;1;FLOAT;-0.45;False;2;FLOAT;0.01;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;140;-998.2184,-980.9135;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;135;-1304.021,-884.498;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;134;-1832.316,-857.9906;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;137;-1842.614,-748.9496;Inherit;False;Property;_TuoWeiChangDu;TuoWeiChangDu;6;0;Create;True;0;0;0;False;0;False;-0.5;-0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;990.3154,-1918.962;Half;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Trail_027;False;False;False;False;True;True;True;True;True;True;True;True;False;False;True;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.RangedFloatNode;163;-26.28277,-1465.892;Inherit;False;Property;_Alpha;Alpha;13;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;156;-31.44042,-1619.289;Inherit;False;124;mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;158;-349.5228,-2008.706;Half;False;Property;_JianBian;JianBian;10;0;Create;True;0;0;0;False;0;False;1.5;1.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;155;29.45135,-2120.773;Inherit;False;Property;_liangdu;liangdu;9;0;Create;True;0;0;0;False;0;False;4;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;162;-1366.634,-1503.589;Inherit;False;Property;_TuoWeiControler_2;TuoWeiControler_2;12;0;Create;True;0;0;0;False;0;False;0.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;142;-1320.862,-1031.11;Inherit;False;Constant;_Float5;Float 2;4;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;132;-1899.747,-1257.34;Inherit;False;Property;_Speed_noise;Speed_noise;5;0;Create;True;0;0;0;False;0;False;1,0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;118;-1910.676,-555.8577;Inherit;True;u;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;121;-1390.281,-550.4491;Inherit;True;5;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;119;-1898.072,-254.3712;Inherit;True;v;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;122;-1642.456,-205.5159;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;160;-1633.206,-422.4216;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;167;-1615.899,-549.9163;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;124;-1164.548,-536.811;Inherit;False;mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;150;-354.6172,-2506.529;Inherit;False;Property;_Color1;Color 1;7;0;Create;True;0;0;0;False;0;False;0,0.6226397,1,0;0,0.6226397,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;149;-349.3486,-2319.72;Inherit;False;Property;_Color0;Color 0;8;0;Create;True;0;0;0;False;0;False;1,0,0.6024933,0;1,0,0.6024933,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;127;-1254.417,192.2748;Inherit;False;time;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;123;-1627.432,-102.7161;Inherit;False;Property;_Start_Power;Start_Power;4;0;Create;True;0;0;0;False;0;False;3;3;0;0;0;1;FLOAT;0
WireConnection;126;0;125;0
WireConnection;29;0;30;0
WireConnection;29;1;114;0
WireConnection;52;0;29;1
WireConnection;114;0;115;0
WireConnection;114;2;36;0
WireConnection;141;0;130;1
WireConnection;141;1;142;0
WireConnection;130;0;129;0
WireConnection;130;1;131;0
WireConnection;131;0;147;0
WireConnection;131;2;132;0
WireConnection;64;0;43;0
WireConnection;64;1;159;0
WireConnection;64;2;70;0
WireConnection;66;0;64;0
WireConnection;66;1;145;0
WireConnection;147;0;115;0
WireConnection;133;3;134;0
WireConnection;133;1;137;0
WireConnection;151;0;149;0
WireConnection;151;1;150;0
WireConnection;151;2;157;0
WireConnection;102;0;154;0
WireConnection;102;3;89;0
WireConnection;154;0;151;0
WireConnection;154;1;155;0
WireConnection;154;2;153;0
WireConnection;157;0;152;0
WireConnection;157;1;158;0
WireConnection;89;0;153;4
WireConnection;89;1;88;1
WireConnection;89;2;140;0
WireConnection;89;3;156;0
WireConnection;89;4;163;0
WireConnection;88;0;87;0
WireConnection;88;1;66;0
WireConnection;159;0;65;0
WireConnection;43;3;52;0
WireConnection;43;1;161;0
WireConnection;43;2;162;0
WireConnection;140;0;141;0
WireConnection;140;1;135;0
WireConnection;135;0;133;0
WireConnection;134;0;115;1
WireConnection;0;2;102;0
WireConnection;0;9;89;0
WireConnection;118;0;115;1
WireConnection;121;0;115;2
WireConnection;121;1;167;0
WireConnection;121;2;160;0
WireConnection;121;3;122;0
WireConnection;121;4;123;0
WireConnection;119;0;115;2
WireConnection;122;0;119;0
WireConnection;160;0;118;0
WireConnection;167;0;118;0
WireConnection;124;0;121;0
WireConnection;127;0;126;0
ASEEND*/
//CHKSM=FD9878283D2A7C5774DB44E4A259BCB53DC57E80