using UnityEngine;

namespace Kit
{
    public static class GizmosHelper
    {
        public static void DrawWireSphere(Transform transform, Color color, float radius)
        {
            if (transform == null)
            {
                return;
            }

            Gizmos.color = color;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        public static void DrawRectangle(RectTransform rectTransform, Color gizmoColor, Camera camera = null)
        {
            if (rectTransform == null)
            {
                return;
            }

            var cam = camera != null ? camera : Camera.main;
            Gizmos.color = gizmoColor;
            var corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            var lb = RectTransformUtility.WorldToScreenPoint(cam, corners[0]);
            var rt = RectTransformUtility.WorldToScreenPoint(cam, corners[2]);
            DrawRectangle(rt, lb);
        }

        private static void DrawRectangle(Vector2 rightTopCorner, Vector2 leftBottomCorner)
        {
            var centerOffset = (rightTopCorner + leftBottomCorner) * 0.5f;
            var displacement = rightTopCorner - leftBottomCorner;
            var x = Vector2.Dot(displacement, Vector2.right);
            var y = Vector2.Dot(displacement, Vector2.up);
            var leftTopCorner = new Vector2(-x * 0.5f, y * 0.5f) + centerOffset;
            var rightBottomCorner = new Vector2(x * 0.5f, -y * 0.5f) + centerOffset;
            Gizmos.DrawLine(rightTopCorner, leftTopCorner);
            Gizmos.DrawLine(leftTopCorner, leftBottomCorner);
            Gizmos.DrawLine(leftBottomCorner, rightBottomCorner);
            Gizmos.DrawLine(rightBottomCorner, rightTopCorner);
        }
    }
}