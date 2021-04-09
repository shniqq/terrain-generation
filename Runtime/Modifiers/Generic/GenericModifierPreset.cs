using System.Collections;
using NaughtyAttributes;
using TerrainGeneration.Utils;
using UnityEngine;

namespace TerrainGeneration.Modifiers.Generic
{
    [CreateAssetMenu(menuName = Constants.MenuPrefix + nameof(GenericModifierPreset))]
    public class GenericModifierPreset : ScriptableObject, IGenericTerrainModifier
    {
        [SerializeReference, SerializeReferenceButton, AllowNesting]
        private IGenericTerrainModifier _genericTerrainModifier;

        public void Setup(ushort seed)
        {
            _genericTerrainModifier.Setup(seed);
        }

        public IEnumerator Modify(TerrainData data, Terrain terrain, ushort seed)
        {
            yield return _genericTerrainModifier.Modify(data, terrain, seed);
        }
    }
}