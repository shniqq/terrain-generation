using System.Collections;
using UnityEngine;

namespace TerrainGeneration.Modifiers.Details
{
    public interface IDetailsModifier
    {
        void Setup(ushort seed);
        IEnumerator Modify(Terrain terrain, TerrainData data, ushort seed);
    }
}