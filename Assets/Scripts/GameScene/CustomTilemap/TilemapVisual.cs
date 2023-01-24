using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomTilemap;

namespace CustomTilemap {
    public class TilemapVisual : MonoBehaviour {
        protected struct UVCoords {
            public Vector2 uv00;
            public Vector2 uv11;
        }

        [SerializeField] protected float _tileSize = 16f;
        protected Mesh _mesh;
        protected Dictionary<int, UVCoords> _uvCoordsDictionary;
        MeshRenderer _meshRenderer;
        CustomGrid<TileObject> _grid;
        public void Initalize(CustomGrid<TileObject> grid, Material material) {
            _meshRenderer = GetComponent<MeshRenderer>();
            _grid = grid;

            _mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = _mesh;

            _meshRenderer.material = material;
            Texture texture = material.mainTexture;
            float textureWidth = texture.width;
            float textureHeight = texture.height;

            _uvCoordsDictionary = new Dictionary<int, UVCoords>();
            
            int xCount = Mathf.FloorToInt(textureWidth / _tileSize);
            int yCount = Mathf.FloorToInt(textureHeight / _tileSize);
            
            InitUVCoordsDictionary(xCount, yCount);
        }

        public void ChangeMaterial(Material material) {
            if (material.Equals(_meshRenderer.material)) return;

            _meshRenderer.material = material;
            Texture texture = material.mainTexture;
            float textureWidth = texture.width;
            float textureHeight = texture.height;

            int xCount = Mathf.FloorToInt(textureWidth / _tileSize);
            int yCount = Mathf.FloorToInt(textureHeight / _tileSize);
            if (xCount * yCount != _uvCoordsDictionary.Count) {
                InitUVCoordsDictionary(xCount, yCount);
            }
        }

        void InitUVCoordsDictionary(int xCount, int yCount) {
            int index = 0;
            float paddingX = 1f / xCount;
            float paddingY = 1f / yCount;

            _uvCoordsDictionary.Clear();
            for (int i = 0; i < yCount; ++i) {
                for (int j = 0; j < xCount; ++j) {
                    float x = paddingX * j;
                    float y = paddingY * (yCount - i - 1);

                    _uvCoordsDictionary[index++] = new UVCoords {
                        uv00 = new Vector2(x, y),
                        uv11 = new Vector2(x + paddingX, y + paddingY),
                    };
                }
            }
        }

        public void UpdateHeatMapVisual() {
            _mesh.Clear();
            MeshUtils.CreateEmptyMeshArrays(_grid.Width * _grid.Height, out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

            for (int c = 0; c < _grid.Width; ++c) {
                for (int r = 0; r < _grid.Height; ++r) {
                    int index = c * _grid.Height + r;
                    Vector3 quadSize = new Vector3(1, 1) * _grid.CellSize;

                    TileObject gridObject = _grid.GetGridObject(r, c);
                    int textureIndex = gridObject.GetIndex();
                    Vector2 gridUV00, gridUV11;
                    if (textureIndex == -1) {
                        gridUV00 = Vector2.zero;
                        gridUV11 = Vector2.zero;
                        quadSize = Vector3.zero;
                    } else {
                        UVCoords uvCoords = _uvCoordsDictionary[textureIndex];
                        gridUV00 = uvCoords.uv00;
                        gridUV11 = uvCoords.uv11;
                    }

                    MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, _grid.GetWorldPosition(r, c) + quadSize * .5f, 0f, quadSize, gridUV00, gridUV11);
                }
            }

            _mesh.vertices = vertices;
            _mesh.uv = uv;
            _mesh.triangles = triangles;
        }
    }
}