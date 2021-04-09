using System;
using UnityEngine;

namespace TerrainGeneration.Modifiers.Texture
{
    [Serializable]
    public class RemapValues : AbstractTextureModifier
    {
        [SerializeField, Range(0, 1f)] private float _newMin;
        [SerializeField, Range(0, 1f)] private float _newMax = 1f;
        [SerializeField, Range(0, 1f)] private float _oldMin;
        [SerializeField, Range(0, 1f)] private float _oldMax = 1f;

        protected override float SamplePixel(int x, int y, float current)
        {
            return Utils.Utils.Remap(current, _oldMin, _oldMax, _newMin, _newMax);
        }
    }
}