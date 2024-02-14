sampler uImage0 : register(s0); 
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition; 
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

float PI = 3.14159265359;

float4 Shockwave(float4 position : SV_POSITION, float2 coords : TEXCOORD0) : COLOR0
{
    // Calculate the distance from the current pixel to the center of the shockwave
    float2 distanceToCenter = coords - uTargetPosition.xy / uScreenResolution.xy;
    float distance = length(distanceToCenter);

    // Check if the pixel is within the specified radius (uIntensity)
    if (distance < uIntensity)
    {
        // Calculate coordinates relative to the shockwave center
        float2 centerCoords = (coords - uTargetPosition.xy) / uScreenResolution.xy;

        // Calculate dot field and ripple
        float dotField = dot(centerCoords, centerCoords);
        float ripple = dotField * uColor.y * PI - uProgress * uColor.z;

        // Apply ripple effect only within the specified radius
        if (ripple < 0 && ripple > uColor.x * -2 * PI)
        {
            ripple = saturate(sin(ripple));
        }
        else
        {
            ripple = 0;
        }

        // Apply distortion within the sphere
        float2 sampleCoords = coords + ((ripple * uOpacity / uScreenResolution) * centerCoords);

        return tex2D(uImage0, sampleCoords);
    }
    else
    {
        // Return original color for pixels outside the specified radius
        return tex2D(uImage0, coords);
    }
}

technique Technique1 
{
    pass ShockwavePass
    {
        PixelShader = compile ps_2_0 Shockwave();
    }
}