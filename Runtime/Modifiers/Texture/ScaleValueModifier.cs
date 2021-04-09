using System;
using UnityEngine;

namespace TerrainGeneration.Modifiers.Texture
{
    [Serializable]
    public class ScaleValueModifier : AbstractTextureModifier
    {
        [SerializeField, Range(0, 1)] private float _scale;

        protected override float SamplePixel(int x, int y, float current)
        {
            return current * _scale;
        }
    }
}