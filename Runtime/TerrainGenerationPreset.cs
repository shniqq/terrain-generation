using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using TerrainGeneration.Modifiers.Details;
using TerrainGeneration.Modifiers.Generic;
using TerrainGeneration.Modifiers.Layers;
using TerrainGeneration.Modifiers.Texture;
using TerrainGeneration.Modifiers.Trees;
using TerrainGeneration.Utils;
using Unity.Profiling;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TerrainGeneration
{
    [CreateAssetMenu(menuName = Constants.MenuPrefix + nameof(TerrainGenerationPreset))]
    public class TerrainGenerationPreset : ScriptableObject
    {
        private static readonly ProfilerMarker TextureModifiersProfilerMarker =
            new ProfilerMarker(nameof(GenerateHeightmap));
        private static readonly ProfilerMarker LayersModifiersProfilerMarker =
            new ProfilerMarker(nameof(ApplyTerrainLayers));
        private static readonly ProfilerMarker TreeModifiersProfilerMarker =
            new ProfilerMarker(nameof(ApplyTrees));
        private static readonly ProfilerMarker GenericModifiersProfilerMarker =
            new ProfilerMarker(nameof(ApplyGenericTerrainModifiers));
        private static readonly ProfilerMarker DetailsModifiersProfilerMarker =
            new ProfilerMarker(nameof(ApplyDetailsTerrainModifiers));

        [SerializeReference, SerializeReferenceButton, AllowNesting]
        internal List<ITextureModifier> _textureModifiers;
        [SerializeReference, SerializeReferenceButton, AllowNesting]
        internal List<ILayerModifier> _terrainLayersModifiers;
        [SerializeReference, SerializeReferenceButton, AllowNesting]
        internal List<ITreeModifier> _terrainTreeModifiers;
        [SerializeReference, SerializeReferenceButton, AllowNesting]
        internal List<IDetailsModifier> _detailsModifiers;
        [SerializeReference, SerializeReferenceButton, AllowNesting]
        internal List<IGenericTerrainModifier> _genericTerrainModifiers;

        [SerializeField] private ushort _seed;
        [SerializeField] private bool _randomizeSeed;

        [ShowAssetPreview, SerializeField, ShowIf(nameof(HasPreview))]
        internal Texture2D _preview;

        internal bool HasPreview => _preview != null;

        public Texture2D GenerateHeightmap(ushort seed, int size)
        {
            TextureModifiersProfilerMarker.Begin();
            var combinedPreviews = new float[size * size];
            combinedPreviews.Initialize();
            _seed = seed;

            var heightmap = new Texture2D(size, size);

            _textureModifiers.ForEach(modifier => modifier?.Setup(_seed, size));

            foreach (var modifier in _textureModifiers.Where(e => e != null && e.Enabled))
            {
                modifier.Modify(heightmap);
            }

            _preview = heightmap;

            TextureModifiersProfilerMarker.End();
            return heightmap;
        }

        public void ApplyTerrainLayers(TerrainData terrainData)
        {
            LayersModifiersProfilerMarker.Begin();
            foreach (var terrainLayersModifier in _terrainLayersModifiers)
            {
                terrainLayersModifier.ApplyLayer(terrainData, _seed);
            }

            LayersModifiersProfilerMarker.End();
        }

        public IEnumerator ApplyTrees(TerrainData terrainData, Action stepComplete)
        {
            TreeModifiersProfilerMarker.Begin();
            terrainData.treeInstances = new TreeInstance[0];
            foreach (var modifier in _terrainTreeModifiers)
            {
                modifier?.ApplyTrees(terrainData, _seed);
                stepComplete.Invoke();
                yield return null;
            }

            TreeModifiersProfilerMarker.End();
        }

        public IEnumerator ApplyGenericTerrainModifiers(TerrainData terrainData, Terrain terrain, Action stepComplete)
        {
            GenericModifiersProfilerMarker.Begin();
            foreach (var modifier in _genericTerrainModifiers)
            {
                yield return modifier?.Modify(terrainData, terrain, _seed);
                stepComplete.Invoke();
            }

            GenericModifiersProfilerMarker.End();
        }

        public IEnumerator ApplyDetailsTerrainModifiers(TerrainData terrainData, Terrain terrain, Action stepComplete)
        {
            DetailsModifiersProfilerMarker.Begin();
            foreach (var modifier in _detailsModifiers)
            {
                yield return modifier?.Modify(terrain, terrainData, _seed);
                stepComplete.Invoke();
            }

            DetailsModifiersProfilerMarker.End();
        }

        [Button(enabledMode: EButtonEnableMode.Editor)]
        public void GenerateHeightmapPreview()
        {
            var seed = _randomizeSeed ? (ushort) Random.Range(0, ushort.MaxValue) : _seed;
            GenerateHeightmap(seed, 513);
        }
    }
}