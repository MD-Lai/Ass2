// Original Cg/HLSL code stub copyright (c) 2010-2012 SharpDX - Alexandre Mutel
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// Adapted for COMP30019 by Jeremy Nicholson, 10 Sep 2012
// Adapted further by Chris Ewin, 23 Sep 2013
// Adapted further (again) by Alex Zable (port to Unity), 19 Aug 2016

Shader "MyShaders/PhongShader"
{
	Properties
	{
		_PointLightColor("Point Light Color", Color) = (0, 0, 0)
		_PointLightPosition("Point Light Position", Vector) = (0.0, 0.0, 0.0)
		_FalloffCoeff("Falloff Coefficient", Float) = 6
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
			uniform float3 _UnlitColor;
			uniform float _FalloffCoeff;

			uniform sampler2D _MainTex;
			uniform sampler2D _NormalMapTex;

			struct vertIn
			{
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				float4 tangent : TANGENT;
				float2 uv : TEXCOORD0;
			};

			struct vertOut
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
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
				float4 surfaceColor = tex2D(_MainTex, v.uv);

				float3 bump = (tex2D(_NormalMapTex, v.uv) - float3(0.5, 0.5, 0.5)) * 2.0; //brings normal into range of -1 to 1
				float3 bumpNormal = (bump.x * normalize(v.worldTangent)) +
					(bump.y * normalize(v.worldBinormal)) +
					(bump.z * normalize(v.worldNormal));
				bumpNormal = normalize(bumpNormal);

				// Don't want any ambient lighting

				// Calculate diffuse RBG reflections, we save the results of L.N because we will use it again
				// (when calculating the reflected ray in our specular component)
				float dist = length(_PointLightPosition - v.worldVertex.xyz);
				float fAtt = 1 / dist;
				float Kd = 1;
				float3 L = normalize(_PointLightPosition - v.worldVertex.xyz);
				float LdotN = dot(L, bumpNormal);
				float3 dif = (_FalloffCoeff * fAtt * fAtt) * _PointLightColor.rgb * Kd * surfaceColor *saturate(LdotN);
				
				// Calculate specular reflections
				float Ks = 3;
				float specN = 500; // Values>>1 give tighter highlights
				float3 V = normalize(_WorldSpaceCameraPos - v.worldVertex.xyz);
				// Using classic reflection calculation:
				//float3 R = normalize((2.0 * LdotN * interpNormal) - L);
				//float3 spe = fAtt * _PointLightColor.rgb * Ks * pow(saturate(dot(V, R)), specN);
				// Using Blinn-Phong approximation:
				float3 H = normalize(V + L);
				float3 spe = fAtt * _PointLightColor.rgb * Ks * pow(saturate(dot(bumpNormal, H)), specN);

				// Combine Phong illumination model components
				float4 returnColor = float4(0.0f, 0.0f, 0.0f, 0.0f);
				returnColor.rgb = dif.rgb + spe.rgb;
				
				
				returnColor.a = surfaceColor.a;

				return returnColor;
			}
			ENDCG
		}
	}
}
