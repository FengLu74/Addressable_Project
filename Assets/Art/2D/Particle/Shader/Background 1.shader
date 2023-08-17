// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Background"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Depth_texture("Depth_texture", 2D) = "white" {}
		_Float3("Front_Depth", Float) = 0
		_Float2("Mid_Depth", Float) = 0
		_Float1("Depth", Float) = 0
		_Mid_texture_smoke_2("Mid_texture_smoke_2", 2D) = "white" {}
		_RotateAngle("RotateAngle", Range( -1 , 360)) = 360
		_Main_texture("Main_texture", 2D) = "white" {}
		_Float6("Float 6", Float) = 180
		_Flowmap_texture("Flowmap_texture", 2D) = "white" {}
		_ramp("ramp", 2D) = "white" {}
		_Light("Light", Float) = 1
		_Float4("Float 4", Float) = 0.5
		_Left("Left", Float) = 1
		_Right("Right", Float) = 1
		_UP("UP", Float) = 1
		_Down("Down", Float) = 1
		_Particle_Texture("Particle_Texture", 2D) = "white" {}
		_Particle_speed("Particle_speed", Vector) = (1,1,0,0)
		_Particle_Voronoi_scale("Particle_Voronoi_scale", Float) = 5
		_Particle_Alpha_Power("Particle_Alpha_Power", Vector) = (0.1,0.6,0,0)
		[HDR]_Color0("Color 0", Color) = (0,1,0.7890246,0)
		_particle_offset("particle_offset", Vector) = (1,1,0,0)
		_particle_tiling("particle_tiling", Vector) = (1,1,0,0)
		_Mid_noise_scale("Mid_noise_scale", Float) = 15
		_Particle_noise_scale("Particle_noise_scale", Float) = 5
		_Particle_noise_power("Particle_noise_power", Float) = 5
		_Particle_Alpha_speed("Particle_Alpha_speed", Float) = 0
		_Particle_noise_speed("Particle_noise_speed", Float) = 0
		_Mid_noise_speed("Mid_noise_speed", Float) = 0
		[HDR]_Color1("Color 1", Color) = (1,0.4752927,0,0)
		_FLowmap_lerp("FLowmap_lerp", Float) = 0.2
		_Flowmap_speed("Flowmap_speed", Vector) = (1,1,0,0)
		_Flowmap_X("Flowmap_X", Vector) = (-0.5,1,0,0)
		_Flowmap_Y("Flowmap_Y", Vector) = (-0.5,1,0,0)
		_FlowMap_scale("FlowMap_scale", Float) = 0
		_Flowmap_smoothstep("Flowmap_smoothstep", Vector) = (0.1,0.4,0,0)
		_FLowmap_mask_power("FLowmap_mask_power", Float) = 1
		_Mid_texture("Mid_texture", 2D) = "white" {}
		_mid_texture_light("mid_texture_light", Float) = 4
		_Mid_texture_smoke("Mid_texture_smoke", 2D) = "white" {}
		_Mid_Noiseflowspeed_3("Mid_Noiseflowspeed_3", Vector) = (-0.5,0,0,0)
		_Mid_texture_Noiseflowspped1("Mid_texture_Noiseflowspped", Vector) = (1,-1,0,0)
		_Mid_Noiseflowspeed_1("Mid_Noiseflowspeed_1", Vector) = (-0.2,0,0,0)
		_Mid_Noiseflowspeed_2("Mid_Noiseflowspeed_2", Vector) = (-0.5,-0.5,0,0)
		_Mid_texture_NoisePower("Mid_texture_NoisePower", Float) = 0.2
		_Mid_NoiseUVTilling("Mid_NoiseUVTilling", Vector) = (1,1,0,0)
		_Mid_noisepower_1("Mid_noisepower_1", Float) = 0.17
		_Mid_NoiseMask_1("Mid_NoiseMask_1", Vector) = (-0.33,2.53,0,0)
		_Mid_NoiseMask_2("Mid_NoiseMask_2", Vector) = (-0.17,2.53,0,0)
		_Mid_NoiseUVOffset("Mid_NoiseUVOffset", Vector) = (0,1,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
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
			float3 viewDir;
			INTERNAL_DATA
			float3 worldNormal;
		};

		uniform sampler2D _ramp;
		uniform float _RotateAngle;
		uniform float _Float6;
		uniform float _Right;
		uniform float _Left;
		uniform float _UP;
		uniform float _Down;
		uniform float _Float4;
		uniform float _Mid_noise_scale;
		uniform float _Mid_noise_speed;
		uniform float2 _Mid_Noiseflowspeed_3;
		uniform sampler2D _Depth_texture;
		uniform float _Float2;
		uniform float4 _Depth_texture_ST;
		uniform sampler2D _Mid_texture;
		uniform float2 _Mid_Noiseflowspeed_2;
		uniform float2 _Mid_NoiseUVTilling;
		uniform float2 _Mid_NoiseUVOffset;
		uniform sampler2D _Mid_texture_smoke;
		uniform float2 _Mid_Noiseflowspeed_1;
		uniform float2 _Mid_NoiseMask_1;
		uniform float _Mid_noisepower_1;
		uniform float _Mid_texture_NoisePower;
		uniform float2 _Mid_NoiseMask_2;
		uniform sampler2D _Mid_texture_smoke_2;
		uniform float2 _Mid_texture_Noiseflowspped1;
		uniform float _mid_texture_light;
		uniform sampler2D _Main_texture;
		uniform float _Float1;
		uniform sampler2D _Flowmap_texture;
		uniform float2 _Flowmap_smoothstep;
		uniform float2 _Flowmap_speed;
		uniform float _FlowMap_scale;
		uniform float2 _Flowmap_Y;
		uniform float2 _Flowmap_X;
		uniform float _FLowmap_mask_power;
		uniform float _FLowmap_lerp;
		uniform float _Light;
		uniform float2 _Particle_Alpha_Power;
		uniform float _Particle_Voronoi_scale;
		uniform float _Particle_Alpha_speed;
		uniform float _Float3;
		uniform sampler2D _Particle_Texture;
		uniform float2 _Particle_speed;
		uniform float2 _particle_tiling;
		uniform float2 _particle_offset;
		uniform float _Particle_noise_scale;
		uniform float _Particle_noise_speed;
		uniform float _Particle_noise_power;
		uniform float4 _Color0;
		uniform float4 _Color1;
		uniform float _Cutoff = 0.5;


		inline float3 ASESafeNormalize(float3 inVec)
		{
			float dp3 = max( 0.001f , dot( inVec , inVec ) );
			return inVec* rsqrt( dp3);
		}


		float2 voronoihash310( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi310( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
		{
			float2 n = floor( v );
			float2 f = frac( v );
			float F1 = 8.0;
			float F2 = 8.0; float2 mg = 0;
			for ( int j = -1; j <= 1; j++ )
			{
				for ( int i = -1; i <= 1; i++ )
			 	{
			 		float2 g = float2( i, j );
			 		float2 o = voronoihash310( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
					float d = 0.5 * dot( r, r );
			 		if( d<F1 ) {
			 			F2 = F1;
			 			F1 = d; mg = g; mr = r; id = o;
			 		} else if( d<F2 ) {
			 			F2 = d;
			
			 		}
			 	}
			}
			return F1;
		}


		inline float2 POM( sampler2D heightMap, float2 uvs, float2 dx, float2 dy, float3 normalWorld, float3 viewWorld, float3 viewDirTan, int minSamples, int maxSamples, float parallax, float refPlane, float2 tilling, float2 curv, int index )
		{
			float3 result = 0;
			int stepIndex = 0;
			int numSteps = ( int )lerp( (float)maxSamples, (float)minSamples, saturate( dot( normalWorld, viewWorld ) ) );
			float layerHeight = 1.0 / numSteps;
			float2 plane = parallax * ( viewDirTan.xy / viewDirTan.z );
			uvs.xy += refPlane * plane;
			float2 deltaTex = -plane * layerHeight;
			float2 prevTexOffset = 0;
			float prevRayZ = 1.0f;
			float prevHeight = 0.0f;
			float2 currTexOffset = deltaTex;
			float currRayZ = 1.0f - layerHeight;
			float currHeight = 0.0f;
			float intersection = 0;
			float2 finalTexOffset = 0;
			while ( stepIndex < numSteps + 1 )
			{
			 	currHeight = tex2Dgrad( heightMap, uvs + currTexOffset, dx, dy ).r;
			 	if ( currHeight > currRayZ )
			 	{
			 	 	stepIndex = numSteps + 1;
			 	}
			 	else
			 	{
			 	 	stepIndex++;
			 	 	prevTexOffset = currTexOffset;
			 	 	prevRayZ = currRayZ;
			 	 	prevHeight = currHeight;
			 	 	currTexOffset += deltaTex;
			 	 	currRayZ -= layerHeight;
			 	}
			}
			int sectionSteps = 2;
			int sectionIndex = 0;
			float newZ = 0;
			float newHeight = 0;
			while ( sectionIndex < sectionSteps )
			{
			 	intersection = ( prevHeight - prevRayZ ) / ( prevHeight - currHeight + currRayZ - prevRayZ );
			 	finalTexOffset = prevTexOffset + intersection * deltaTex;
			 	newZ = prevRayZ - intersection * layerHeight;
			 	newHeight = tex2Dgrad( heightMap, uvs + finalTexOffset, dx, dy ).r;
			 	if ( newHeight > newZ )
			 	{
			 	 	currTexOffset = finalTexOffset;
			 	 	currHeight = newHeight;
			 	 	currRayZ = newZ;
			 	 	deltaTex = intersection * deltaTex;
			 	 	layerHeight = intersection * layerHeight;
			 	}
			 	else
			 	{
			 	 	prevTexOffset = finalTexOffset;
			 	 	prevHeight = newHeight;
			 	 	prevRayZ = newZ;
			 	 	deltaTex = ( 1 - intersection ) * deltaTex;
			 	 	layerHeight = ( 1 - intersection ) * layerHeight;
			 	}
			 	sectionIndex++;
			}
			return uvs.xy + finalTexOffset;
		}


		float3 mod3D289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 mod3D289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 permute( float4 x ) { return mod3D289( ( x * 34.0 + 1.0 ) * x ); }

		float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }

		float snoise( float3 v )
		{
			const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );
			float3 i = floor( v + dot( v, C.yyy ) );
			float3 x0 = v - i + dot( i, C.xxx );
			float3 g = step( x0.yzx, x0.xyz );
			float3 l = 1.0 - g;
			float3 i1 = min( g.xyz, l.zxy );
			float3 i2 = max( g.xyz, l.zxy );
			float3 x1 = x0 - i1 + C.xxx;
			float3 x2 = x0 - i2 + C.yyy;
			float3 x3 = x0 - 0.5;
			i = mod3D289( i);
			float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );
			float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
			float4 x_ = floor( j / 7.0 );
			float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
			float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 h = 1.0 - abs( x ) - abs( y );
			float4 b0 = float4( x.xy, y.xy );
			float4 b1 = float4( x.zw, y.zw );
			float4 s0 = floor( b0 ) * 2.0 + 1.0;
			float4 s1 = floor( b1 ) * 2.0 + 1.0;
			float4 sh = -step( h, 0.0 );
			float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
			float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
			float3 g0 = float3( a0.xy, h.x );
			float3 g1 = float3( a0.zw, h.y );
			float3 g2 = float3( a1.xy, h.z );
			float3 g3 = float3( a1.zw, h.w );
			float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
			g0 *= norm.x;
			g1 *= norm.y;
			g2 *= norm.z;
			g3 *= norm.w;
			float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
			m = m* m;
			m = m* m;
			float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
			return 42.0 * dot( m, px);
		}


		float2 voronoihash71( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi71( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
		{
			float2 n = floor( v );
			float2 f = frac( v );
			float F1 = 8.0;
			float F2 = 8.0; float2 mg = 0;
			for ( int j = -1; j <= 1; j++ )
			{
				for ( int i = -1; i <= 1; i++ )
			 	{
			 		float2 g = float2( i, j );
			 		float2 o = voronoihash71( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
					float d = 0.5 * dot( r, r );
			 		if( d<F1 ) {
			 			F2 = F1;
			 			F1 = d; mg = g; mr = r; id = o;
			 		} else if( d<F2 ) {
			 			F2 = d;
			
			 		}
			 	}
			}
			return F1;
		}


		float2 voronoihash108( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi108( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
		{
			float2 n = floor( v );
			float2 f = frac( v );
			float F1 = 8.0;
			float F2 = 8.0; float2 mg = 0;
			for ( int j = -1; j <= 1; j++ )
			{
				for ( int i = -1; i <= 1; i++ )
			 	{
			 		float2 g = float2( i, j );
			 		float2 o = voronoihash108( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
					float d = 0.5 * dot( r, r );
			 		if( d<F1 ) {
			 			F2 = F1;
			 			F1 = d; mg = g; mr = r; id = o;
			 		} else if( d<F2 ) {
			 			F2 = d;
			
			 		}
			 	}
			}
			return F1;
		}


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Normal = float3(0,0,1);
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = Unity_SafeNormalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 worldToObjDir334 = ASESafeNormalize( mul( unity_WorldToObject, float4( ase_worldViewDir, 0 ) ).xyz );
			float cos335 = cos( ( ( _RotateAngle / _Float6 ) * UNITY_PI ) );
			float sin335 = sin( ( ( _RotateAngle / _Float6 ) * UNITY_PI ) );
			float2 rotator335 = mul( worldToObjDir334.xy - float2( 0,0 ) , float2x2( cos335 , -sin335 , sin335 , cos335 )) + float2( 0,0 );
			float Viewdir339 = abs( (frac( rotator335.x )*2.0 + -1.0) );
			float U20 = i.uv_texcoord.x;
			float FlipU26 = ( 1.0 - i.uv_texcoord.x );
			float V21 = i.uv_texcoord.y;
			float FlipV27 = ( 1.0 - i.uv_texcoord.y );
			float clampResult54 = clamp( ( ( step( U20 , _Right ) * step( FlipU26 , _Left ) ) * ( step( V21 , _UP ) * step( FlipV27 , _Down ) ) ) , 0.0 , 1.0 );
			float Mask55 = clampResult54;
			float2 appendResult323 = (float2(( ( pow( Viewdir339 , 1.0 ) * Mask55 ) + (0) ) , _Float4));
			float4 ramp337 = tex2D( _ramp, appendResult323 );
			float mulTime312 = _Time.y * _Mid_noise_speed;
			float time310 = mulTime312;
			float2 voronoiSmoothId310 = 0;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float2 OffsetPOM220 = POM( _Depth_texture, i.uv_texcoord, ddx(i.uv_texcoord), ddy(i.uv_texcoord), ase_worldNormal, ase_worldViewDir, i.viewDir, 8, 8, _Float2, 0, _Depth_texture_ST.xy, float2(0,0), 0 );
			float2 Mid_depth226 = OffsetPOM220;
			float2 panner233 = ( 1.0 * _Time.y * _Mid_Noiseflowspeed_3 + Mid_depth226);
			float2 coords310 = panner233 * _Mid_noise_scale;
			float2 id310 = 0;
			float2 uv310 = 0;
			float voroi310 = voronoi310( coords310, time310, id310, uv310, 0, voronoiSmoothId310 );
			float2 uv_TexCoord267 = i.uv_texcoord * _Mid_NoiseUVTilling + _Mid_NoiseUVOffset;
			float2 panner305 = ( 1.0 * _Time.y * _Mid_Noiseflowspeed_2 + uv_TexCoord267);
			float2 panner274 = ( 1.0 * _Time.y * _Mid_Noiseflowspeed_1 + uv_TexCoord267);
			float clampResult299 = clamp( ( ( V21 + _Mid_NoiseMask_2.x ) * _Mid_NoiseMask_2.y ) , 0.0 , 1.0 );
			float2 panner294 = ( 1.0 * _Time.y * _Mid_texture_Noiseflowspped1 + uv_TexCoord267);
			float4 Mid_texture257 = ( ( voroi310 * 1.0 ) * tex2D( _Mid_texture, ( float4( panner305, 0.0 , 0.0 ) + ( ( ( ( tex2D( _Mid_texture_smoke, panner274 ) + _Mid_NoiseMask_1.x ) * _Mid_NoiseMask_1.y ) * _Mid_noisepower_1 ) * _Mid_texture_NoisePower * clampResult299 ) ).rg ) * ( ( tex2D( _Mid_texture_smoke_2, panner294 ).r + 0.5 ) - clampResult299 ) * _mid_texture_light * ( 1.0 - clampResult299 ) );
			float2 OffsetPOM3 = POM( _Depth_texture, i.uv_texcoord, ddx(i.uv_texcoord), ddy(i.uv_texcoord), ase_worldNormal, ase_worldViewDir, i.viewDir, 8, 8, _Float1, 0, _Depth_texture_ST.xy, float2(0,0), 0 );
			float2 Depth_Mask209 = OffsetPOM3;
			float2 panner119 = ( 1.0 * _Time.y * _Flowmap_speed + Depth_Mask209);
			float simplePerlin3D144 = snoise( float3( panner119 ,  0.0 )*_FlowMap_scale );
			simplePerlin3D144 = simplePerlin3D144*0.5 + 0.5;
			float clampResult170 = clamp( ( ( V21 + _Flowmap_Y.x ) * _Flowmap_Y.y ) , 0.0 , 1.0 );
			float clampResult171 = clamp( ( ( FlipU26 + _Flowmap_X.x ) * _Flowmap_X.y ) , 0.0 , 1.0 );
			float clampResult136 = clamp( ( clampResult170 * clampResult171 * _FLowmap_mask_power ) , 0.0 , 1.0 );
			float smoothstepResult158 = smoothstep( _Flowmap_smoothstep.x , _Flowmap_smoothstep.y , ( simplePerlin3D144 * clampResult136 ));
			float2 temp_cast_4 = (smoothstepResult158).xx;
			float2 lerpResult123 = lerp( Depth_Mask209 , temp_cast_4 , _FLowmap_lerp);
			float FlowMap_mask149 = tex2D( _Flowmap_texture, lerpResult123 ).r;
			float4 Albedo12 = ( Mask55 * ( tex2D( _Main_texture, ( Depth_Mask209 + FlowMap_mask149 ) ) * _Light * _Light ) );
			o.Albedo = ( ( ramp337 * 0.5 ) + ( Mid_texture257 + Albedo12 ) ).rgb;
			float mulTime86 = _Time.y * _Particle_Alpha_speed;
			float time71 = mulTime86;
			float2 voronoiSmoothId71 = 0;
			float2 OffsetPOM268 = POM( _Depth_texture, float2( 0,0 ), ddx(float2( 0,0 )), ddy(float2( 0,0 )), ase_worldNormal, ase_worldViewDir, i.viewDir, 8, 8, _Float3, 0, _Depth_texture_ST.xy, float2(0,0), 0 );
			float2 Front_depth270 = OffsetPOM268;
			float2 coords71 = Front_depth270 * _Particle_Voronoi_scale;
			float2 id71 = 0;
			float2 uv71 = 0;
			float voroi71 = voronoi71( coords71, time71, id71, uv71, 0, voronoiSmoothId71 );
			float smoothstepResult76 = smoothstep( _Particle_Alpha_Power.x , _Particle_Alpha_Power.y , voroi71);
			float2 uv_TexCoord87 = i.uv_texcoord * _particle_tiling + _particle_offset;
			float2 panner65 = ( 1.0 * _Time.y * _Particle_speed + uv_TexCoord87);
			float mulTime109 = _Time.y * _Particle_noise_speed;
			float time108 = mulTime109;
			float2 voronoiSmoothId108 = 0;
			float2 coords108 = Front_depth270 * _Particle_noise_scale;
			float2 id108 = 0;
			float2 uv108 = 0;
			float voroi108 = voronoi108( coords108, time108, id108, uv108, 0, voronoiSmoothId108 );
			float simplePerlin2D115 = snoise( panner65 );
			simplePerlin2D115 = simplePerlin2D115*0.5 + 0.5;
			float4 lerpResult113 = lerp( _Color0 , _Color1 , simplePerlin2D115);
			float4 Particle_emission83 = ( ( smoothstepResult76 * tex2D( _Particle_Texture, ( panner65 + ( uv108 * _Particle_noise_power ) ) ) ) * lerpResult113 );
			o.Emission = Particle_emission83.rgb;
			float temp_output_58_0 = Mask55;
			o.Alpha = temp_output_58_0;
			clip( temp_output_58_0 - _Cutoff );
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
				surfIN.viewDir = IN.tSpace0.xyz * worldViewDir.x + IN.tSpace1.xyz * worldViewDir.y + IN.tSpace2.xyz * worldViewDir.z;
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
Node;AmplifyShaderEditor.CommentaryNode;340;-5348.035,635.5756;Inherit;False;2921.141;1536.082;noise;39;288;228;287;289;280;294;302;301;303;297;296;299;278;272;255;233;305;263;277;262;276;300;282;295;298;290;275;266;274;306;267;257;312;310;314;313;311;315;316;noise;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;166;-4725.892,-1363.259;Inherit;False;2776.313;902.6914;FlowMap;23;116;123;158;124;134;159;144;136;135;148;146;154;119;130;133;147;143;149;168;169;170;171;172;FlowMap;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;151;-1844.832,-1363.88;Inherit;False;1363.851;489.6937;Albedo;9;12;59;10;56;11;9;207;212;213;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;105;-4695.19,-417.8574;Inherit;False;2051.453;953.9537;particle;28;64;71;76;80;86;78;68;87;74;97;92;93;65;70;69;75;83;96;106;107;108;109;110;111;112;113;115;271;Particle;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;62;-3704.745,-2392.106;Inherit;False;1711.893;985.5854;Main_Mask;20;19;53;45;52;43;20;26;24;21;27;25;44;46;48;47;34;54;50;49;55;Main_Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;60;-4687.073,-2136.753;Inherit;False;821.6996;656;Depth;12;3;2;1;5;6;209;220;225;226;268;269;270;Depth;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;19;-3654.746,-2121.585;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;53;-3201.182,-1640.24;Inherit;False;Property;_Down;Down;16;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;45;-3052.406,-2115.833;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0.8;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;43;-3054.531,-2342.106;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0.8;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;20;-3407.521,-2330.002;Inherit;True;U;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;24;-3394.156,-2079.994;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;21;-3407.724,-1972.095;Inherit;True;V;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;25;-3396.428,-1740.32;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;44;-3051.205,-1896.635;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0.81;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;46;-3054.104,-1660.522;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0.8;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-2825.645,-2336.791;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-2824.229,-1895.72;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-2584.703,-2337.814;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;54;-2364.647,-2331.247;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;55;-2216.853,-2322.594;Inherit;False;Mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;58;-296.4615,223.5093;Inherit;False;55;Mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;85;-306.5433,92.39496;Inherit;False;83;Particle_emission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;64;-4611.72,-351.7794;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SmoothstepOpNode;76;-3650.162,-367.8574;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;80;-3375.138,-364.197;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector2Node;68;-4201.513,65.88562;Inherit;False;Property;_Particle_speed;Particle_speed;18;0;Create;True;0;0;0;False;0;False;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;87;-4225.812,-72.43608;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;15,15;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;93;-4386.23,70.7478;Inherit;False;Property;_particle_offset;particle_offset;22;0;Create;True;0;0;0;False;0;False;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;70;-3108.79,-352.2964;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;83;-2881.735,-344.4754;Inherit;True;Particle_emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector2Node;78;-3877.341,-301.9307;Inherit;False;Property;_Particle_Alpha_Power;Particle_Alpha_Power;20;0;Create;True;0;0;0;False;0;False;0.1,0.6;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;65;-4029.457,-62.5401;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.VoronoiNode;71;-4083.926,-383.434;Inherit;True;0;0;1;0;1;False;1;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.Vector2Node;92;-4386.126,-72.11089;Inherit;False;Property;_particle_tiling;particle_tiling;23;0;Create;True;0;0;0;False;0;False;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;107;-3996.538,245.6998;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;86;-4294.24,-318.2714;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;97;-4629.836,-223.2405;Inherit;False;Property;_Particle_Alpha_speed;Particle_Alpha_speed;27;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;108;-4224.937,241.0996;Inherit;True;0;0;1;0;1;False;1;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.RangedFloatNode;111;-4083.934,412.7326;Inherit;False;Property;_Particle_noise_power;Particle_noise_power;26;0;Create;True;0;0;0;False;0;False;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;115;-3458.696,284.744;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;49;-3205.743,-2285.067;Inherit;False;Property;_Right;Right;14;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;50;-3186.875,-2118.654;Inherit;False;Property;_Left;Left;13;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-3181.647,-1874.923;Inherit;False;Property;_UP;UP;15;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;26;-3244.415,-2046.767;Inherit;True;FlipU;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;27;-3234.021,-1739.868;Inherit;True;FlipV;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;134;-3242.956,-1232.484;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;124;-2943.862,-1008.061;Inherit;False;Property;_FLowmap_lerp;FLowmap_lerp;31;0;Create;True;0;0;0;False;0;False;0.2;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;158;-2997.189,-1230.65;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;170;-3895.576,-890.9282;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;171;-3895.576,-680.9282;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;135;-4099.145,-682.0316;Inherit;True;ConstantBiasScale;-1;;3;63208df05c83e8e49a48ffbdce2e43a0;0;3;3;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;147;-4429.042,-729.183;Inherit;False;26;FlipU;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;148;-4279.312,-613.567;Inherit;False;Property;_Flowmap_X;Flowmap_X;33;0;Create;True;0;0;0;False;0;False;-0.5,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;169;-4286.577,-825.9282;Inherit;False;Property;_Flowmap_Y;Flowmap_Y;34;0;Create;True;0;0;0;False;0;False;-0.5,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;154;-3772.91,-1027.105;Inherit;False;Property;_FlowMap_scale;FlowMap_scale;35;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;130;-4012.969,-1040.172;Inherit;False;Property;_Flowmap_speed;Flowmap_speed;32;0;Create;True;0;0;0;False;0;False;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;146;-3643.485,-866.1317;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;136;-3442.624,-879.0323;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;144;-3602.453,-1233.685;Inherit;True;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;159;-3218.204,-998.4278;Inherit;False;Property;_Flowmap_smoothstep;Flowmap_smoothstep;36;0;Create;True;0;0;0;False;0;False;0.1,0.4;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;119;-3819.586,-1232.559;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.2,-0.2;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;123;-2735.114,-1273.327;Inherit;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;116;-2614.233,-892.3891;Inherit;True;Property;_Flowmap_texture;Flowmap_texture;9;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;149;-2162.717,-1093.487;Inherit;False;FlowMap_mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-1374.962,-1011.129;Inherit;False;Property;_Light;Light;11;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;143;-4590.939,-1284.094;Inherit;False;209;Depth_Mask;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;213;-1801.387,-1290.36;Inherit;False;209;Depth_Mask;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;212;-1813.281,-1199.668;Inherit;False;149;FlowMap_mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;74;-4254.772,-210.9793;Inherit;False;Property;_Particle_Voronoi_scale;Particle_Voronoi_scale;19;0;Create;True;0;0;0;False;0;False;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;172;-3722.958,-612.8502;Inherit;False;Property;_FLowmap_mask_power;FLowmap_mask_power;37;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;12;-667.1642,-1235.569;Inherit;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;207;-1590.648,-1264.642;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;9;-1480.517,-1218.188;Inherit;True;Property;_Main_texture;Main_texture;7;0;Create;True;0;0;0;False;0;False;-1;501f77cba7fd8c14ab61f285182909cd;501f77cba7fd8c14ab61f285182909cd;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;56;-1189.844,-1274.177;Inherit;False;55;Mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-1198.887,-1158.44;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;59;-996.5936,-1251.757;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-4672.448,-2079.124;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ParallaxOcclusionMappingNode;3;-4278.807,-2064.253;Inherit;False;0;8;False;;16;False;;2;0.02;0;False;1,1;False;0,0;8;0;FLOAT2;0,0;False;1;SAMPLER2D;;False;7;SAMPLERSTATE;;False;2;FLOAT;0.02;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT2;0,0;False;6;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;209;-4058.602,-2062.502;Inherit;False;Depth_Mask;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-4446.742,-1982.143;Inherit;False;Property;_Float1;Depth;4;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ParallaxOcclusionMappingNode;220;-4274.37,-1878.099;Inherit;False;0;8;False;;16;False;;2;0.02;0;False;1,1;False;0,0;8;0;FLOAT2;0,0;False;1;SAMPLER2D;;False;7;SAMPLERSTATE;;False;2;FLOAT;0.02;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT2;0,0;False;6;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ParallaxOcclusionMappingNode;268;-4275.602,-1684.632;Inherit;False;0;8;False;;16;False;;2;0.02;0;False;1,1;False;0,0;8;0;FLOAT2;0,0;False;1;SAMPLER2D;;False;7;SAMPLERSTATE;;False;2;FLOAT;0.02;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT2;0,0;False;6;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;269;-4445.477,-1619.382;Inherit;False;Property;_Float3;Front_Depth;2;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;270;-4055.102,-1686.882;Inherit;False;Front_depth;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;271;-4612.084,-101.0267;Inherit;False;270;Front_depth;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;226;-4048.671,-1878.701;Inherit;False;Mid_depth;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;1;-4680.959,-1958.481;Inherit;True;Property;_Depth_texture;Depth_texture;1;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;6;-4631.712,-1654.609;Inherit;False;Tangent;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;225;-4472.154,-1805.118;Inherit;False;Property;_Float2;Mid_Depth;3;0;Create;False;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;106;-3863.191,-55.31849;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;168;-4116.577,-891.9282;Inherit;True;ConstantBiasScale;-1;;4;63208df05c83e8e49a48ffbdce2e43a0;0;3;3;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;133;-4448.426,-887.2234;Inherit;False;21;V;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;69;-3753.781,-89.30475;Inherit;True;Property;_Particle_Texture;Particle_Texture;17;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;113;-3158.743,-21.85129;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;112;-3438.104,101.0266;Inherit;False;Property;_Color1;Color 1;30;1;[HDR];Create;True;0;0;0;False;0;False;1,0.4752927,0,0;1,0.4752927,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;75;-3426.206,-85.54465;Inherit;False;Property;_Color0;Color 0;21;1;[HDR];Create;True;0;0;0;False;0;False;0,1,0.7890246,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;63;-463.213,-114.3655;Inherit;False;12;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;279;-180.5038,-169.5506;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;265;-464.3086,-201.582;Inherit;False;257;Mid_texture;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;110;-4653.138,259.4138;Inherit;False;Property;_Particle_noise_speed;Particle_noise_speed;28;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;109;-4444.943,280.0839;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;96;-4534.065,395.5966;Inherit;False;Property;_Particle_noise_scale;Particle_noise_scale;25;0;Create;True;0;0;0;False;0;False;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;353.1616,-44.83591;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Background;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;True;Opaque;;AlphaTest;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.Vector2Node;288;-5290.062,1358.691;Inherit;False;Property;_Mid_NoiseUVOffset;Mid_NoiseUVOffset;50;0;Create;True;0;0;0;False;0;False;0,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;228;-5207.711,1024.604;Inherit;False;226;Mid_depth;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;287;-5298.035,1211.659;Inherit;False;Property;_Mid_NoiseUVTilling;Mid_NoiseUVTilling;46;0;Create;True;0;0;0;False;0;False;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;289;-3877.48,1181.882;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;280;-4121.79,1171.994;Inherit;True;ConstantBiasScale;-1;;5;63208df05c83e8e49a48ffbdce2e43a0;0;3;3;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.PannerNode;294;-4622.104,1630.08;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;302;-3929.99,1620.436;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;301;-4066.229,1790.851;Inherit;False;Constant;_Float5;Float 2;4;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;303;-3620.085,1662.947;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;297;-4138.403,1900.099;Inherit;False;ConstantBiasScale;-1;;6;63208df05c83e8e49a48ffbdce2e43a0;0;3;3;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;296;-4408.059,1802.696;Inherit;True;21;V;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;299;-3869.587,1856.063;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;278;-3919.132,1503.495;Inherit;False;Property;_Mid_texture_NoisePower;Mid_texture_NoisePower;45;0;Create;True;0;0;0;False;0;False;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;272;-3579.132,1093.728;Inherit;False;2;2;0;FLOAT2;0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;255;-3468.167,1067.32;Inherit;True;Property;_Mid_texture;Mid_texture;38;0;Create;True;0;0;0;False;0;False;-1;70c887586beeaaf4683cc15cc2715b79;5f9111b4760c49443b9048a7ef8fbae6;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;233;-4609.655,882.5109;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;305;-4349.934,1021.118;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;263;-3154.244,1362.105;Inherit;False;Property;_mid_texture_light;mid_texture_light;39;0;Create;True;0;0;0;False;0;False;4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;277;-3665.738,1221.022;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;262;-3011.793,1022.992;Inherit;True;5;5;0;FLOAT;1;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;276;-4444.508,1163.604;Inherit;True;Property;_Mid_texture_smoke;Mid_texture_smoke;40;0;Create;True;0;0;0;False;0;False;-1;a7cae8bdd4eee65488de6084e2263d67;a7cae8bdd4eee65488de6084e2263d67;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;300;-4385.103,1591.749;Inherit;True;Property;_Mid_texture_smoke_2;Mid_texture_smoke_2;5;0;Create;True;0;0;0;False;0;False;-1;70c887586beeaaf4683cc15cc2715b79;70c887586beeaaf4683cc15cc2715b79;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;282;-4350.213,1422.85;Inherit;False;Property;_Mid_NoiseMask_1;Mid_NoiseMask_1;48;0;Create;True;0;0;0;False;0;False;-0.33,2.53;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;295;-4930.496,1676.751;Inherit;False;Property;_Mid_texture_Noiseflowspped1;Mid_texture_Noiseflowspped;42;0;Create;True;0;0;0;False;0;False;1,-1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;298;-4405.449,2007.658;Inherit;False;Property;_Mid_NoiseMask_2;Mid_NoiseMask_2;49;0;Create;True;0;0;0;False;0;False;-0.17,2.53;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;290;-4102.527,1511.871;Inherit;False;Property;_Mid_noisepower_1;Mid_noisepower_1;47;0;Create;True;0;0;0;False;0;False;0.17;0.17;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;275;-4943.319,1392.004;Inherit;False;Property;_Mid_Noiseflowspeed_1;Mid_Noiseflowspeed_1;43;0;Create;True;0;0;0;False;0;False;-0.2,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;266;-4907.443,897.858;Inherit;False;Property;_Mid_Noiseflowspeed_3;Mid_Noiseflowspeed_3;41;0;Create;True;0;0;0;False;0;False;-0.5,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;274;-4629.783,1226.477;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;306;-4723.905,1039.462;Inherit;False;Property;_Mid_Noiseflowspeed_2;Mid_Noiseflowspeed_2;44;0;Create;True;0;0;0;False;0;False;-0.5,-0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;267;-5061.132,1232.738;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;257;-2650.894,1021.483;Inherit;False;Mid_texture;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleTimeNode;312;-4289.629,718.8234;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;310;-4066.277,685.5756;Inherit;True;0;0;1;0;1;False;1;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;314;-3826.387,731.5535;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;313;-4479.264,805.9302;Inherit;False;Property;_Mid_noise_scale;Mid_noise_scale;24;0;Create;True;0;0;0;False;0;False;15;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;311;-4497.825,698.1531;Inherit;False;Property;_Mid_noise_speed;Mid_noise_speed;29;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;315;-4006.959,973.5032;Inherit;False;Constant;_Float0;Float 0;47;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;316;-3362.233,1442.26;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;319;-1135.734,1663.123;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;320;-952.8409,1699.057;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;321;-1299.268,1653.196;Inherit;False;339;Viewdir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;322;-1148.204,1796.738;Inherit;False;55;Mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;323;-431.5572,1758.275;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;324;-995.7399,1866.417;Inherit;False;-1;;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;325;-732.1768,1696.187;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;327;-645.5831,1918.898;Inherit;False;Property;_Float4;Float 4;12;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;317;-610.9172,1008.931;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;318;-449.2042,1013.652;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;2;False;2;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;328;-1533.754,1254.375;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;329;-1405.092,1311.034;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PiNode;330;-1624.644,1456.222;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;333;-1622.349,990.9207;Inherit;False;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TransformDirectionNode;334;-1371.047,1028.827;Inherit;False;World;Object;True;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RotatorNode;335;-1077.763,1038.362;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;336;-704.4442,1028.871;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.AbsOpNode;338;-229.2655,1007.724;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;339;-100.2465,1002.511;Inherit;False;Viewdir;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;341;-326.6183,-549.3243;Inherit;False;337;ramp;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;342;137.3818,-437.3242;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;343;-102.6183,-517.3243;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;344;-294.6183,-437.3242;Inherit;False;Constant;_Float8;Float 5;11;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;337;142.6142,1693.613;Inherit;False;ramp;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;326;-266.6486,1678.168;Inherit;True;Property;_ramp;ramp;10;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;331;-1800.282,1219.92;Inherit;False;Property;_RotateAngle;RotateAngle;6;0;Create;True;0;0;0;False;0;False;360;360;-1;360;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;332;-1705.55,1312.034;Inherit;False;Property;_Float6;Float 6;8;0;Create;True;0;0;0;False;0;False;180;45;0;0;0;1;FLOAT;0
WireConnection;45;0;26;0
WireConnection;45;1;50;0
WireConnection;43;0;20;0
WireConnection;43;1;49;0
WireConnection;20;0;19;1
WireConnection;24;0;19;1
WireConnection;21;0;19;2
WireConnection;25;0;19;2
WireConnection;44;0;21;0
WireConnection;44;1;52;0
WireConnection;46;0;27;0
WireConnection;46;1;53;0
WireConnection;48;0;43;0
WireConnection;48;1;45;0
WireConnection;47;0;44;0
WireConnection;47;1;46;0
WireConnection;34;0;48;0
WireConnection;34;1;47;0
WireConnection;54;0;34;0
WireConnection;55;0;54;0
WireConnection;76;0;71;0
WireConnection;76;1;78;1
WireConnection;76;2;78;2
WireConnection;80;0;76;0
WireConnection;80;1;69;0
WireConnection;87;0;92;0
WireConnection;87;1;93;0
WireConnection;70;0;80;0
WireConnection;70;1;113;0
WireConnection;83;0;70;0
WireConnection;65;0;87;0
WireConnection;65;2;68;0
WireConnection;71;0;271;0
WireConnection;71;1;86;0
WireConnection;71;2;74;0
WireConnection;107;0;108;2
WireConnection;107;1;111;0
WireConnection;86;0;97;0
WireConnection;108;0;271;0
WireConnection;108;1;109;0
WireConnection;108;2;96;0
WireConnection;115;0;65;0
WireConnection;26;0;24;0
WireConnection;27;0;25;0
WireConnection;134;0;144;0
WireConnection;134;1;136;0
WireConnection;158;0;134;0
WireConnection;158;1;159;1
WireConnection;158;2;159;2
WireConnection;170;0;168;0
WireConnection;171;0;135;0
WireConnection;135;3;147;0
WireConnection;135;1;148;1
WireConnection;135;2;148;2
WireConnection;146;0;170;0
WireConnection;146;1;171;0
WireConnection;146;2;172;0
WireConnection;136;0;146;0
WireConnection;144;0;119;0
WireConnection;144;1;154;0
WireConnection;119;0;143;0
WireConnection;119;2;130;0
WireConnection;123;0;143;0
WireConnection;123;1;158;0
WireConnection;123;2;124;0
WireConnection;116;1;123;0
WireConnection;149;0;116;1
WireConnection;12;0;59;0
WireConnection;207;0;213;0
WireConnection;207;1;212;0
WireConnection;9;1;207;0
WireConnection;10;0;9;0
WireConnection;10;1;11;0
WireConnection;10;2;11;0
WireConnection;59;0;56;0
WireConnection;59;1;10;0
WireConnection;3;0;2;0
WireConnection;3;1;1;0
WireConnection;3;2;5;0
WireConnection;3;3;6;0
WireConnection;209;0;3;0
WireConnection;220;0;2;0
WireConnection;220;1;1;0
WireConnection;220;2;225;0
WireConnection;220;3;6;0
WireConnection;268;1;1;0
WireConnection;268;2;269;0
WireConnection;268;3;6;0
WireConnection;270;0;268;0
WireConnection;226;0;220;0
WireConnection;106;0;65;0
WireConnection;106;1;107;0
WireConnection;168;3;133;0
WireConnection;168;1;169;1
WireConnection;168;2;169;2
WireConnection;69;1;106;0
WireConnection;113;0;75;0
WireConnection;113;1;112;0
WireConnection;113;2;115;0
WireConnection;279;0;265;0
WireConnection;279;1;63;0
WireConnection;109;0;110;0
WireConnection;0;0;342;0
WireConnection;0;2;85;0
WireConnection;0;9;58;0
WireConnection;0;10;58;0
WireConnection;289;0;280;0
WireConnection;289;1;290;0
WireConnection;280;3;276;0
WireConnection;280;1;282;1
WireConnection;280;2;282;2
WireConnection;294;0;267;0
WireConnection;294;2;295;0
WireConnection;302;0;300;1
WireConnection;302;1;301;0
WireConnection;303;0;302;0
WireConnection;303;1;299;0
WireConnection;297;3;296;0
WireConnection;297;1;298;1
WireConnection;297;2;298;2
WireConnection;299;0;297;0
WireConnection;272;0;305;0
WireConnection;272;1;277;0
WireConnection;255;1;272;0
WireConnection;233;0;228;0
WireConnection;233;2;266;0
WireConnection;305;0;267;0
WireConnection;305;2;306;0
WireConnection;277;0;289;0
WireConnection;277;1;278;0
WireConnection;277;2;299;0
WireConnection;262;0;314;0
WireConnection;262;1;255;0
WireConnection;262;2;303;0
WireConnection;262;3;263;0
WireConnection;262;4;316;0
WireConnection;276;1;274;0
WireConnection;300;1;294;0
WireConnection;274;0;267;0
WireConnection;274;2;275;0
WireConnection;267;0;287;0
WireConnection;267;1;288;0
WireConnection;257;0;262;0
WireConnection;312;0;311;0
WireConnection;310;0;233;0
WireConnection;310;1;312;0
WireConnection;310;2;313;0
WireConnection;314;0;310;0
WireConnection;314;1;315;0
WireConnection;316;0;299;0
WireConnection;319;0;321;0
WireConnection;320;0;319;0
WireConnection;320;1;322;0
WireConnection;323;0;325;0
WireConnection;323;1;327;0
WireConnection;325;0;320;0
WireConnection;325;1;324;0
WireConnection;317;0;336;0
WireConnection;318;0;317;0
WireConnection;328;0;331;0
WireConnection;328;1;332;0
WireConnection;329;0;328;0
WireConnection;329;1;330;0
WireConnection;334;0;333;0
WireConnection;335;0;334;0
WireConnection;335;2;329;0
WireConnection;336;0;335;0
WireConnection;338;0;318;0
WireConnection;339;0;338;0
WireConnection;342;0;343;0
WireConnection;342;1;279;0
WireConnection;343;0;341;0
WireConnection;343;1;344;0
WireConnection;337;0;326;0
WireConnection;326;1;323;0
ASEEND*/
//CHKSM=CDC4601BD1E4522E67319729DBACD47CC13E238B