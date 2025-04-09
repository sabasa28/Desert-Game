using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NoiseLevel
{ 
    none,
    low,
    medium,
    high,
    global
}

public struct Noise
{
    public NoiseLevel noiseLevel;
    public Vector2 noiseOrigin;
    public Noise(NoiseLevel newNoiseLevel, Vector2 newNoieOrigin)
    {
        noiseLevel = newNoiseLevel;
        noiseOrigin = newNoieOrigin;
    }
}

public interface INoisy 
{
    void NoiseWasMade(Noise noise);
}
