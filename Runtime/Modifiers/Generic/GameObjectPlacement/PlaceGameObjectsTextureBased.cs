using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NaughtyAttributes;
using TerrainGeneration.Modifiers.Texture;
using TerrainGeneration.Utils;
using UnityEngine;
using Random = System.Random;

namespace TerrainGeneration.Modifiers.Generic.GameObjectPlacement
{
    [Serializable]
    public class PlaceGameObjectsTextureBased : AbstractPlaceGameObjects
    {
        [Serializable]
        private class GameObjectData
        {
            [SerializeField] public List<GameObject> GameObjects;
            [SerializeField] public bool PickRandomly = true;
            [SerializeField, MinMaxSlider(0, 1)] public Vector2 HeightRange;
            [SerializeField, MinMaxSlider(0, 90)] public Vector2 SlopeRange;
            [SerializeField] public bool AlignWithSlope;
            [SerializeField, MinMaxSlider(0, 1)] public Vector2 ApplicablePixelValueRange;
            [SerializeField, Range(0, 1)] public float PlacementChance = 1f;
            [SerializeField] public bool RotateYRandomly = false;
        }

        [SerializeField] private GameObjectData _gameObjectData;

        [SerializeReference, SerializeReferenceButton, AllowNesting]
        private ITextureModifier _noiseTextureModifier;

        public override IEnumerator Modify(TerrainData data, Terrain terrain, ushort seed)
        {
            var parent = ClearPreviousAndGetParent(terrain).transform;

            var random = new Random(seed);
            var terrainPosition = terrain.transform.position;
            var objectPickingIndex = 0;

            var textureSize = Mathf.RoundToInt(data.heightmapResolution);
            _noiseTextureModifier.Setup(seed, textureSize);
            var texture = new Texture2D(textureSize, textureSize);
            _noiseTextureModifier.Modify(texture);

            var gameObjectCreationParams = new List<GameObjectCreationParameters>();

            texture.ForEachPixelRelative((x, z, color) =>
            {
                var slope = data.GetSteepness(x, z);
                var heightRelative = terrain.SampleHeightRelative(x, z);

                var applicable = _gameObjectData.ApplicablePixelValueRange.IsValueWithinXY(color.grayscale)
                                 && _gameObjectData.SlopeRange.IsValueWithinXY(slope)
                                 && _gameObjectData.HeightRange.IsValueWithinXY(heightRelative);
                if (applicable && random.NextDouble() <= _gameObjectData.PlacementChance)
                {
                    var gameObject = _gameObjectData.PickRandomly
                        ? _gameObjectData.GameObjects.ElementAt(random.Next(_gameObjectData.GameObjects.Count))
                        : _gameObjectData.GameObjects.ElementAt(objectPickingIndex);
                    objectPickingIndex =
                        (int) Mathf.Repeat(objectPickingIndex++, _gameObjectData.GameObjects.Count - 1);

                    var normal = data.GetInterpolatedNormal(x, z);
                    var rotation = _gameObjectData.AlignWithSlope
                        ? Quaternion.FromToRotation(Vector3.up, normal)
                        : Quaternion.identity;
                    if (_gameObjectData.RotateYRandomly)
                    {
                        rotation *= Quaternion.Euler(0, random.NextFloat() * 180f, 0);
                    }

                    var position = new Vector3(
                        x * data.size.x,
                        heightRelative * data.size.y,
                        z * data.size.z);

                    var creationParameters = new GameObjectCreationParameters(
                        gameObject,
                        terrainPosition + position,
                        rotation,
                        parent);
                    gameObjectCreationParams.Add(creationParameters);
                }
            });

            var stopWatch = Stopwatch.StartNew();
            foreach (var gameObjectCreationParameters in gameObjectCreationParams)
            {
                if (stopWatch.Elapsed.TotalSeconds >= 1 / 30f)
                {
                    yield return null;
                    stopWatch.Restart();
                }
                CreateGameObject(gameObjectCreationParameters);
            }
        }

        private struct GameObjectCreationParameters
        {
            public GameObject Prefab { get; }
            public Vector3 Position { get; }
            public Quaternion Rotation { get; }
            public Transform Parent { get; }

            public GameObjectCreationParameters(
                GameObject prefab,
                Vector3 position,
                Quaternion rotation,
                Transform parent)
            {
                Prefab = prefab;
                Position = position;
                Rotation = rotation;
                Parent = parent;
            }
        }

        private void CreateGameObject(GameObjectCreationParameters parameters)
        {
            TryGetFromPool(parameters.Prefab, parameters.Position, parameters.Rotation, parameters.Parent);
        }
    }
}