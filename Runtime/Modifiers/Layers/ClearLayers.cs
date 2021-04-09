using System;
using System.Collections;
using UnityEngine;

namespace TerrainGeneration.Modifiers.Layers
{
    [Serializable]
    public class ClearLayers : ILayerModifier
    {
        public void ApplyLayer(TerrainData terrainData, ushort seed)
        {
            terrainData.terrainLayers = new TerrainLayer[0];
            var clearedAlphaMaps = terrainData
                .GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
            clearedAlphaMaps.Initialize();
            terrainData.SetAlphamaps(0, 0, clearedAlphaMaps);
        }
    }
}