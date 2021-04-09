using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using Random = System.Random;

namespace TerrainGeneration.Utils
{
    [Serializable]
    internal class RandomFloatValueProvider : IFloatValueProvider
    {
        private Random _random;

        [SerializeField, MinMaxSlider(1, 100)] private Vector2 _valueRange;

        public void Setup(ushort seed)
        {
            _random = new Random(seed);
        }

        public float GetValue()
        {
            var value = TerrainGeneration.Utils.Utils.Remap((float) _random.NextDouble(), 0, 1, _valueRange.x, _valueRange.y);
            Assert.IsTrue(_valueRange.IsValueWithinXY(value));
            return value;
        }
    }
}