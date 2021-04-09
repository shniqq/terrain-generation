using System;
using System.Linq;
using NaughtyAttributes;
using TerrainGeneration.Utils;
using UnityEngine;

namespace TerrainGeneration.Modifiers.Texture
{
    [Serializable]
    public abstract class AbstractTextureModifier : ITextureModifier
    {
        protected ushort Seed { get; private set; }
        protected int Size { get; private set; }

        public bool Enabled => _enabled;
        [SerializeField] private bool _enabled = true;
        
        [SerializeField, ShowAssetPreview, ShowIf(nameof(HasPreview))]
        private Texture2D _preview;

        private bool HasPreview => _preview != null;

        public virtual void Setup(ushort seed, int size)
        {
            Seed = seed;
            Size = size;
        }

        protected virtual float SamplePixel(int x, int y, float current)
        {
            return current;
        }

        public virtual void Modify(Texture2D texture)
        {
            var heightData = texture.GetPixels().Select(e => e.grayscale).ToArray();
            for (var y = 0; y < Size; y++)
            {
                for (var x = 0; x < Size; x++)
                {
                    var index = Map2DIndexTo1DIndex(x, y);
                    var currentHeight = heightData[index];
                    currentHeight = SamplePixel(x, y, currentHeight);
                    heightData[index] = currentHeight;
                }
            }

            texture.SetPixels(heightData.Select(e => new Color(e, e, e)).ToArray());
            texture.Apply();
            SetPreview(texture);
        }

        protected int Map2DIndexTo1DIndex(int x, int y)
        {
            return y + Size * x;
        }

        protected void SetPreview(Texture2D texture)
        {
            _preview = texture.Clone();
        }
    }
}