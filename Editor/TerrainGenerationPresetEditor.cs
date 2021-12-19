using System.Collections.Generic;
using EditorGuiUtils.Editor;
using UnityEditor;

namespace TerrainGeneration.Editor
{
    [CustomEditor(typeof(TerrainGenerationPreset))]
    public class TerrainGenerationPresetEditor : UnityEditor.Editor
    {
        private TerrainGenerationPreset Preset => target as TerrainGenerationPreset;

        private TabsBlock _tabs;

        protected override bool ShouldHideOpenButton()
        {
            return true;
        }

        private void OnEnable()
        {
            _tabs = new TabsBlock(new Dictionary<string, System.Action>()
            {
                {"Heightmap Texture", TextureModifiers},
                {"Terrain Layers", TerrainLayers},
                {"Terrain Trees", TerrainTrees},
                {"Terrain Details", TerrainDetails},
                {"Generic Modifiers", GenericModifiers}
            });
            _tabs.SetCurrentMethod(0);
        }

        public override void OnInspectorGUI()
        {
            Undo.RecordObject(target, "Terrain Generation Modification");

            if (_tabs is null)
            {
                OnEnable();
            }

            _tabs.Draw();
            serializedObject.ApplyModifiedProperties();
        }

        private void TextureModifiers()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Preset._textureModifiers)));
        }

        private void TerrainLayers()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Preset._terrainLayersModifiers)));
        }

        private void TerrainTrees()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Preset._terrainTreeModifiers)));
        }

        private void TerrainDetails()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Preset._detailsModifiers)));
        }

        private void GenericModifiers()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Preset._genericTerrainModifiers)));
        }
    }
}