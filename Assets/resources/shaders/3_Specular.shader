Shader "MyShaders/Specular"
{
	Properties
	{
		_Color ("Color", Color) = (1.0,1.0,1.0,1.0)
		_SpecColor ("Specular Color", Color) = (1.0,1.0,1.0,1.0)
		_Shininess ("Shininess", Float) = 10
	}

	SubShader
	{
		Tags { "LightMode" = "ForwardBase"} // confirmed still needed in unity 5 so weird things don't happen

		Pass
		{
			
			CGPROGRAM // switches from shaderlab to cg
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			#pragma multi_compile_fwdbase
			#include "AutoLight.cginc"


			// user defined variables
			uniform float4 _Color;
			uniform float4 _SpecColor;
			uniform float _Shininess;

			// unity defined variables
			uniform float4 _LightColor0;

			// base input structs
			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 col : COLOR;	
				LIGHTING_COORDS(0,1)
			};

			//vertex func
			vertexOutput vert(vertexInput v)
			{
				vertexOutput o;
				//vectors
				float3 normalDirection = normalize(mul(float4(v.normal, 0.0), _World2Object).xyz );
				//calculating distance between camera pos and vertex pos effectively
				// 
				float3 viewDirection = normalize(float3(float4(_WorldSpaceCameraPos.xyz , 1.0) - mul(_Object2World, v.vertex).xyz ));
				//mul(_Object2World, v.vertex).xyz ) vertex pos in world space
				// float4(_WorldSpaceCameraPos.xyz , 1.0) camera pos in world space
				float3 lightDirection;
				float atten = 1.0;

				// lighting
				lightDirection = normalize(_WorldSpaceLightPos0.xyz );
				float3 diffuseReflection = atten * _LightColor0.xyz * max(0.0, dot(normalDirection , lightDirection));
				float3 specularReflection = atten * _SpecColor.rgb * max(0.0, dot(normalDirection , lightDirection)) * pow(max(0.0,dot(reflect(-lightDirection, normalDirection), viewDirection)), _Shininess);
				float3 lightFinal = diffuseReflection + specularReflection + UNITY_LIGHTMODEL_AMBIENT;

				o.col = float4(lightFinal * _Color.rgb ,1.0);
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}

			//fragment func
			float4 frag(vertexOutput i) : COLOR // don't forget semantics
			{
				float attenuation = LIGHT_ATTENUATION(i);
				return i.col;
			}

			ENDCG
		}
	}
	// fallback commented out during development
	// in case default shader fails and fallback masks errors
	// Fallback "Diffuse"

}

/*breakdown of specular reflection
float3 specularReflection = reflect(-lightDirection, normalDirection);
>First we create our reflection vector by reflecting the light across the normal

float3 specularReflection = dot(reflect(-lightDirection, normalDirection));
>then use this in a dot product to get our reflection on the surface
>as reflected vector gets closer to view direction, becomes 1, as it gets further away, becomes 0

float3 specularReflection = max(0.0, dot(reflect(-lightDirection, normalDirection)));
> make sure we don't get -ve values on other side of obj

float3 specularReflection = pow(max(0.0, dot(reflect(-lightDirection, normalDirection),viewDirection)));
> using pow func, can raise specular highlight to a power to control intensity

float3 specularReflection = max(0.0, dot(normalDirections, lightDirection)) * pow(max(0.0, dot(reflect(-lightDirection, normalDirection),viewDirection)),_Shininess);
> by multiplying it by the same dot product from lambertian shader we can fade highlight away as it approaches edge of lit area

float3 specularReflection = max(0.0, dot(normalDirections, lightDirection)) * atten * _LightColor0.rgb * _SpecColor.rgb * pow(max(0.0, dot(reflect(-lightDirection, normalDirection),viewDirection)),_Shininess);
> finally multiply specular highlight by attenuation (1 for directional lights) and specular light color
*/