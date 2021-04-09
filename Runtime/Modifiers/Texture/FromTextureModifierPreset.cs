using System;
using NaughtyAttributes;
using UnityEngine;

namespace TerrainGeneration.Modifiers.Texture
{
    [Serializable]
    public class FromTextureModifierPreset : AbstractTextureModifier
    {
        [SerializeField, AllowNesting] private TextureModifierPreset _textureModifierPreset;

        public override void Setup(ushort seed, int size)
        {
            _textureModifierPreset.Setup(seed, size);
        }

        public override void Modify(Texture2D texture)
        {
            _textureModifierPreset.Modify(texture);
            SetPreview(texture);
        }
    }
}