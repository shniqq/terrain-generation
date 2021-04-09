using System;
using UnityEngine;

namespace TerrainGeneration.Modifiers.Texture
{
    [Serializable]
    public class CombineTexturesCurve : AbstractCombineTexturesModifier
    {
        [SerializeField] private AnimationCurve _curve;
        
        protected override float SamplePixel(int x, int y, float current)
        {
            var other = SamplePixelFromOther(x, y);

            var lerp = _curve.Evaluate(other);

            return Mathf.Lerp(current, other, lerp);
        }
    }
}