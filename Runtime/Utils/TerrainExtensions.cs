using UnityEngine;

namespace TerrainGeneration.Utils
{
    public static class TerrainExtensions
    {
        public static float SampleHeightRelative(this Terrain terrain, float x, float z)
        {
            var size = terrain.terrainData.size;
            return terrain.SampleHeight(new Vector3(x * size.x, 0f, z * size.z)) / size.y;
        }
    }
}