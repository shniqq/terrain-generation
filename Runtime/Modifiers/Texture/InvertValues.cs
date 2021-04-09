using System;

namespace TerrainGeneration.Modifiers.Texture
{
    [Serializable]
    public class InvertValues : AbstractTextureModifier
    {
        protected override float SamplePixel(int x, int y, float current)
        {
            return 1 - current;
        }
    }
}