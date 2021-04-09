using System;
using UnityEngine;

namespace TerrainGeneration.Modifiers.Texture
{
    [Serializable]
    public class SamplePerlinNoise : AbstractTextureModifier
    {
        [SerializeField, Range(0, 50f)] private float _scale = 1f;

        protected override float SamplePixel(int x, int y, float current)
        {
            var xOffset = x / (double) Size * _scale;
            var xPosition = Seed + xOffset;

            var yOffset = y / (double) Size * _scale;
            var yPosition = Seed + yOffset;

            return Mathf.Clamp01(Mathf.PerlinNoise((float) xPosition, (float) yPosition));
        }
    }
}