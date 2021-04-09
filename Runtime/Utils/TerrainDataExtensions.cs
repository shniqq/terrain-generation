using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace TerrainGeneration.Utils
{
    public static class TerrainDataExtensions
    {
        public static float GetSteepnessAtPoint(this TerrainData terrainData, int x, int y)
        {
            return terrainData.GetSteepness(
                x / (float) terrainData.alphamapWidth,
                y / (float) terrainData.alphamapHeight);
        }

        public static float GetHeightRelative(this TerrainData terrainData, int x, int y)
        {
            var maxHeight = terrainData.heightmapScale.y;
            return terrainData.GetHeight(y, x) / maxHeight;
        }

        public static float GetHeightRelative(this TerrainData terrainData, float x, float y)
        {
            return terrainData.GetHeightRelative(
                Mathf.RoundToInt(terrainData.size.x * x),
                Mathf.RoundToInt(terrainData.size.z * y));
        }

        public static float GetInterpolatedHeightRelative(this TerrainData terrainData, float x, float y)
        {
            Assert.IsTrue(x >= 0 && x <= 1);
            Assert.IsTrue(y >= 0 && y <= 1);

            return terrainData.GetInterpolatedHeight(x, y) / terrainData.size.y;
        }

        public static TerrainData SetTerrainSize(this TerrainData terrainData, Vector3 size, int heightmapScale)
        {
            //Order is important!
            terrainData.heightmapResolution = heightmapScale;
            terrainData.size = size;
            return terrainData;
        }

        public readonly struct SamplePointInformation
        {
            public Vector3 Position { get; }
            public float RelativeHeight { get; }
            public float Slope { get; }
            public Vector3 Normal { get; }

            public SamplePointInformation(Vector3 position, float relativeHeight, float slope, Vector3 normal)
            {
                Position = position;
                RelativeHeight = relativeHeight;
                Slope = slope;
                Normal = normal;
            }
        }

        public static void SampleForEachPoint(this TerrainData terrainData,
            Func<float> xSamplingRateProvider,
            Func<float> zSamplingRateProvider,
            Action<SamplePointInformation> forEachPointAction)
        {
            var x = 0f;
            while (x < terrainData.size.x)
            {
                var z = 0f;
                while (z < terrainData.size.z)
                {
                    var xPosition = x * terrainData.heightmapResolution / terrainData.size.x;
                    var zPosition = z * terrainData.heightmapResolution / terrainData.size.z;
                    var xRelative = x / terrainData.size.x;
                    var zRelative = z / terrainData.size.z;

                    var heightRelative =
                        terrainData.GetInterpolatedHeightRelative(xRelative, zRelative);
                    var steepness = terrainData.GetSteepness(xRelative, zRelative);
                    var normal = terrainData.GetInterpolatedNormal(xRelative, zRelative);

                    var samplePointInformation = new SamplePointInformation(
                        new Vector3(x, heightRelative * terrainData.size.y, z),
                        heightRelative,
                        steepness,
                        normal);

                    forEachPointAction(samplePointInformation);

                    var zSamplingRate = zSamplingRateProvider();
                    if (zSamplingRate <= 0)
                    {
                        Debug.LogError($"{nameof(zSamplingRate)} must be bigger than 0!");
                        return;
                    }

                    z += zSamplingRate;
                }

                var xSamplingRate = xSamplingRateProvider();
                if (xSamplingRate <= 0)
                {
                    Debug.LogError($"{nameof(xSamplingRate)} must be bigger than 0!");
                    return;
                }

                x += xSamplingRate;
            }
        }

        public static void SampleForEachPoint(this TerrainData terrainData, float xSamplingRate, float zSamplingRate,
            Action<SamplePointInformation> forEachPointAction)
        {
            SampleForEachPoint(terrainData, () => xSamplingRate, () => zSamplingRate, forEachPointAction);
        }
    }
}