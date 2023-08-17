using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Kit
{
    public enum EAnchorType
    {
        None,
        LeftTop,
        RightTop,
        LeftBottom,
        RightBottom,
        Full,
    }

    public static partial class Extensions
    {
        public static void SetChildMaskableGraphicAlpha(this Transform root, float alpha = 0.5f)
        {
            var maskableGraphics = root.GetComponentsInChildren<MaskableGraphic>();
            foreach (var graphic in maskableGraphics)
            {
                var graphicColor = graphic.color;
                graphicColor.a = alpha;
                graphic.color = graphicColor;
            }
        }

        public static Transform FindInChildren(this Transform root, string name, bool includeHidden)
        {
            if (root == null || string.IsNullOrEmpty(name))
            {
                return root;
            }

            return root.GetComponentsInChildren<Transform>(includeHidden)
                .FirstOrDefault(t => t.name.ToUpper() == name.ToUpper());
        }

        public static void SetAnchor(this RectTransform rectTransform, EAnchorType anchorType)
        {
            switch (anchorType)
            {
                case EAnchorType.LeftTop:
                    rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
                    rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
                    rectTransform.anchorMax = new Vector2(0, 1);
                    rectTransform.anchorMin = new Vector2(0, 1);
                    rectTransform.pivot = new Vector2(0, 1);
                    break;
                case EAnchorType.RightTop:
                    rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
                    rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 0);
                    rectTransform.anchorMax = Vector2.one;
                    rectTransform.anchorMin = Vector2.one;
                    rectTransform.pivot = Vector2.one;
                    break;
                case EAnchorType.LeftBottom:
                    rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 0);
                    rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
                    rectTransform.anchorMax = Vector2.zero;
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.pivot = Vector2.zero;
                    break;
                case EAnchorType.RightBottom:
                    rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 0);
                    rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 0);
                    rectTransform.anchorMax = new Vector2(1, 0);
                    rectTransform.anchorMin = new Vector2(1, 0);
                    rectTransform.pivot = new Vector2(1, 0);
                    break;
                case EAnchorType.Full:
                    rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
                    rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
                    rectTransform.anchorMax = Vector2.one;
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    break;
            }
        }
    }
}