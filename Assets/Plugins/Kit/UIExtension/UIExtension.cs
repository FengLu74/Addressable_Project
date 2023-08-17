using UnityEngine.UI;
namespace Kit {
    public static class UIExtension {
        public static void SetText(this Text text, string content) {
            if (text != null) {
                text.text = content;
            }
        }

        public static void SetText(this Text text, int content) => text.SetText(content.ToString());
    }
}
