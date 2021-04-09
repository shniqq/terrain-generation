using UnityEngine;

namespace TerrainGeneration.Modifiers.Layers
{
    public interface ILayerModifier
    {
        void ApplyLayer(TerrainData terrainData, ushort seed);
    }
}