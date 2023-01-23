using System;
using System.Collections.Generic;
using UnityEngine;
using Aroma;

public interface IGridObject {
    int GetIndex();
    void SetIndex(int index);
}

namespace CustomTilemap {
    public class CustomGrid<T> where T : IGridObject {
        public float CellSize { get; private set; }
        private List<List<T>> _gridList;
        Func<T> _createObjectCallback;
        Action<T> _returnObjectCallback;
        public int Width { get; private set; }
        public int Height { get; private set; }
        Vector3 _originPosition;

        public CustomGrid(float cellSize, int width, int height, Vector3 originPosition, Func<T> createGridObject, Action<T> returnObject = null) {
            CellSize = cellSize;

            Width = width;
            Height = height;
            _originPosition = originPosition;

            _createObjectCallback = createGridObject;
            _returnObjectCallback = returnObject;

            _gridList = new List<List<T>>(height);
            for (int r = 0; r < height; ++r) {
                _gridList.Add(new List<T>(width));
                for (int c = 0; c < width; ++c) {
                    _gridList[r].Add(_createObjectCallback());
                }
            }
        }

        public void ResetGrid(LevelInfo level, int width, int height, Vector3 originPosition, Action updateVisualCallback) {
            AllocGridList(width, height);

            for (int r = 0; r < height; ++r) {
                for (int c = 0; c < width; ++c) {
                    _gridList[r][c].SetIndex(-1);
                }
            }

            for (int i = 0; i < level.tilesets.Length; ++i) {
                var tileInfo = level.tilesets[i].tileInfo;
                for (int j = 0; j < tileInfo.Length; ++j) {
                    int r = tileInfo[j].row;
                    int c = tileInfo[j].column;
                    _gridList[r][c].SetIndex(tileInfo[j].textureIndex);
                }
            }

            _originPosition = originPosition;
            Width = width;
            Height = height;

            updateVisualCallback();
        }

        /// Only Used In Editor
        public void ResizeGrid(Vector3 originPosition, int currentGridWidth, int currentGridHeight, Vector3 currentOriginPosition, int widthDelta, int heightDelta) {
            int prevWidth = currentGridWidth - widthDelta;
            int prevHeight = currentGridHeight - heightDelta;
            bool wChanged = !(Mathf.Abs(currentOriginPosition.x - originPosition.x) < Mathf.Epsilon);
            bool hChanged = !(Mathf.Abs(currentOriginPosition.y - originPosition.y) < Mathf.Epsilon);

            T[,] gridArray = new T[currentGridWidth, currentGridHeight];
            /*
            for (int x = 0; x < currentGridWidth; ++x) {
                for (int y = 0; y < currentGridHeight; ++y) {
                    gridArray[x, y] = _createObjectCallback();
                }
            }
            
            for (int x = 0; x < prevWidth; ++x) {
                for (int y = 0; y < prevHeight; ++y) {
                    int idx = _gridArray[x, y].GetIndex();
                    _returnObjectCallback?.Invoke(_gridArray[x, y]);
                    if (idx == -1) continue;

                    int alteredX = wChanged ? x + widthDelta : x;
                    int alteredY = hChanged ? y + heightDelta : y;

                    if (alteredX < 0 || alteredY < 0 || alteredX >= LayerModel.CurrentGridWidth || alteredY >= LayerModel.CurrentGridHeight) {
                        continue;
                    }

                    gridArray[alteredX, alteredY].SetIndex(idx);
                }
            }

            _gridArray = gridArray.Clone() as T[,];*/
        }


        void AllocGridList(int width, int height) {
            int widthSize = _gridList[0].Count;
            int heightSize = _gridList.Count;

            if (height > heightSize) {
                for (int i = 0; i < height - heightSize; ++i) {
                    _gridList.Add(new List<T>(width));
                    for (int j = 0; j < width; ++j) {
                        _gridList[heightSize + i].Add(_createObjectCallback());
                    }
                }
            }
            if (width > widthSize) {
                for (int i = 0; i < height; ++i) {
                    for (int j = 0; j < width - widthSize; ++j) {
                        _gridList[i].Add(_createObjectCallback());
                    }
                }
            }
        }

        public List<List<T>> GetGridList() {
            return _gridList;
        }

        public T GetGridObject(int r, int c) {
            if (r >= 0 && c >= 0 && r < Height && c < Width) {
                return _gridList[r][c];
            } 
            else {
                return default(T);
            }
        }

        public Vector3 GetWorldPosition(int r, int c) {
            return GridUtility.GetWorldPosition(r, c, CellSize, _originPosition);
        }
    }
}