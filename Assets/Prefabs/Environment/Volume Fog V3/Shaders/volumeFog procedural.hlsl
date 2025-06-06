
// (Keijiro) This shader was slightly modified from the original version.
// It's recommended to use the original version for other purposes.

//
// Description : Array and textureless GLSL 2D/3D/4D simplex
//               noise functions.
//      Author : Ian McEwan, Ashima Arts.
//  Maintainer : ijm
//     Lastmod : 20110822 (ijm)
//     License : Copyright (C) 2011 Ashima Arts. All rights reserved.
//               Distributed under the MIT License. See LICENSE file.
//               https://github.com/ashima/webgl-noise
//

float3 mod289(float3 x)
{
    return x - floor(x * (1.0 / 289.0)) * 289.0;
}

float4 mod289(float4 x) {
    return x - floor(x * (1.0 / 289.0)) * 289.0;
}

float4 permute(float4 x)
{
    return mod289((x * 34.0 + 1.0) * x);
}

float4 taylorInvSqrt(float4 r)
{
    return 1.79284291400159 - 0.85373472095314 * r;
}

float snoise(float3 v)
{
    const float2 C = float2(1.0 / 6.0, 1.0 / 3.0);

    // First corner
    float3 i  = floor(v + dot(v, C.yyy));
    float3 x0 = v   - i + dot(i, C.xxx);

    // other corners
    float3 g = step(x0.yzx, x0.xyz);
    float3 l = 1.0 - g;
    float3 i1 = min(g.xyz, l.zxy);
    float3 i2 = max(g.xyz, l.zxy);

    // x1 = x0 - i1  + 1.0 * C.xxx;
    // x2 = x0 - i2  + 2.0 * C.xxx;
    // x3 = x0 - 1.0 + 3.0 * C.xxx;
    float3 x1 = x0 - i1 + C.xxx;
    float3 x2 = x0 - i2 + C.yyy;
    float3 x3 = x0 - 0.5;

    // Permutations
    i = mod289(i); // Avoid truncation effects in permutation
    float4 p =
      permute(permute(permute(i.z + float4(0.0, i1.z, i2.z, 1.0))
                            + i.y + float4(0.0, i1.y, i2.y, 1.0))
                            + i.x + float4(0.0, i1.x, i2.x, 1.0));

    // Gradients: 7x7 points over a square, mapped onto an octahedron.
    // The ring size 17*17 = 289 is close to a multiple of 49 (49*6 = 294)
    float4 j = p - 49.0 * floor(p * (1.0 / 49.0));  // fmod(p,7*7)

    float4 x_ = floor(j * (1.0 / 7.0));
    float4 y_ = floor(j - 7.0 * x_ );  // fmod(j,N)

    float4 x = x_ * (2.0 / 7.0) + 0.5 / 7.0 - 1.0;
    float4 y = y_ * (2.0 / 7.0) + 0.5 / 7.0 - 1.0;

    float4 h = 1.0 - abs(x) - abs(y);

    float4 b0 = float4(x.xy, y.xy);
    float4 b1 = float4(x.zw, y.zw);

    //float4 s0 = float4(lessThan(b0, 0.0)) * 2.0 - 1.0;
    //float4 s1 = float4(lessThan(b1, 0.0)) * 2.0 - 1.0;
    float4 s0 = floor(b0) * 2.0 + 1.0;
    float4 s1 = floor(b1) * 2.0 + 1.0;
    float4 sh = -step(h, float4(0,0,0,0));

    float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
    float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;

    float3 g0 = float3(a0.xy, h.x);
    float3 g1 = float3(a0.zw, h.y);
    float3 g2 = float3(a1.xy, h.z);
    float3 g3 = float3(a1.zw, h.w);

    // Normalise gradients
    float4 norm = taylorInvSqrt(float4(dot(g0, g0), dot(g1, g1), dot(g2, g2), dot(g3, g3)));
    g0 *= norm.x;
    g1 *= norm.y;
    g2 *= norm.z;
    g3 *= norm.w;

    // Mix final noise value
    float4 m = max(0.6 - float4(dot(x0, x0), dot(x1, x1), dot(x2, x2), dot(x3, x3)), 0.0);
    m = m * m;
    m = m * m;

    float4 px = float4(dot(x0, g0), dot(x1, g1), dot(x2, g2), dot(x3, g3));
    return 42.0 * dot(m, px);
}

float sampleNoises(float3 vec) {
    return snoise(vec) + 0.5 * snoise(vec * 4)+ 0.25 * snoise(vec * 12) + 0.125;
}

float Unity_RandomRange_float(float2 Seed)
{
    float randomno =  frac(sin(dot(Seed, float2(12.9898, 78.233)))*43758.5453);
    return  randomno - 0.5;
} 

SAMPLER(sampler_linear_repeat);
void volumeFog_float(
    Texture2D Noise2DTexture,
    float Noise2DScale,
    float Samples,
    float MeshDistance,
    float StartingHeight,
    float OverallHeight,
    float randomness,
    float3 Size,
    float Threshold,
    float Multiplier,
    float MaxDistance,
    float3 Position,
    float3 View,
    out float Fog
    ) { 
    Fog = 0;

    if ((Position.y < StartingHeight - OverallHeight && View.y >= 0) || (Position.y > StartingHeight && View.y <= 0)) {
        return;
    }

    float OverallDistance;
    float yDistanceToStart = StartingHeight - Position.y;
    float yDistanceToEnd = Position.y - (StartingHeight - OverallHeight);
    float distanceToStart;
    float distanceToEnd;
    float maxDistance;

    bool between = Position.y < StartingHeight && Position.y > StartingHeight - OverallHeight;
    if (View.y > 0) {
        // above fog
        distanceToStart =  length((yDistanceToStart / View.y) * View);
        distanceToEnd =  length((yDistanceToEnd / View.y) * View);
        

    } else {
        // below fog
        distanceToStart =  length(((yDistanceToEnd) / View.y) * View);
        distanceToEnd =  length(((yDistanceToStart) / View.y) * View);


    }

 
    distanceToStart =  between? 0 : distanceToStart;
    OverallDistance = abs(distanceToStart - distanceToEnd) * -1;


    Samples = distanceToStart < MeshDistance ? min(Samples, 50) : 0; // 50 will be the maximum samples, you can change this value if needed

    float Distance = max(OverallDistance,( distanceToStart - MeshDistance )) / Samples;

    float random = Unity_RandomRange_float(View.xz);
    float3 randVec = random * randomness * View * (Distance / 10);


    float3 p;
    float yDistance;
    float noise;
    float topBottomFade;
    float3 vectorToAdd;

    float3 vectorToStart = (View * distanceToStart * -1);

    [loop]
    for (int i = 0; i < Samples ; i++) {
        if (Fog >= 4.5) {
            break;
        }
        
        vectorToAdd = vectorToStart + View * Distance * i;
        if (length(vectorToAdd) > MaxDistance + random) {
            Fog += 0.02;
            continue;
        }

        p = Position + vectorToAdd;
        yDistance =  StartingHeight - p.y;

        p += randVec;
        p *= Size;
        
        float n3D = sampleNoises(p);
        float n = SAMPLE_TEXTURE2D(Noise2DTexture, sampler_linear_repeat , p.xz * Noise2DScale).x  - .5;
        noise = n + n3D ; 
        topBottomFade = saturate(yDistance* 1.25)  * saturate((OverallHeight - yDistance)* 1.25);
        Fog += saturate((noise - Threshold) * Multiplier * topBottomFade) ;
    }

    Fog = 1 - saturate (exp(-Fog));
}