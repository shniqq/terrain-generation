using UnityEngine;

namespace TerrainGeneration.Modifiers.Texture
{
    public interface ITextureModifier
    {
        bool Enabled { get; }
        void Setup(ushort seed, int size);
        void Modify(Texture2D texture);
    }
}