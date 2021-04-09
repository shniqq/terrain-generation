using System.Collections;
using UnityEngine;

namespace TerrainGeneration.Modifiers.Generic
{
    public interface IGenericTerrainModifier
    {
        void Setup(ushort seed);
        IEnumerator Modify(TerrainData data, Terrain terrain, ushort seed);
    }
}