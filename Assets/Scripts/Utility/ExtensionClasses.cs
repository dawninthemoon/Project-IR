using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Aroma {
    public static class VectorUtility {
        public static Vector3 GetScaleVec(float dir) {
            Vector3 scale = new Vector3(dir, 1f, 1f);
            return scale;
        }
        public static Vector2 ChangeXPos(this Vector2 vec, float x) {
            Vector2 newVec = new Vector2(x, vec.y);
            return newVec;
        }

        public static Vector3 ChangeXPos(this Vector3 vec, float x) {
            Vector3 newVec = new Vector3(x, vec.y, vec.z);
            return newVec;
        }
    }

    public static class GameObjectExtensions {
        private static List<Component> _componentCache = new List<Component>();
        public static T GetComponentNoAlloc<T>(this GameObject obj) where T : Component {
            obj.GetComponents(typeof(T), _componentCache);
            var component = _componentCache.Count > 0 ? _componentCache[0] : null;
            _componentCache.Clear();
            return component as T;
        }
    }

    public static class CustomMath {
        public static float Floor(float value, int digits) {
            float c = Mathf.Pow(10f, digits);
            return Mathf.Floor(value * c) / c;
        }
    }

    public static class EditorUtils {
        public static Vector3 GetMousePosition() {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return worldPosition;
        }
        public static void SortRectanglePoints(ref Vector2 p00, ref Vector2 p10, ref Vector2 p11, ref Vector2 p01) {
            Vector2[] arr = new Vector2[4] { p00, p10, p11, p01 };

            Vector2[] check = new Vector2[4];
            for (int i = 0; i < 4; ++i) {
                check[i] = Vector2.zero;
                for (int j = 0; j < 4; ++j) {
                    if (i == j) continue;
                    if (arr[i].x > arr[j].x) ++check[i].x;
                    else if (arr[i].x < arr[j].x) --check[i].x;
                    if (arr[i].y > arr[j].y) ++check[i].y;
                    else if (arr[i].y < arr[j].y) --check[i].y;
                }
            }

            for (int i = 0; i < 4; ++i) {
                if (check[i].x < -1.5f && check[i].y > 1.5f)
                    p00 = arr[i];
                else if (check[i].x > 1.5f && check[i].y > 1.5f)
                    p10 = arr[i];
                else if (check[i].x < 1.5f && check[i].y < 1.5f)
                    p01 = arr[i];
                else
                    p11 = arr[i];
            }
        }

        public static void Swap<T>(ref T value1, ref T value2) {
            T temp = value1;
            value1 = value2;
            value2 = temp;
        }
    }

    public static class GridUtils {
        public static readonly string DefaultGridSizeKey = "DefaultGridSize";
        
        public static Vector3 GetWorldPosition(int c, int r, float cellSize, Vector3 originPosition) {
            return new Vector3(c, r) * cellSize + originPosition;
        }
        public static void GetXY(Vector3 worldPosition, out int c, out int r, float cellSize, Vector3 originPosition) {
            c = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
            r = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
        }
    }

    public static class Bezier {
        public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)  {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                oneMinusT * oneMinusT * p0 +
                2f * oneMinusT * t * p1 +
                t * t * p2;
        }
    }

    public static class StringUtils {
        private static StringBuilder _stringBuilder = new StringBuilder(64);
        public static string MergeStrings(params string[] strList) {
            _stringBuilder.Clear();
            foreach (string str in strList) {
                _stringBuilder.Append(str);
            }
            return _stringBuilder.ToString();
        }
        public static string GetRandomString(params string[] stringList) {
            return stringList[Random.Range(0, stringList.Length)];
        }
    }
}