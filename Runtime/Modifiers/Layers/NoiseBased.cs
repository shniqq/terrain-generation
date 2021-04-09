using System;
using NaughtyAttributes;
using TerrainGeneration.Utils;
using UnityEngine;

namespace TerrainGeneration.Modifiers.Layers
{
    [Serializable]
    public class NoiseBased : AbstractLayerModifier
    {
        [Serializable]
        private class TerrainLayerSettings
        {
            [MinMaxSlider(0, 1)] public Vector2 HeightRange = new Vector2(0, 1);
            [MinMaxSlider(0, 90)] public Vector2 SteepnessRange = new Vector2(0, 90);
            [Range(0, 1)] public float NoiseScale;
            public float NoiseScale2;

            public bool IsApplicable(float height, float steepness)
            {
                var isInSteepnessRange = steepness >= SteepnessRange.x &&
                                         steepness <= SteepnessRange.y;
                var isInHeightRange = height >= HeightRange.x &&
                                      height <= HeightRange.y;
                var isApplicable = isInSteepnessRange && isInHeightRange;
                return isApplicable;
            }
        }

        [SerializeField, AllowNesting] private TerrainLayerSettings _terrainLayerSettings;


        protected override float DetermineLayerAlpha(TerrainData terrainData, int x, int y)
        {
            var height = terrainData.GetHeightRelative(x, y);
            var steepness = terrainData.GetSteepnessAtPoint(x, y);
            if (!_terrainLayerSettings.IsApplicable(height, steepness))
            {
                return 0;
            }
            
            var noiseXPosition = x / (float) terrainData.alphamapHeight * _terrainLayerSettings.NoiseScale2
                                 + Seed;
            var noiseYPosition = y / (float) terrainData.alphamapWidth * _terrainLayerSettings.NoiseScale2
                                 + Seed;
            var noiseValue = Mathf.PerlinNoise(noiseXPosition, noiseYPosition)
                             * _terrainLayerSettings.NoiseScale;
            return noiseValue;
        }
    }
}