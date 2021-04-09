using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TerrainGeneration.Modifiers.Generic.GameObjectPlacement
{
    [Serializable]
    public abstract class AbstractPlaceGameObjects : IGenericTerrainModifier
    {
        [SerializeField] private string _identifier;
        protected string Identifier => _identifier;

        private Stack<GameObject> _pool = new Stack<GameObject>();
        private GameObject _poolParent;

        protected GameObject ClearPreviousAndGetParent(Terrain terrain)
        {
            var name = $"[TerrainGenerator][{Identifier}] {nameof(PlaceGameObjectsHeightBased)}";
            var parent = terrain.transform.Find(name)?.gameObject;
            var poolParentName = Identifier + " - Pool";
            if (_poolParent == null)
            {
                _poolParent = GameObject.Find(poolParentName) ?? new GameObject(poolParentName);
                _poolParent.SetActive(false);
            }

            if (parent != null)
            {
                for (var i = parent.transform.childCount - 1; i >= 0; --i)
                {
                    var child = parent.transform.GetChild(i);
                    child.gameObject.SetActive(false);
                    child.SetParent(_poolParent.transform);
                    _pool.Push(child.gameObject);
                }

                Object.DestroyImmediate(parent);
            }

            parent = new GameObject(name);
            parent.transform.SetParent(terrain.transform);
            parent.transform.localPosition = Vector3.zero;
            return parent;
        }

        protected GameObject TryGetFromPool(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            if (_pool.Count > 0 && _pool.Peek() != null)
            {
                var pooledObject = _pool.Pop();
                pooledObject.transform.SetPositionAndRotation(position, rotation);
                pooledObject.transform.SetParent(parent, true);
                pooledObject.SetActive(true);
                return pooledObject;
            }

            return Object.Instantiate(prefab, position, rotation, parent);
        }

        public void Setup(ushort seed)
        {
        }

        public abstract IEnumerator Modify(TerrainData data, Terrain terrain, ushort seed);
    }
}