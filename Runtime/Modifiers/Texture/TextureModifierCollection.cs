using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace TerrainGeneration.Modifiers.Texture
{
    [Serializable]
    public class TextureModifierCollection : AbstractTextureModifier
    {
        [SerializeReference, SerializeReferenceButton, AllowNesting]
        private List<ITextureModifier> _otherTextureModifiers;

        public override void Setup(ushort seed, int size)
        {
            base.Setup(seed, size);

            _otherTextureModifiers?.ForEach(modifier => modifier?.Setup(seed, size));
        }

        public override void Modify(Texture2D texture)
        {
            foreach (var modifier in _otherTextureModifiers.Where(e => e.Enabled))
            {
                modifier?.Modify(texture);
            }

            SetPreview(texture);
        }
    }
}