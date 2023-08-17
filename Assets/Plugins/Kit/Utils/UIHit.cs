using System;
using UnityEngine;
using UnityEngine.UI;

namespace Kit
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class UIHit : MaskableGraphic
    {
        protected UIHit() => useLegacyMeshGeneration = false;

        protected override void OnPopulateMesh(VertexHelper vh) => vh.Clear();

        [Obsolete("Use OnPopulateMesh(VertexHelper vh) instead.", false)]
        protected override void OnPopulateMesh(Mesh m) => m.Clear();
    }
}