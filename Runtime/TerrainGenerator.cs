using System;
using System.Collections;
using NaughtyAttributes;
using TerrainGeneration.Utils;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace TerrainGeneration
{
    public class TerrainGenerator : MonoBehaviour
    {
        private static readonly int[] HeightmapResolutions = {33, 65, 129, 257, 513, 1025, 2049, 5097};
        private static readonly ProfilerMarker GenerateTerrainProfilerMarker =
            new ProfilerMarker(nameof(GenerateTerrain));

        [SerializeField, Dropdown(nameof(HeightmapResolutions))]
        private int _heightmapResolution;
        [SerializeField, Range(0, 4048)] private int _detailResolution = 512;
        [SerializeField, ValidateInput(nameof(ValidateDetailResolutionPerPatch))]
        private int _detailResolutionPerPatch = 32;
        [SerializeField] private Vector3 _size;

        [ShowAssetPreview, SerializeField] private Texture2D _heightmap;

        [SerializeField] private TerrainGenerationPreset _terrainGenerationPreset;
        [SerializeField] private Material _terrainMaterial;

        public ushort Seed => _seed;
        [SerializeField] private ushort _seed;
        [SerializeField] private bool _randomizeSeed;

        public Terrain LastGeneratedTerrain { get; private set; }

        private bool ValidateDetailResolutionPerPatch(int detailResolutionPerPatch)
        {
            var allowedRange = new Vector2(8, 128);
            return _detailResolution % detailResolutionPerPatch == 0
                   && allowedRange.IsValueWithinXY(detailResolutionPerPatch);
        }

        public void SetSeed(ushort seed)
        {
            _randomizeSeed = false;
            _seed = seed;
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void GenerateTerrain(Action onComplete = null)
        {
            StopAllCoroutines();
            StartCoroutine(GenerateTerrainAsync(onComplete));
        }

        public IEnumerator GenerateTerrainAsync(Action onComplete)
        {
            GenerateTerrainProfilerMarker.Begin(this);
            LastGeneratedTerrain = null;
            if (_randomizeSeed)
            {
                _seed = (ushort) Random.Range(0, ushort.MaxValue);
            }

            _heightmap = _terrainGenerationPreset.GenerateHeightmap(_seed, _heightmapResolution);

            var terrainData = new TerrainData().SetTerrainSize(_size, _heightmapResolution);
            terrainData.SetDetailResolution(_detailResolution, _detailResolutionPerPatch);

            var heightmap = _heightmap.GetPixels();
            var height = new float[_heightmapResolution, _heightmapResolution];
            for (var x = 0; x < _heightmapResolution; x++)
            {
                for (var y = 0; y < _heightmapResolution; y++)
                {
                    var sample = heightmap[y * _heightmapResolution + x];
                    height[y, x] = sample.grayscale;
                }
            }

            terrainData.SetHeights(0, 0, height);

            var terrain = Terrain.activeTerrain;
            if (terrain == null)
            {
                terrain = Terrain.CreateTerrainGameObject(terrainData).GetComponent<Terrain>();
                SceneManager.MoveGameObjectToScene(terrain.gameObject, gameObject.scene);
            }

            foreach (Transform child in terrain.transform)
            {
                child.gameObject.SetActive(false);
            }

            terrain.materialTemplate = _terrainMaterial;
            _terrainGenerationPreset.ApplyTerrainLayers(terrainData);
            SetTerrainData(terrain, terrainData);
            yield return new WaitForEndOfFrame();

            yield return _terrainGenerationPreset.ApplyTrees(terrainData, () => SetTerrainData(terrain, terrainData));

            yield return new WaitForEndOfFrame();

            yield return _terrainGenerationPreset
                .ApplyGenericTerrainModifiers(terrainData, terrain, () => SetTerrainData(terrain, terrainData));

            yield return new WaitForEndOfFrame();

            yield return _terrainGenerationPreset
                .ApplyDetailsTerrainModifiers(terrainData, terrain, () => SetTerrainData(terrain, terrainData));

            yield return new WaitForEndOfFrame();

            terrain.name = $"[{nameof(TerrainGenerator)}] Terrain: {_seed}";
            GenerateTerrainProfilerMarker.End();
            yield return null;

            LastGeneratedTerrain = terrain;
            onComplete?.Invoke();
        }

        private static void SetTerrainData(Terrain terrain, TerrainData terrainData)
        {
            terrain.terrainData = terrainData;
            terrain.GetComponent<TerrainCollider>().terrainData = terrainData;
            terrain.Flush();
        }
    }
}