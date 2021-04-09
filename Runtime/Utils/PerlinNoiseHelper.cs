using UnityEngine;

namespace TerrainGeneration.Utils
{
    public static class PerlinNoiseHelper
    {
        public static float SampleNoise(int size, int x, int y, float scale, float seed)
        {
            var noiseOriginX = scale * x / size + seed;
            var noiseOriginY = scale * y / size + seed;
            var perlinNoise = Mathf.PerlinNoise(noiseOriginX, noiseOriginY);
            var sample = Mathf.Clamp01(perlinNoise);
            return sample;
        }
    }
}