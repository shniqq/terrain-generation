using System;
using UnityEngine;

namespace TerrainGeneration.Modifiers.Texture
{
    [Serializable]
    public class ClampValue : AbstractTextureModifier
    {
        [SerializeField, Range(0, 1)] private float _min;
        [SerializeField, Range(0, 1)] private float _max = 1;

        [SerializeField] private bool _remap;

        protected override float SamplePixel(int x, int y, float current)
        {
            current = Mathf.Clamp(current, _min, _max);
            if (_remap)
            {
                current = Utils.Utils.Remap(current, _min, _max, 0, 1);
            }

            return current;
        }
    }
}