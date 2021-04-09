using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace TerrainGeneration.Modifiers.Layers
{
    [Serializable]
    public abstract class AbstractLayerModifier : ILayerModifier
    {
        [SerializeField] private TerrainLayer _terrainLayer;

        protected ushort Seed { get; private set; }

        public virtual void ApplyLayer(TerrainData terrainData, ushort seed)
        {
            Assert.IsNotNull(_terrainLayer);
            Seed = seed;

            EnsureLayerExists(terrainData, _terrainLayer, out var newAlphaMapLayerCount);

            var existingAlphaMaps = terrainData.GetAlphamaps(0, 0,
                terrainData.alphamapWidth,
                terrainData.alphamapHeight);

            var newAlphaMaps = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, newAlphaMapLayerCount];

            for (var y = 0; y < terrainData.alphamapHeight; y++)
            {
                for (var x = 0; x < terrainData.alphamapWidth; x++)
                {
                    var alpha = DetermineLayerAlpha(terrainData, x, y);

                    NormalizeAlphaAcrossLayers(existingAlphaMaps, newAlphaMaps, x, y,
                        alpha, newAlphaMapLayerCount);
                }
            }
            
            terrainData.SetAlphamaps(0, 0, newAlphaMaps);
        }

        protected abstract float DetermineLayerAlpha(TerrainData terrainData, int x, int y);

        protected void EnsureLayerExists(TerrainData terrainData, TerrainLayer layer, out int newAlphaMapLayerCount)
        {
            newAlphaMapLayerCount = terrainData.alphamapLayers + 1;

            var terrainLayers = terrainData.terrainLayers;
            var hasLayer = terrainLayers.Contains(layer);
            if (!hasLayer)
            {
                var newTerrainLayers = new TerrainLayer[terrainData.terrainLayers.Length + 1];
                Array.Copy(terrainData.terrainLayers, newTerrainLayers, terrainData.terrainLayers.Length);
                newTerrainLayers[newTerrainLayers.Length - 1] = layer;
                terrainData.terrainLayers = newTerrainLayers;
            }
        }

        protected static void NormalizeAlphaAcrossLayers(float[,,] existingAlphaMaps,
            float[,,] alphaMaps,
            int x,
            int y,
            float alpha,
            int alphaMapsLayerCount)
        {
            existingAlphaMaps[x, y, alphaMapsLayerCount - 1] = alpha;
            var totalAlpha = 0f;
            for (var i = 0; i < alphaMapsLayerCount; i++)
            {
                totalAlpha += existingAlphaMaps[x, y, i];
            }

            for (var i = 0; i < alphaMapsLayerCount; i++)
            {
                alphaMaps[x, y, i] = existingAlphaMaps[x, y, i] / totalAlpha;
            }
        }
    }
}