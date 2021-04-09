using System;
using NaughtyAttributes;
using TerrainGeneration.Utils;
using UnityEngine;

namespace TerrainGeneration.Modifiers.Layers
{
    [Serializable]
    public class ElevationBased : AbstractLayerModifier
    {
        [Serializable]
        private class TerrainLayerSettings
        {
            [MinMaxSlider(0, 1)] public Vector2 HeightRange = new Vector2(0, 1);
            [MinMaxSlider(0, 90)] public Vector2 SteepnessRange = new Vector2(0, 90);

            public bool IsApplicable(float height, float steepness)
            {
                var isInSteepnessRange = steepness >= SteepnessRange.x &&
                                         steepness <= SteepnessRange.y;
                var isInHeightRange = height >= HeightRange.x &&
                                      height <= HeightRange.y;
                var isApplicable = isInSteepnessRange && isInHeightRange;
                return isApplicable;
            }
        }

        [SerializeField, AllowNesting] private TerrainLayerSettings _terrainLayerSettings;

        protected override float DetermineLayerAlpha(TerrainData terrainData, int x, int y)
        {
            var alpha = 1f;

            var heightRelative = terrainData.GetHeightRelative(x, y);
            var steepness = terrainData.GetSteepnessAtPoint(x, y);

            var canApplyLayer = _terrainLayerSettings.IsApplicable(heightRelative, steepness);
            if (!canApplyLayer)
            {
                alpha = 0f;
            }

            return alpha;
        }
    }
}