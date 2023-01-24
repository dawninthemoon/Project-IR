using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectEditor {
    public static class EditorUtils {
        public static Vector3 GetMouseWorldPosition() {
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
    }
}