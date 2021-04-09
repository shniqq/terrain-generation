using System;
using UnityEngine;

namespace TerrainGeneration.Modifiers.Texture
{
    [Serializable]
    public class SampleVoronoiNoise : AbstractTextureModifier
    {
        private FastNoiseLite _fastNoiseLite;

        [SerializeField] private FastNoiseLite.CellularDistanceFunction _cellularDistanceFunction;
        [SerializeField] private FastNoiseLite.CellularReturnType _cellularReturnType;
        [SerializeField, Range(0, 10f)] private float _cellularJitter;
        [SerializeField, Range(0, 5f)] private float _frequency;

        public override void Setup(ushort seed, int size)
        {
            base.Setup(seed, size);

            _fastNoiseLite = new FastNoiseLite(seed);
            _fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            _fastNoiseLite.SetCellularDistanceFunction(_cellularDistanceFunction);
            _fastNoiseLite.SetCellularJitter(_cellularJitter);
            _fastNoiseLite.SetCellularReturnType(_cellularReturnType);

            _fastNoiseLite.SetFrequency(_frequency);
        }

        protected override float SamplePixel(int x, int y, float current)
        {
            return _fastNoiseLite.GetNoise(x, y);
        }
    }
}