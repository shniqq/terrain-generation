using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace TerrainGeneration.Modifiers.Generic
{
    [Serializable]
    public class GenericModifierCollection : IGenericTerrainModifier
    {
        [SerializeReference, SerializeReferenceButton, AllowNesting]
        private List<IGenericTerrainModifier> _genericTerrainModifiers;

        public void Setup(ushort seed)
        {
            _genericTerrainModifiers.ForEach(e => e.Setup(seed));
        }

        public IEnumerator Modify(TerrainData data, Terrain terrain, ushort seed)
        {
            foreach (var modifier in _genericTerrainModifiers)
            {
                yield return modifier?.Modify(data, terrain, seed);
            }
        }
    }
}