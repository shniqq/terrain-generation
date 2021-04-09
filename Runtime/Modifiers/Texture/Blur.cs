using System;
using NaughtyAttributes;
using Unity.Jobs;
using UnityEngine;

namespace TerrainGeneration.Modifiers.Texture
{
    [Serializable]
    public class Blur : AbstractTextureModifier
    {
        [SerializeField] private BlurType _blurType;
        [SerializeField, ShowIf(nameof(IsBoxBlur)), AllowNesting]
        private int _blurSize;
        [SerializeField, ShowIf(nameof(IsGaussianBlur)), AllowNesting]
        private float _sigma;

        private bool IsBoxBlur => _blurType == BlurType.BoxBlur;
        private bool IsGaussianBlur => _blurType == BlurType.Gaussian;

        public override void Modify(Texture2D texture)
        {
            if (texture.isReadable == false)
            {
                var error = new UnityException("This texture is not readable!");
                Debug.LogException(error);
                return;
            }

            if (texture.format != TextureFormat.RGBA32)
            {
                var error = new UnityException(
                    $"Wrong format. Texture is not {TextureFormat.RGB24} but {texture.format}!");
                Debug.LogException(error);
                return;
            }

            switch (_blurType)
            {
                case BlurType.BoxBlur:
                    BoxBlur(texture);
                    break;
                case BlurType.Gaussian:
                    GaussianBlur(texture);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            SetPreview(texture);
        }

        private void GaussianBlur(Texture2D texture)
        {
            var job = new GaussianBlurRGBA32Job(texture.GetRawTextureData<RGBA32>(), texture.width, texture.height,
                _sigma);
            job.Schedule().Complete();
            texture.Apply();
        }

        private void BoxBlur(Texture2D texture)
        {
            var job = new BoxBlurRGBA32Job(
                texture.GetRawTextureData<RGBA32>(),
                texture.width,
                texture.height,
                _blurSize
            );
            job.Schedule().Complete();
            texture.Apply();
        }

        public enum BlurType
        {
            BoxBlur,
            Gaussian
        }
    }
}