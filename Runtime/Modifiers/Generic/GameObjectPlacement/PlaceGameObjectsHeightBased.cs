using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using TerrainGeneration.Utils;
using UnityEngine;
using Random = System.Random;

namespace TerrainGeneration.Modifiers.Generic.GameObjectPlacement
{
    [Serializable]
    public class PlaceGameObjectsHeightBased : AbstractPlaceGameObjects
    {
        [Serializable]
        private class GameObjectData
        {
            [SerializeField] public List<GameObject> GameObjects;
            [SerializeField] public bool PickRandomly = true;
            [SerializeField, MinMaxSlider(0, 1)] public Vector2 HeightRange;
            [SerializeField, MinMaxSlider(0, 90)] public Vector2 SlopeRange;
            [SerializeField] public bool AlignWithSlope;
            [SerializeField] public bool EnforceGroundLevel = false;
            [SerializeReference, SerializeReferenceButton, AllowNesting]
            public IFloatValueProvider SpacingProvider;
        }

        [SerializeField] private GameObjectData _gameObjectData;

        public override IEnumerator Modify(TerrainData data, Terrain terrain, ushort seed)
        {
            var parent = ClearPreviousAndGetParent(terrain);
            _gameObjectData.SpacingProvider.Setup(seed);

            var random = new Random(seed);
            var terrainPosition = terrain.transform.position;
            var objectPickingIndex = 0;

            var terrainCollider = terrain.GetComponent<TerrainCollider>();

            data.SampleForEachPoint(_gameObjectData.SpacingProvider.GetValue, _gameObjectData.SpacingProvider.GetValue,
                pointInformation =>
                {
                    if (_gameObjectData.HeightRange.IsValueWithinXY(pointInformation.RelativeHeight)
                        && _gameObjectData.SlopeRange.IsValueWithinXY(pointInformation.Slope))
                    {
                        var gameObject = _gameObjectData.PickRandomly
                            ? _gameObjectData.GameObjects.ElementAt(random.Next(_gameObjectData.GameObjects.Count))
                            : _gameObjectData.GameObjects.ElementAt(objectPickingIndex);
                        objectPickingIndex =
                            (int) Mathf.Repeat(objectPickingIndex++, _gameObjectData.GameObjects.Count - 1);
                        var rotation = _gameObjectData.AlignWithSlope
                            ? Quaternion.FromToRotation(Vector3.up, pointInformation.Normal)
                            : Quaternion.identity;
                        var position = terrainPosition + pointInformation.Position;
                        if (_gameObjectData.EnforceGroundLevel)
                        {
                            if (terrainCollider.Raycast(new Ray(position, pointInformation.Normal),
                                out var hitInfo, 10f))
                            {
                                position = hitInfo.point;
                            }
                        }

                        TryGetFromPool(gameObject, position, rotation, parent.transform);
                    }
                });

            yield return null;
        }
    }
}