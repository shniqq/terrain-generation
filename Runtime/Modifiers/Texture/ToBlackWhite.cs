using System;
using UnityEngine;

namespace TerrainGeneration.Modifiers.Texture
{
    [Serializable]
    public class ToBlackWhite : AbstractTextureModifier
    {
        [SerializeField, Range(0, 1)] private float _threshold = 0.5f;

        protected override float SamplePixel(int x, int y, float current)
        {
            return current >= _threshold ? 1 : 0;
        }
    }
}