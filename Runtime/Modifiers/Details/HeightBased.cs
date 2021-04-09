using System;
using System.Collections;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace TerrainGeneration.Modifiers.Details
{
    [Serializable]
    public class HeightBased : IDetailsModifier
    {
        [SerializeField] private int _spacing = 10;
        [SerializeField] private Texture2D _detailTexture;
        [SerializeField, MinMaxSlider(float.Epsilon, 10f)]
        private Vector2 _heightRange;
        [SerializeField, MinMaxSlider(float.Epsilon, 10f)]
        private Vector2 _widthRange;
        [SerializeField] private Color _dryColor = new Color(0.62f, 0.32f, 0.17f);
        [SerializeField] private Color _healthyColor = Color.green;
        [SerializeField] private float _noiseSpread = 1f;
        [SerializeField, ReadOnly] private DetailRenderMode _renderMode = DetailRenderMode.GrassBillboard;
        [SerializeField] private float _holeEdgePadding;

        public void Setup(ushort seed)
        {
        }

        public IEnumerator Modify(Terrain terrain, TerrainData data, ushort seed)
        {
            EnsureDetailPrototypeAdded(data, out var index);

            int[,] detailMap = new int[data.detailWidth, data.detailHeight];

            for (var y = 0; y < data.detailHeight; y += _spacing)
            {
                for (var x = 0; x < data.detailWidth; x += _spacing)
                {
                    /*int xHM = (int) (x / (float) data.detailWidth * data.heightmapResolution);
                    int yHM = (int) (y / (float) data.detailHeight * data.heightmapResolution);

                    float thisNoise = Utils.Map(Mathf.PerlinNoise(x * details[i].feather,
                        y * details[i].feather), 0, 1, 0.5f, 1);
                    float thisHeightStart = details[i].minHeight * thisNoise -
                                            details[i].overlap * thisNoise;

                    float nextHeightStart = details[i].maxHeight * thisNoise +
                                            details[i].overlap * thisNoise;

                    float thisHeight = heightMap[yHM, xHM];
                    float steepness = data.GetSteepness(xHM / (float) data.size.x,
                        yHM / (float) data.size.z);
                    if ((thisHeight >= thisHeightStart && thisHeight <= nextHeightStart) &&
                        (steepness >= details[i].minSlope && steepness <= details[i].maxSlope))
                    {
                        detailMap[y, x] = 1;
                    }*/

                    detailMap[y, x] = 1;
                }
            }

            data.SetDetailLayer(0, 0, index, detailMap);

            yield return null;
        }

        private void EnsureDetailPrototypeAdded(TerrainData data, out int index)
        {
            if (!data.detailPrototypes.Any(e => e.prototypeTexture))
            {
                var detailPrototypes = data.detailPrototypes.ToList();
                detailPrototypes.Add(new DetailPrototype
                {
                    prototypeTexture = _detailTexture,
                    minHeight = _heightRange.x,
                    maxHeight = _heightRange.y,
                    minWidth = _widthRange.x,
                    maxWidth = _widthRange.y,
                    noiseSpread = _noiseSpread,
                    dryColor = _dryColor,
                    healthyColor = _healthyColor,
                    usePrototypeMesh = false,
                    renderMode = _renderMode,
                    holeEdgePadding = _holeEdgePadding
                });
                data.detailPrototypes = detailPrototypes.ToArray();
            }

            index = data.detailPrototypes.ToList().FindIndex(e => e.prototypeTexture == _detailTexture);
        }

        private void Y()
        {
            // DetailPrototype[] newDetailPrototypes;
            // newDetailPrototypes = new DetailPrototype[details.Count];
            // int dindex = 0;
            // foreach (Detail d in details)
            // {
            //     newDetailPrototypes[dindex] = new DetailPrototype();
            //     newDetailPrototypes[dindex].prototype = d.prototype;
            //     newDetailPrototypes[dindex].prototypeTexture = d.prototypeTexture;
            //     newDetailPrototypes[dindex].healthyColor = d.healthyColour;
            //     newDetailPrototypes[dindex].dryColor = d.dryColour;
            //     newDetailPrototypes[dindex].minHeight = d.heightRange.x;
            //     newDetailPrototypes[dindex].maxHeight = d.heightRange.y;
            //     newDetailPrototypes[dindex].minWidth = d.widthRange.x;
            //     newDetailPrototypes[dindex].maxWidth = d.widthRange.y;
            //     newDetailPrototypes[dindex].noiseSpread = d.noiseSpread;
            //     if (newDetailPrototypes[dindex].prototype)
            //     {
            //         newDetailPrototypes[dindex].usePrototypeMesh = true;
            //         newDetailPrototypes[dindex].renderMode = DetailRenderMode.VertexLit;
            //     }
            //     else
            //     {
            //         newDetailPrototypes[dindex].usePrototypeMesh = false;
            //         newDetailPrototypes[dindex].renderMode = DetailRenderMode.GrassBillboard;
            //     }
            //
            //     dindex++;
            // }
            //
            // data.detailPrototypes = newDetailPrototypes;
            //
            // float[,] heightMap = data.GetHeights(0, 0,
            //     data.heightmapResolution,
            //     data.heightmapResolution);
            //
            // for (int i = 0; i < data.detailPrototypes.Length; i++)
            // {
            //     int[,] detailMap = new int[data.detailWidth, data.detailHeight];
            //
            //     for (int y = 0; y < data.detailHeight; y += _spacing)
            //     {
            //         for (int x = 0; x < data.detailWidth; x += _spacing)
            //         {
            //             if (UnityEngine.Random.Range(0.0f, 1.0f) > details[i].density)
            //             {
            //                 continue;
            //             }
            //
            //             int xHM = (int) (x / (float) data.detailWidth * data.heightmapResolution);
            //             int yHM = (int) (y / (float) data.detailHeight * data.heightmapResolution);
            //
            //             float thisNoise = Utils.Map(Mathf.PerlinNoise(x * details[i].feather,
            //                 y * details[i].feather), 0, 1, 0.5f, 1);
            //             float thisHeightStart = details[i].minHeight * thisNoise -
            //                                     details[i].overlap * thisNoise;
            //
            //             float nextHeightStart = details[i].maxHeight * thisNoise +
            //                                     details[i].overlap * thisNoise;
            //
            //             float thisHeight = heightMap[yHM, xHM];
            //             float steepness = data.GetSteepness(xHM / (float) data.size.x,
            //                 yHM / (float) data.size.z);
            //             if ((thisHeight >= thisHeightStart && thisHeight <= nextHeightStart) &&
            //                 (steepness >= details[i].minSlope && steepness <= details[i].maxSlope))
            //             {
            //                 detailMap[y, x] = 1;
            //             }
            //         }
            //     }
            //
            //     data.SetDetailLayer(0, 0, i, detailMap);
            // }
        }
    }
}