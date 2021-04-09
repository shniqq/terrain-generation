using System;
using UnityEngine;

namespace TerrainGeneration.Modifiers.Texture
{
    [Serializable]
    public class SampleSimplex2SNoise : AbstractTextureModifier
    {
        private FastNoiseLite _fastNoiseLite;

        [SerializeField, Range(0, 0.1f)] private float _frequency;
        [SerializeField, Range(0, 10f)] private float _fractalGain;
        [SerializeField, Range(0, 10f)] private float _fractalLucanarity;
        [SerializeField, Range(1, 10)] private int _fractalOctaves = 1;
        [SerializeField] private FastNoiseLite.FractalType _fractalType;
        [SerializeField, Range(0, 10f)] private float _fractalWeightedStrength;
        [SerializeField, Range(0, 10f)] private float _fractalPingPongStrength;

        public override void Setup(ushort seed, int size)
        {
            base.Setup(seed, size);

            _fastNoiseLite = new FastNoiseLite(seed);
            _fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);

            _fastNoiseLite.SetFrequency(_frequency);

            _fastNoiseLite.SetFractalGain(_fractalGain);
            _fastNoiseLite.SetFractalLacunarity(_fractalLucanarity);
            _fastNoiseLite.SetFractalOctaves(_fractalOctaves);
            _fastNoiseLite.SetFractalType(_fractalType);
            _fastNoiseLite.SetFractalWeightedStrength(_fractalWeightedStrength);
            _fastNoiseLite.SetFractalPingPongStrength(_fractalPingPongStrength);
        }

        protected override float SamplePixel(int x, int y, float current)
        {
            return _fastNoiseLite.GetNoise(x, y);
        }
    }
}