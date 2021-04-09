using System;
using System.Linq;
using NaughtyAttributes;
using TerrainGeneration.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TerrainGeneration.Modifiers.Trees
{
    [Serializable]
    public class HeightBased : ITreeModifier
    {
        [SerializeField] private GameObject _tree;
        [SerializeField] private float _bendFactor = 1f;
        [SerializeField, MinMaxSlider(0, 1)] private Vector2 _heightRange;
        [SerializeField, Range(1, 10)] private float _scale = 1f;
        [SerializeField] private int _treeSpacing;

        public void ApplyTrees(TerrainData terrainData, ushort seed)
        {
            var existingPrototypes = terrainData.treePrototypes;
            var treePrototypes = existingPrototypes.ToList();
            var treePrototype = treePrototypes.FirstOrDefault(e => e.prefab == _tree);

            if (treePrototype == null)
            {
                treePrototype = new TreePrototype
                {
                    prefab = _tree,
                    bendFactor = _bendFactor
                };
                treePrototypes.Add(treePrototype);
                terrainData.treePrototypes = treePrototypes.ToArray();
            }

            var treePrototypeIndex = treePrototypes.IndexOf(treePrototype);

            var existingTrees = terrainData.treeInstances.ToList();

            for (var x = 0; x < terrainData.size.x; x += _treeSpacing)
            {
                for (var z = 0; z < terrainData.size.z; z += _treeSpacing)
                {
                    var terrainHeight = terrainData.GetHeightRelative(z, x);
                    var isInHeightRange = terrainHeight >= _heightRange.x &&
                                          terrainHeight <= _heightRange.y;

                    if (isInHeightRange)
                    {
                        var xPosition = x / (float) terrainData.heightmapResolution;
                        var zPosition = z / (float) terrainData.heightmapResolution;
                        var newTreeInstance = new TreeInstance
                        {
                            position = new Vector3(xPosition, 0f, zPosition),
                            heightScale = _scale,
                            widthScale = _scale,
                            color = Color.white,
                            rotation = Random.Range(0f, Mathf.PI * 2f),
                            lightmapColor = Color.white,
                            prototypeIndex = treePrototypeIndex
                        };
                        existingTrees.Add(newTreeInstance);
                    }
                }
            }

            terrainData.SetTreeInstances(existingTrees.ToArray(), true);
        }
    }
}