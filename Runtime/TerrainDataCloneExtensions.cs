using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace TerrainGeneration
{
    /// <summary>
    /// Provides means to deep-copy a TerrainData object because Unity's built-in "Instantiate" method
    /// will miss some things and the resulting copy still shares data with the original.
    /// </summary>
    /// From: https://gist.github.com/Eldoir/d5a438dfedee55552915b55097dda1d4
    public static class TerrainDataCloneExtensions
    {
        /// <summary>
        /// Creates a real deep-copy of a TerrainData
        /// </summary>
        /// <param name="original">TerrainData to duplicate</param>
        /// <returns>New terrain data instance</returns>
        public static TerrainData Clone(this TerrainData original)
        {
            var duplicate = new TerrainData
            {
                alphamapResolution = original.alphamapResolution,
                baseMapResolution = original.baseMapResolution,
                detailPrototypes = CloneDetailPrototypes(original.detailPrototypes)
            };
            
            duplicate.SetDetailResolution(original.detailResolution, original.detailResolutionPerPatch);

            duplicate.heightmapResolution = original.heightmapResolution;
            duplicate.size = original.size;

            duplicate.terrainLayers = CloneTerrainLayers(original.terrainLayers);

            duplicate.wavingGrassAmount = original.wavingGrassAmount;
            duplicate.wavingGrassSpeed = original.wavingGrassSpeed;
            duplicate.wavingGrassStrength = original.wavingGrassStrength;
            duplicate.wavingGrassTint = original.wavingGrassTint;

            duplicate.SetAlphamaps(0, 0, original.GetAlphamaps(0, 0, original.alphamapWidth, original.alphamapHeight));
            var heights = original.GetHeights(0, 0, original.heightmapResolution, original.heightmapResolution);
            duplicate.SetHeights(0, 0, heights);

            for (var n = 0; n < original.detailPrototypes.Length; n++)
            {
                duplicate.SetDetailLayer(0, 0, n,
                    original.GetDetailLayer(0, 0, original.detailWidth, original.detailHeight, n));
            }

            duplicate.treePrototypes = CloneTreePrototypes(original.treePrototypes);
            duplicate.treeInstances = CloneTreeInstances(original.treeInstances);

            //AssertPublicInstancePropertiesEqual(duplicate, original);

            return duplicate;
        }

        /// <summary>
        /// Deep-copies an array of terrain layers
        /// </summary>
        /// <param name="original">Prototypes to clone</param>
        /// <returns>Cloned array</returns>
        private static TerrainLayer[] CloneTerrainLayers(TerrainLayer[] original)
        {
            var terrainLayers = new TerrainLayer[original.Length];

            for (var n = 0; n < terrainLayers.Length; n++)
            {
                var newTerrainLayer = new TerrainLayer
                {
                    name = original[n].name,
                    metallic = original[n].metallic,
                    normalMapTexture = original[n].normalMapTexture,
                    normalScale = original[n].normalScale,
                    smoothness = original[n].smoothness,
                    specular = original[n].specular,
                    diffuseTexture = original[n].diffuseTexture,
                    tileOffset = original[n].tileOffset,
                    tileSize = original[n].tileSize,
                    maskMapRemapMin = original[n].maskMapRemapMin,
                    maskMapRemapMax = original[n].maskMapRemapMax,
                    maskMapTexture = original[n].maskMapTexture,
                    hideFlags = original[n].hideFlags,
                    diffuseRemapMax = original[n].diffuseRemapMax,
                    diffuseRemapMin = original[n].diffuseRemapMin
                };

                AssertPublicInstancePropertiesEqual(newTerrainLayer, original[n]);

                terrainLayers[n] = newTerrainLayer;
            }

            return terrainLayers;
        }

        /// <summary>
        /// Deep-copies an array of detail prototype instances
        /// </summary>
        /// <param name="original">Prototypes to clone</param>
        /// <returns>Cloned array</returns>
        private static DetailPrototype[] CloneDetailPrototypes(DetailPrototype[] original)
        {
            var protoDuplicate = new DetailPrototype[original.Length];

            for (var n = 0; n < original.Length; n++)
            {
                var newDetailPrototype = new DetailPrototype
                {
                    dryColor = original[n].dryColor,
                    healthyColor = original[n].healthyColor,
                    maxHeight = original[n].maxHeight,
                    maxWidth = original[n].maxWidth,
                    minHeight = original[n].minHeight,
                    minWidth = original[n].minWidth,
                    noiseSpread = original[n].noiseSpread,
                    prototype = original[n].prototype,
                    prototypeTexture = original[n].prototypeTexture,
                    renderMode = original[n].renderMode,
                    usePrototypeMesh = original[n].usePrototypeMesh,
                    holeEdgePadding = original[n].holeEdgePadding
                };

#pragma warning disable 618
                const string bendFactorName = nameof(newDetailPrototype.bendFactor);
#pragma warning restore 618

                AssertPublicInstancePropertiesEqual(newDetailPrototype, original[n], bendFactorName);

                protoDuplicate[n] = newDetailPrototype;
            }

            return protoDuplicate;
        }

        /// <summary>
        /// Deep-copies an array of tree prototype instances
        /// </summary>
        /// <param name="original">Prototypes to clone</param>
        /// <returns>Cloned array</returns>
        private static TreePrototype[] CloneTreePrototypes(TreePrototype[] original)
        {
            var clones = new TreePrototype[original.Length];

            for (var n = 0; n < original.Length; n++)
            {
                clones[n] = new TreePrototype
                {
                    bendFactor = original[n].bendFactor,
                    prefab = original[n].prefab,
                    navMeshLod = original[n].navMeshLod
                };
            }

            return clones;
        }

        /// <summary>
        /// Deep-copies an array of tree instances
        /// </summary>
        /// <param name="original">Trees to clone</param>
        /// <returns>Cloned array</returns>
        private static TreeInstance[] CloneTreeInstances(TreeInstance[] original)
        {
            var clones = new TreeInstance[original.Length];

            System.Array.Copy(original, clones, original.Length);

            return clones;
        }

        private static void AssertPublicInstancePropertiesEqual<T>(T self, T to, params string[] ignore) where T : class
        {
            if (self == null || to == null)
            {
                Assert.AreEqual(self, to);
            }

            var type = typeof(T);
            var ignoreDList = new List<string>(ignore);
            foreach (var pi in type.GetProperties(System.Reflection.BindingFlags.Public |
                                                  System.Reflection.BindingFlags.Instance))
            {
                if (ignoreDList.Contains(pi.Name))
                {
                    continue;
                }

                var selfValue = type.GetProperty(pi.Name)?.GetValue(self, null);
                var toValue = type.GetProperty(pi.Name)?.GetValue(to, null);

                Assert.AreEqual(selfValue, toValue);
            }
        }
    }
}