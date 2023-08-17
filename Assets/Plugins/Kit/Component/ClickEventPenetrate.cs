using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Kit
{
    public class ClickEventPenetrate : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        private readonly List<RaycastResult> _results = new List<RaycastResult>();

        public void OnPointerClick(PointerEventData eventData) =>
            PassEvent(eventData, ExecuteEvents.pointerClickHandler);

        public void OnPointerUp(PointerEventData eventData) =>
            PassEvent(eventData, ExecuteEvents.pointerUpHandler);

        public void OnPointerDown(PointerEventData eventData) =>
            PassEvent(eventData, ExecuteEvents.pointerDownHandler);

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedParameter.Global
        public void PassEvent<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function) where T : IEventSystemHandler
        {
            _results.Clear();
            EventSystem.current.RaycastAll(data, _results);
            var current = data.pointerCurrentRaycast.gameObject;
            foreach (var t in _results)
            {
                // ReSharper disable once InvertIf
                if (current != t.gameObject)
                {
                    ExecuteEvents.Execute(t.gameObject, data, function);
                    break;
                }
            }
        }
    }
}