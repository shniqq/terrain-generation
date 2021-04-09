using System;
using NaughtyAttributes;
using TerrainGeneration.Utils;
using UnityEngine;

namespace TerrainGeneration.Modifiers.Texture
{
    [Serializable]
    public class CombineTextures : AbstractCombineTexturesModifier
    {
        [SerializeField] private CombinationMethod _combinationMethod;
        [SerializeField, Range(0, 1)] private float _weight = 1f;
        [SerializeField, MinMaxSlider(0, 1)] private Vector2 _applicableRange;

        protected override float SamplePixel(int x, int y, float current)
        {
            var other = SamplePixelFromOther(x, y);

            if (!_applicableRange.IsValueWithinXY(other))
            {
                return current;
            }

            switch (_combinationMethod)
            {
                case CombinationMethod.None:
                    return current;
                case CombinationMethod.Add:
                    return current + other * _weight;
                case CombinationMethod.Subtract:
                    return current - other * _weight;
                case CombinationMethod.Multiply:
                    return other * _weight * current;
                case CombinationMethod.Lerp:
                    return Mathf.Lerp(current, other, _weight);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public enum CombinationMethod
        {
            None,
            Add,
            Subtract,
            Multiply,
            Lerp
        }
    }
}