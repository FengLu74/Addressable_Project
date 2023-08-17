using UnityEngine;
namespace Kit {
    public class MonoUserData : MonoBehaviour {
        // ReSharper disable once NotAccessedField.Global
        public object obj;
        public static MonoUserData Get(GameObject go, object data) {
            if (go == null) {
                return null;
            }
            var userData = go.GetComponent<MonoUserData>();
            if (userData != null) {
                return userData;
            }
            userData = go.AddComponent<MonoUserData>();
            userData.obj = data;
            return userData;
        }
    }

    public class MonoTData<T, TV> : MonoBehaviour where TV : MonoTData<T, TV> {
        // ReSharper disable once NotAccessedField.Global
        public T data;
        public static TV Get(GameObject go, T data) {
            if (go == null) {
                return default;
            }
            var userData = go.GetComponent<TV>();
            if (userData != null) {
                return userData;
            }
            userData = go.AddComponent<TV>();
            userData.data = data;
            return userData;
        }
    }

    public class MonoIntData : MonoTData<int, MonoIntData> { }

    public class StructData<T> where T : struct {
        public T Data;
    }

    public class StructVector3Data : StructData<Vector3> { }
    public class StructVector2Data : StructData<Vector2> { }
    public class StructBoolData : StructData<bool> { }
    public class StructIntData : StructData<int> { }
}
