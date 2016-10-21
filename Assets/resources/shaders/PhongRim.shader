// Modification of the phong shader given in labs, with assistance from www.unitycookie.com
// added a lighting system to colour the edges of highlights in a separate colour
// added bump and texture mapping

Shader "MyShaders/PhongRimBump"
{
	Properties
	{
		_PointLightColor("Point Light Color", Color) = (0, 0, 0)
		_PointLightPosition("Point Light Position", Vector) = (0.0, 0.0, 0.0)
		_RimColor("Rim Color" , Color) = (1.0,1.0,1.0)
		_RimPower("Rim Power" , Range(0.1,10.0)) = 3.0
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform float3 _PointLightColor;
			uniform float3 _PointLightPosition;
			uniform float3 _RimColor;
			uniform float _RimPower;
			

			uniform sampler2D _MainTex;
			uniform sampler2D _NormalMapTex;

			struct vertIn
			{
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				float4 tangent : TANGENT;
				float2 uv : TEXCOORD0;
				// float4 color : COLOR; not needed anymore as colour will be sampled from texture;
			};

			struct vertOut
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				// float4 color : COLOR; same as above
				float4 worldVertex : TEXCOORD1;
				float3 worldNormal : TEXCOORD2;
				float3 worldTangent : TEXCOORD3;
				float3 worldBinormal : TEXCOORD4;
			};

			// Implementation of the vertex shader
			vertOut vert(vertIn v)
			{
				vertOut o;

				// Convert Vertex position and corresponding normal into world coords
				// Note that we have to multiply the normal by the transposed inverse of the world 
				// transformation matrix (for cases where we have non-uniform scaling; we also don't
				// care about the "fourth" dimension, because translations don't affect the normal) 
				float4 worldVertex = mul(_Object2World, v.vertex);
				float3 worldNormal = normalize(mul(transpose((float3x3)_World2Object), v.normal.xyz));
				float3 worldTangent = normalize(mul(transpose((float3x3)_World2Object), v.tangent.xyz));
				float3 worldBinormal = normalize(cross(worldTangent, worldNormal));

				// Transform vertex in world coordinates to camera coordinates, and pass colour
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;

				// Pass out the world vertex position and world normal to be interpolated
				// in the fragment shader (and utilised)
				o.worldVertex = worldVertex;
				o.worldNormal = worldNormal;
				o.worldTangent = worldTangent;
				o.worldBinormal = worldBinormal;

				return o;
			}

			// Implementation of the fragment shader
			fixed4 frag(vertOut v) : SV_Target
			{
				// Our interpolated normal might not be of length 1
				//float3 interpNormal = normalize(v.worldNormal); replaced by normal map

				float4 surfaceColor = tex2D(_MainTex, v.uv); // colour from sampling point on texture

				float3 bump = (tex2D(_NormalMapTex, v.uv) - float3(0.5, 0.5, 0.5)) * 2.0; //brings normal into range of -1 to 1
				float3 bumpNormal = (bump.x * normalize(v.worldTangent)) +
									(bump.y * normalize(v.worldBinormal)) +
									(bump.z * normalize(v.worldNormal));
				bumpNormal = normalize(bumpNormal);
				// no need for ambient lighting

				// Calculate diffuse RBG reflections, we save the results of L.N because we will use it again
				// (when calculating the reflected ray in our specular component)
				float fAtt = 1/length(_PointLightPosition - v.worldVertex.xyz);
				float Kd = 1; // diffuse coeff
				float3 L = normalize(_PointLightPosition - v.worldVertex.xyz);
				float LdotN = dot(L, bumpNormal);
				float3 dif = fAtt * _PointLightColor.rgb * Kd * surfaceColor * saturate(LdotN);

				// Calculate specular reflections
				float Ks = 1; // specular coeef
				float specN = 50; // Values>>1 give tighter highlights
				float3 V = normalize(_WorldSpaceCameraPos - v.worldVertex.xyz);
				// Using classic reflection calculation:
				//float3 R = normalize((2.0 * LdotN * interpNormal) - L);
				//float3 spe = fAtt * _PointLightColor.rgb * Ks * pow(saturate(dot(V, R)), specN);
				// Using Blinn-Phong approximation:
				float3 H = normalize(V + L);
				float3 spe = fAtt * _PointLightColor.rgb * Ks * surfaceColor * pow(saturate(dot(bumpNormal, H)), specN);

				// rim Lighting
				float Kr = 1;
				float rim = 1 - saturate(dot(bumpNormal, normalize(V)));
				float3 rimLighting = fAtt * _PointLightColor.rgb * Kr * _RimColor * saturate(dot(bumpNormal, L)) * pow(rim, 1/_RimPower);

				// Combine Phong illumination model components
				float4 returnColor = float4(0.0f, 0.0f, 0.0f, 0.0f);
				returnColor.rgb = dif.rgb + spe.rgb + rimLighting.rgb;
				returnColor.a = surfaceColor.a;

				return returnColor;
			}
			ENDCG
		}
	}
}
