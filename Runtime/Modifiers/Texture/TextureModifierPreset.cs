using NaughtyAttributes;
using TerrainGeneration.Utils;
using UnityEngine;

namespace TerrainGeneration.Modifiers.Texture
{
    [CreateAssetMenu(menuName = Constants.MenuPrefix + nameof(TextureModifierPreset))]
    public class TextureModifierPreset : ScriptableObject, ITextureModifier
    {
        public bool Enabled => _enabled;
        [SerializeField] private bool _enabled = true;
        [SerializeField, ShowAssetPreview, ShowIf(nameof(HasPreview))]
        private Texture2D _preview;
        private bool HasPreview => _preview != null;

        [SerializeReference, SerializeReferenceButton, AllowNesting]
        private ITextureModifier _textureModifier;

        [SerializeField] private ushort _previewSeed;

        public void Setup(ushort seed, int size)
        {
            _textureModifier.Setup(seed, size);
        }

        public void Modify(Texture2D texture)
        {
            _textureModifier.Modify(texture);
        }

        [Button]
        private void GeneratePreview()
        {
            Setup(_previewSeed, 512);
            var preview = new Texture2D(512, 512);
            Modify(preview);
            _preview = preview;
        }
    }
}