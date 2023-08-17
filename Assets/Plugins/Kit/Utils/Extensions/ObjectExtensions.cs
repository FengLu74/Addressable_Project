using UnityEngine;
namespace Kit {
    public static partial class Extensions {
        public static bool NearlyEqual(this float f, float v) => Mathf.Approximately(f, v);

        public static bool ApproximatelyEqual(this Vector3 f, Vector3 v) =>
            f.x.NearlyEqual(v.x) && f.y.NearlyEqual(v.y) && f.z.NearlyEqual(v.z);

        public static bool NearlyEqual(this Vector3 f, Vector3 v) => f == v;

        public static bool NearlyEqual(this Quaternion f, Quaternion v) => f == v;

        public static bool NearlyEqual(this Vector2 f, Vector2 v) =>
            f.x.NearlyEqual(v.x) && f.y.NearlyEqual(v.y);

        public static Vector2 DividedBy(this Vector2 vector, Vector2 divisor) {
            for (var i = 0; i < 2; ++i) {
                vector[i] /= divisor[i];
            }
            return vector;
        }

        public static Vector2 Abs(this Vector2 vector) {
            for (var i = 0; i < 2; ++i) {
                vector[i] = Mathf.Abs(vector[i]);
            }
            return vector;
        }

        public static Vector3 SafeNormalized(this Vector3 vector) =>
            vector == Vector3.zero ? Vector3.zero : vector.normalized;

        public static float SafeMagnitude(this Vector3 vector) =>
            vector == Vector3.zero ? 0f : vector.magnitude;

        public static float SafeMagnitude(this Vector2 vector) =>
            vector == Vector2.zero ? 0f : vector.magnitude;

        public static Quaternion SafeLerp(this Quaternion rotation, Quaternion target, float t) {
            // ReSharper disable once InvertIf
            if (Quaternion.Angle(rotation, target) > 179f) {
                target.ToAngleAxis(out var angle, out var axis);
                target = Quaternion.AngleAxis(angle + 1f, axis);
            }
            return Quaternion.Lerp(rotation, target, t);
        }

        /// <summary>
        /// 获取对称点
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="lookAtPoint"></param>
        /// <returns></returns>
        public static Vector3 GetSymmetryPoint(this Transform transform, Vector3 lookAtPoint) {
            var pos = transform.position;
            return new Vector3(
                pos.x * 2 - lookAtPoint.x,
                pos.y * 2 - lookAtPoint.y,
                pos.z * 2 - lookAtPoint.z);
        }
    }
}
