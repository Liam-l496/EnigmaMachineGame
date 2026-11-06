
Shader "Custom/ElectricityWire"
{
    Properties
    {
        [NoScaleOffset]
        _NoiseTex("Electrical Noise Texture", 2D) = "white" {}

        [HDR]
        _EmissionColor("Emission Colour", Color) =
            (0.1, 0.8, 1, 1)

        _EmissionIntensity("Emission Intensity", Float) = 5

        _FlowSpeed("Flow Speed", Float) = 2

        _TileCount("Tile Count", Float) = 5

        _JitterFrequency("Jitter Frequency", Float) = 12

        _JitterAmplitude("Jitter Amplitude", Float) = 0.008

        _PulseWidth("Pulse Width", Range(0.01, 0.5)) = 0.12

        _PulseSoftness("Pulse Softness", Range(0.01, 0.5)) = 0.18

        _BaseGlow("Base Glow", Range(0, 1)) = 0.08

        _Progress("Electrical Progress", Range(0, 1)) = 0

        _TrailLength("Electrical Trail Length", Range(0.01, 1.1)) = 1.1

        _FrontWidth("Electrical Front Width", Range(0.001, 0.1)) = 0.02

        [MainTexture]
        _WireTex("Normal Wire Texture", 2D) = "white" {}

        [MainColor]
        _WireColor("Wire Base Colour", Color) = (1, 1, 1, 1)

        _OffProgress("Electrical Shutdown Progress", Range(0, 1)) = 1

        _UnpoweredAlpha("Unpowered Cable Transparency", Range(0, 1)) = 1
        _PoweredAlpha("Powered Cable Transparency", Range(0, 1)) = 1

        [HideInInspector]
        _ReverseDirection("Reverse Direction", Float) = 0
    }

    SubShader
    {
        Tags
        {
            // "RenderType" = "Opaque"
            // "RenderPipeline" = "UniversalPipeline"
            // "Queue" = "Geometry"
        
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        Pass
        {
            Name "Electricity"

            Cull Off
            ZWrite On
            ZTest LEqual

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);
            TEXTURE2D(_WireTex);
            SAMPLER(sampler_WireTex);

            CBUFFER_START(UnityPerMaterial)

                float4 _EmissionColor;

                float _EmissionIntensity;
                float _FlowSpeed;
                float _TileCount;

                float _JitterFrequency;
                float _JitterAmplitude;

                float _PulseWidth;
                float _PulseSoftness;
                float _BaseGlow;

                float _Progress;
                float _TrailLength;
                float _FrontWidth;

                float4 _WireColor;
                float4 _WireTex_ST;

                float _OffProgress;

                float _UnpoweredAlpha;
                float _PoweredAlpha;

                float _ReverseDirection;

            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;

                float4 tangentOS : TANGENT;

                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;

                float2 uv : TEXCOORD0;
            };

            // Varyings vert(Attributes IN)
            // {
            //     Varyings OUT;

            //     float3 position = IN.positionOS.xyz;

            //     float3 tangent = normalize(IN.tangentOS.xyz);

            //     // Sample animated noise for electrical crackling.
            //     float jitterNoise = SAMPLE_TEXTURE2D_LOD(
            //         _NoiseTex,
            //         sampler_NoiseTex,
            //         float2(
            //             IN.uv.x * _JitterFrequency
            //             + _Time.y * _JitterFrequency,
            //             0.37
            //         ),
            //         0
            //     ).r;

            //     // Convert noise from 0..1 to -1..1.
            //     float jitter = (jitterNoise * 2.0 - 1.0)
            //         * _JitterAmplitude;

            //     // Displace vertices along the wire tangent.
            //     float startupMask = 1.0 - smoothstep(
            //         _Progress,
            //         _Progress + _FrontWidth,
            //         IN.uv.x
            //     );

            //     float shutdownMask = smoothstep(
            //         _OffProgress,
            //         _OffProgress + _FrontWidth,
            //         IN.uv.x
            //     );

            //     float activation = startupMask * shutdownMask;

            //     position += tangent * jitter * activation;

            //     OUT.positionCS = TransformObjectToHClip(position);

            //     OUT.uv = IN.uv;

            //     return OUT;
            // }


            
            // half4 frag(Varyings IN) : SV_Target
            // {
            //     // ----------------------------------
            //     // 1. Sample the normal wire texture
            //     // ----------------------------------

            //     float2 wireUV = TRANSFORM_TEX(
            //         IN.uv, _WireTex
            //     );

            //     half4 wireTexture = SAMPLE_TEXTURE2D(
            //         _WireTex,
            //         sampler_WireTex,
            //         wireUV
            //     );

            //     float3 baseColour =
            //         wireTexture.rgb * _WireColor.rgb;


            //     // ----------------------------------
            //     // 2. Calculate startup progress
            //     // ----------------------------------

            //     float startupMask = 1.0 - smoothstep(
            //         _Progress,
            //         _Progress + _FrontWidth,
            //         IN.uv.x
            //     );


            //     // ----------------------------------
            //     // 3. Calculate shutdown progress
            //     // ----------------------------------

            //     float shutdownMask = smoothstep(
            //         _OffProgress,
            //         _OffProgress + _FrontWidth,
            //         IN.uv.x
            //     );


            //     // ----------------------------------
            //     // 4. Combine activation masks
            //     // ----------------------------------

            //     float activation =
            //         startupMask * shutdownMask;


            //     // ----------------------------------
            //     // 5. Calculate the electrical trail
            //     // ----------------------------------

            //     float distanceBehind =
            //         _Progress - IN.uv.x;

            //     float trailFade = 1.0 - smoothstep(
            //         _TrailLength - _FrontWidth,
            //         _TrailLength,
            //         distanceBehind
            //     );

            //     activation *= trailFade;


            //     // ----------------------------------
            //     // 6. Sample animated electricity
            //     // ----------------------------------

            //     float animatedU =
            //         IN.uv.x * _TileCount
            //         - _Time.y * _FlowSpeed;

            //     float2 noiseUV = float2(
            //         animatedU,
            //         IN.uv.y
            //     );

            //     float noise = SAMPLE_TEXTURE2D(
            //         _NoiseTex,
            //         sampler_NoiseTex,
            //         noiseUV
            //     ).r;


            //     // ----------------------------------
            //     // 7. Electrical detail
            //     // ----------------------------------

            //     float electricalDetail = smoothstep(
            //         0.25,
            //         0.8,
            //         noise
            //     );


            //     // ----------------------------------
            //     // 8. Calculate HDR emission
            //     // ----------------------------------

            //     float finalIntensity =
            //         activation *
            //         (_BaseGlow + electricalDetail);

            //     float3 emission =
            //         _EmissionColor.rgb *
            //         finalIntensity *
            //         _EmissionIntensity;


            //     // ----------------------------------
            //     // 9. Calculate transparency
            //     // ----------------------------------

            //     float cableAlpha = lerp(
            //         _UnpoweredAlpha,
            //         _PoweredAlpha,
            //         activation
            //     );

            //     // Preserve the original wire texture's alpha.
            //     cableAlpha *= wireTexture.a;


            //     // ----------------------------------
            //     // 10. Combine wire and electricity
            //     // ----------------------------------

            //     float3 finalColour =
            //         baseColour + emission;

            //     return half4(finalColour, cableAlpha);
            // }

            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                // Reverse UV direction without rebuilding the mesh.
                float wireU = lerp(
                    IN.uv.x,
                    1.0 - IN.uv.x,
                    _ReverseDirection
                );

                float3 position = IN.positionOS.xyz;

                float3 tangent = normalize(IN.tangentOS.xyz);

                float startupMask = 1.0 - smoothstep(
                    _Progress,
                    _Progress + _FrontWidth,
                    wireU
                );

                float shutdownMask = smoothstep(
                    _OffProgress,
                    _OffProgress + _FrontWidth,
                    wireU
                );

                float activation = startupMask * shutdownMask;

                // Skip the noise texture sample when jitter
                // is disabled or electricity is inactive.
                if (_JitterAmplitude > 0.000001 && activation > 0.001)
                {
                    float jitterNoise = SAMPLE_TEXTURE2D_LOD(
                        _NoiseTex,
                        sampler_NoiseTex,
                        float2(
                            wireU * _JitterFrequency
                            + _Time.y * _JitterFrequency,
                            0.37
                        ),
                        0
                    ).r;

                    float jitter = (jitterNoise * 2.0 - 1.0)
                        * _JitterAmplitude;

                    position += tangent * jitter * activation;
                }

                OUT.positionCS = TransformObjectToHClip(position);

                // Pass the reversed UV to the fragment shader.
                OUT.uv = float2(wireU, IN.uv.y);

                return OUT;
            }


            
            half4 frag(Varyings IN) : SV_Target
            {
                // ----------------------------------
                // 1. Sample the normal wire texture
                // ----------------------------------

                float2 wireUV = TRANSFORM_TEX(
                    IN.uv, _WireTex
                );

                half4 wireTexture = SAMPLE_TEXTURE2D(
                    _WireTex,
                    sampler_WireTex,
                    wireUV
                );

                half3 baseColour =
                    wireTexture.rgb * _WireColor.rgb;


                // ----------------------------------
                // 2. Calculate electrical activation
                // ----------------------------------

                float startupMask = 1.0 - smoothstep(
                    _Progress,
                    _Progress + _FrontWidth,
                    IN.uv.x
                );

                float shutdownMask = smoothstep(
                    _OffProgress,
                    _OffProgress + _FrontWidth,
                    IN.uv.x
                );

                float activation = startupMask * shutdownMask;


                // ----------------------------------
                // 3. Calculate electrical trail
                // ----------------------------------

                float distanceBehind =
                    _Progress - IN.uv.x;

                float trailFade = 1.0 - smoothstep(
                    _TrailLength - _FrontWidth,
                    _TrailLength,
                    distanceBehind
                );

                activation *= trailFade;


                // ----------------------------------
                // 4. Calculate cable transparency
                // ----------------------------------

                half cableAlpha = lerp(
                    _UnpoweredAlpha,
                    _PoweredAlpha,
                    activation
                );

                cableAlpha *= wireTexture.a;


                // ----------------------------------
                // 5. Skip electrical calculations
                //    when electricity is inactive
                // ----------------------------------

                if (activation <= 0.0001)
                {
                    return half4(baseColour, cableAlpha);
                }


                // ----------------------------------
                // 6. Sample animated electrical noise
                // ----------------------------------

                float animatedU =
                    IN.uv.x * _TileCount
                    - _Time.y * _FlowSpeed;

                float2 noiseUV = float2(
                    animatedU,
                    IN.uv.y
                );

                float noise = SAMPLE_TEXTURE2D(
                    _NoiseTex,
                    sampler_NoiseTex,
                    noiseUV
                ).r;


                // ----------------------------------
                // 7. Electrical detail
                // ----------------------------------

                float electricalDetail = smoothstep(
                    0.25,
                    0.8,
                    noise
                );


                // ----------------------------------
                // 8. HDR emission
                // ----------------------------------

                float finalIntensity =
                    activation *
                    (_BaseGlow + electricalDetail);

                half3 emission =
                    _EmissionColor.rgb *
                    finalIntensity *
                    _EmissionIntensity;


                // ----------------------------------
                // 9. Final colour
                // ----------------------------------

                half3 finalColour = baseColour + emission;

                return half4(finalColour, cableAlpha);
            }




            ENDHLSL
        }
    }
}
