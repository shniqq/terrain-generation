using System;
using System.Collections;
using UnityEngine;

namespace TerrainGeneration.Modifiers.Generic
{
    [Serializable]
    public class FromGenericModifierPreset : IGenericTerrainModifier
    {
        [SerializeField] private GenericModifierPreset _genericModifierPreset;

        public void Setup(ushort seed)
        {
        }

        public IEnumerator Modify(TerrainData data, Terrain terrain, ushort seed)
        {
            yield return _genericModifierPreset.Modify(data, terrain, seed);
        }
    }
}