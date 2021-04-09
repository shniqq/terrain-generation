using UnityEngine;

namespace TerrainGeneration.Modifiers.Trees
{
    public interface ITreeModifier
    {
        void ApplyTrees(TerrainData terrainData, ushort seed);
    }
}