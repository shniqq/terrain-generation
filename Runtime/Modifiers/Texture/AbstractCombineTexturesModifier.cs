using System.Linq;
using NaughtyAttributes;
using TerrainGeneration.Utils;
using UnityEngine;

namespace TerrainGeneration.Modifiers.Texture
{
    public abstract class AbstractCombineTexturesModifier : AbstractTextureModifier
    {
        [SerializeReference, SerializeReferenceButton, AllowNesting]
        private ITextureModifier _otherTextureModifier;

        private float[] _otherTextureData;

        public override void Setup(ushort seed, int size)
        {
            base.Setup(seed, size);

            _otherTextureModifier.Setup(seed, size);
        }

        protected float SamplePixelFromOther(int x, int y)
        {
            return _otherTextureData.ElementAt(Map2DIndexTo1DIndex(x, y));
        }

        public override void Modify(Texture2D texture)
        {
            var otherTexture = texture.Clone();
            _otherTextureModifier.Modify(otherTexture);
            _otherTextureData = otherTexture.GetPixels().Select(e => e.grayscale).ToArray();

            var pixelData = texture.GetPixels().Select(e => e.grayscale).ToArray();
            for (var y = 0; y < Size; y++)
            {
                for (var x = 0; x < Size; x++)
                {
                    var index = Map2DIndexTo1DIndex(x, y);
                    var currentHeight = pixelData[index];
                    currentHeight = SamplePixel(x, y, currentHeight);
                    pixelData[index] = currentHeight;
                }
            }

            texture.SetPixels(pixelData.Select(e => new Color(e, e, e)).ToArray());
            texture.Apply();
            SetPreview(texture);
        }
    }
}