 Shader "T1/ModelEffect/testUE4" 
 {
	 Properties 
	 {
		 _MainTex ("模型贴图", 2D) = "white" {}
		 _MaskTex ("mask图", 2D) = "white" {}
		 texEnvMapA ("cubeA", Cube) = "" {}
		 texEnvMapB ("cubeB", Cube) = "" {}
		 litParam0 ("_litParam0", Vector) = (-0.6551209, 0.2035586, 0.7275854, 0)
		 litParam1 ("_litParam1", Vector) = (1,1,1,1)
		 litParam2 ("_litParam2", Vector) = (0.5,0.5,0.5,0.5)
		 edgeColorPm ("edgeColorPm", Vector) = (0.2,1,1,16)
		 LightScale ("LightScale", float) = 1.75
		 IllumScale ("IllumScale", float) = 2.0
		 blendColor ("blendColor", Color) = (1, 1, 1, 1)
		 addColor ("addColor", Color) = (0, 0, 0, 0)
	}
	 
	 SubShader 
	 {
		Tags { "Queue" = "Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		Blend SrcAlpha OneMinusSrcAlpha 
		Fog {Mode Off}
		
		Pass
		{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest

				#include "UnityCG.cginc"
				sampler2D _MainTex;
				sampler2D _MaskTex;
				samplerCUBE texEnvMapA;
				samplerCUBE texEnvMapB;
				fixed4 litParam0;
				fixed4 litParam1;
				fixed4 litParam2;
				fixed4 edgeColorPm;
				fixed LightScale;
				fixed IllumScale;
				fixed4 blendColor;
				fixed4 addColor;


				struct vertexInput {
					float4 vertex : POSITION;
					half3 normal : NORMAL;
					half2 texcoord : TEXCOORD0;
				 };
				 struct v2f {
					float4 pos : SV_POSITION;
					float4 posWorld : TEXCOORD0;
					half3 normalDir : TEXCOORD1;
					half2 uv : TEXCOORD2;
				 };

				half3 calcCubeTexcoord(half3 PixelDir)
				{
					half3 absDir = abs(PixelDir);
					float maxabs = max(absDir.x, max(absDir.y, absDir.z));
					half3 ss = step(maxabs, absDir);
					ss = 0.975 + ss * 0.025;
					return PixelDir * ss;
				}

				half3 _GetEnvironmentCMap(samplerCUBE envMapA, samplerCUBE envMapB, half3 envTC, float fGloss)
				{
					envTC = calcCubeTexcoord(envTC);
					half3 envColorA = texCUBE(envMapA , envTC).xyz;
					half3 envColorB = texCUBE(envMapB , envTC).xyz;

					// artist defines
					fGloss = fGloss * 2.0;

					return lerp( envColorB,envColorA, fGloss);
					//return mix( envColorB,envColorA, fGloss);
				}

				half3 GetSpecular(float NdotH, float Gloss, half3 SpecCol)
				{
					Gloss = max(Gloss, 0.004);
					half3 specularLightps = SpecCol * pow(NdotH, pow(2.0, Gloss*10.0 ));
					return specularLightps;
				}


				// ue4 on mobile
				half3 EnvBRDFApprox( float NoV,float Gloss,half3 SpecularColor )
				{
					float Roughness = 1.004 - Gloss;

					half4 c0 = half4(-1.0,-0.0275,-0.572,0.022);
					half4 c1 = half4( 1.0, 0.0425, 1.04, -0.04 );
					half4 r = Roughness * c0 + c1;

					float a004 = min(r.x*r.x, exp2(-9.28*NoV)) * r.x + r.y;
					half2 AB = half2( -1.04, 1.04 ) * a004 + r.zw;
					return SpecularColor * AB.xxx + AB.yyy;
				}


				float EnvBRDFApproxNonmetal( float NoV, float Gloss )
				{
					float Roughness = 1.004 - Gloss;
					// Same as EnvBRDFApprox( 0.04, Roughness, NoV )

					half2 c0 = half2(-1.0,-0.0275);

					half2 c1 = half2(1.0, 0.0425);

					half2 r = Roughness * c0 + c1;

					return min( r.x * r.x, exp2( -9.28 * NoV ) ) * r.x + r.y;
				}

				half3 D_Approx( float RoL, float Gloss, half3 SpecCol )
				{
					// artist defines
					Gloss *= 0.75;

					float Roughness =  0.975 - Gloss; // clamp( 1.0 - Gloss, 0.05, 0.99);

					float a = Roughness * Roughness;

					float a2 = a * a;

					float rcp_a2 = 1.0 / (a2);

					// 0.5 / ln(2), 0.275 / ln(2)

					float c = 0.72134752 * rcp_a2 + 0.39674113;

					return rcp_a2 * exp2( c * RoL - c ) * SpecCol;
				}

				v2f vert(vertexInput input) 
				{
					v2f output;
 
					output.posWorld = mul(_Object2World, input.vertex);
					output.normalDir = mul(half4(input.normal, 0), _World2Object).xyz;
					output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
					output.uv = input.texcoord; 
					return output;
				}
			
				fixed4 frag(v2f input) : SV_Target 
				{
					fixed4 textureColor = tex2D(_MainTex, input.uv);
					fixed4 maskColor = tex2D(_MaskTex, input.uv);

					fixed3 Normal = normalize(input.normalDir);
					fixed3 viewDir = normalize(_WorldSpaceCameraPos - input.posWorld.xyz);

					half3 NrefV = normalize(reflect(-viewDir, Normal));
					half _gloss = maskColor.g;

					// diffuse
					half3 diffuse_color = texCUBE(texEnvMapB, calcCubeTexcoord(Normal)).xyz;
					//half3 diffuse = EnvBRDFApprox(dot(-viewDir,Normal), _gloss, diffuse_color );
					//diffuse.r += ( litParam1.x + litParam2.x)* 0.0001;
					half3 diffuse = ( litParam2.xyz * diffuse_color );

					//rim
					half cosView = clamp(dot(viewDir, Normal),0.0,1.0);
					half shineEdge = pow(1.0 - cosView*cosView, edgeColorPm.a);
					half rimColor = min(shineEdge, 1.0);

					// reflective
					half3 reflect_Color = _GetEnvironmentCMap(texEnvMapA, texEnvMapB, NrefV, _gloss);

					// brdf specular 
					half RL = clamp(dot(litParam0.xyz, NrefV),0.0,1.0);
					half3 specularIntensity = D_Approx(RL, _gloss,  maskColor.rrr*litParam1.rgb);
					half3 specular = specularIntensity * reflect_Color;

					// final color
					half3 color = (specular + diffuse * textureColor.rgb ) * LightScale + diffuse_color * rimColor * 0.5 * litParam1.a;

					// luminance
					//color = mix( color, textureColor.rgb * IllumScale, maskColor.bbb);
					color = lerp( color, textureColor.rgb * IllumScale, maskColor.bbb);

					// blend user color
					fixed4 fragColor = fixed4(color.rgb*blendColor.rgb + addColor, blendColor.a);
					return fragColor;
				}
			ENDCG
		}
	}
}