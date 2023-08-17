using System;
using System.Collections.Generic;
namespace Kit {
    // ReSharper disable UnusedMember.Global
    // ReSharper disable MemberCanBePrivate.Global
    public class EventDispatcher : Singleton<EventDispatcher> {
        public void AddListener<T1, T2, T3, T4>(string evt, Action<T1, T2, T3, T4> callback) =>
            AddListener(evt, (Delegate)callback);
        public void AddListener<T1, T2, T3>(string evt, Action<T1, T2, T3> callback) =>
            AddListener(evt, (Delegate)callback);
        public void AddListener<T1, T2>(string evt, Action<T1, T2> callback) =>
            AddListener(evt, (Delegate)callback);
        public void AddListener<T>(string evt, Action<T> callback) =>
            AddListener(evt, (Delegate)callback);
        public void AddListener(string evt, Action callback) =>
            AddListener(evt, (Delegate)callback);

        public void AddListener(string evt, Delegate callback) {
            if (_listeners.TryGetValue(evt, out var listener)) {
                listener.Combine(callback);
            } else {
                _listeners[evt] = new DelegateData {Delegate = callback};
            }
        }

        public void RemoveListener<T1, T2, T3, T4>(string evt, Action<T1, T2, T3, T4> callback) =>
            RemoveListener(evt, (Delegate)callback);
        public void RemoveListener<T1, T2, T3>(string evt, Action<T1, T2, T3> callback) =>
            RemoveListener(evt, (Delegate)callback);
        public void RemoveListener<T1, T2>(string evt, Action<T1, T2> callback) =>
            RemoveListener(evt, (Delegate)callback);
        public void RemoveListener<T>(string evt, Action<T> callback) =>
            RemoveListener(evt, (Delegate)callback);
        public void RemoveListener(string evt, Action callback) =>
            RemoveListener(evt, (Delegate)callback);
        public void RemoveAllListener(string evt) => _listeners.Remove(evt);

        private void RemoveListener(string evt, Delegate callback) {
            if (!_listeners.TryGetValue(evt, out var listener)) { return; }
            listener.Remove(callback);
            if (listener.Delegate == null) {
                _listeners.Remove(evt);
            }
        }

        public void DispatchTo<T1, T2, T3, T4>(string evt, object target, T1 arg1, T2 arg2, T3 arg3,
            T4 arg4) {
            var methods = GetMethods(evt);
            if (methods == null) { return; }
            foreach (var m in methods) {
                try {
                    // ReSharper disable once ArrangeRedundantParentheses
                    if ((target == null || m.Target == target)) {
                        ((Action<T1, T2, T3, T4>)m)(arg1, arg2, arg3, arg4);
                    }
                } catch (Exception e) { LogException(e); }
            }
        }
        public void DispatchTo<T1, T2, T3>(string evt, object target, T1 arg1, T2 arg2, T3 arg3) {
            var methods = GetMethods(evt);
            if (methods == null) { return; }
            foreach (var m in methods) {
                try {
                    // ReSharper disable once ArrangeRedundantParentheses
                    if ((target == null || m.Target == target)) {
                        ((Action<T1, T2, T3>)m)(arg1, arg2, arg3);
                    }
                } catch (Exception e) { LogException(e); }
            }
        }
        public void DispatchTo<T1, T2>(string evt, object target, T1 arg1, T2 arg2) {
            var methods = GetMethods(evt);
            if (methods == null) { return; }
            foreach (var m in methods) {
                try {
                    // ReSharper disable once ArrangeRedundantParentheses
                    if ((target == null || m.Target == target)) {
                        ((Action<T1, T2>)m)(arg1, arg2);
                    }
                } catch (Exception e) { LogException(e); }
            }
        }
        public void DispatchTo<T>(string evt, object target, T arg) {
            var methods = GetMethods(evt);
            if (methods == null) { return; }
            foreach (var m in methods) {
                try {
                    // ReSharper disable once ArrangeRedundantParentheses
                    if ((target == null || m.Target == target)) {
                        ((Action<T>)m)(arg);
                    }
                } catch (Exception e) { LogException(e); }
            }
        }
        public void DispatchTo(string evt, object target) {
            var methods = GetMethods(evt);
            if (methods == null) { return; }
            foreach (var m in methods) {
                try {
                    // ReSharper disable once ArrangeRedundantParentheses
                    if ((target == null || m.Target == target)) {
                        ((Action)m)();
                    }
                } catch (Exception e) { LogException(e); }
            }
        }

        public void Dispatch<T1, T2, T3, T4>(string evt, T1 arg1, T2 arg2, T3 arg3, T4 arg4) =>
            DispatchTo(evt, null, arg1, arg2, arg3, arg4);
        public void Dispatch<T1, T2, T3>(string evt, T1 arg1, T2 arg2, T3 arg3) =>
            DispatchTo(evt, null, arg1, arg2, arg3);
        public void Dispatch<T1, T2>(string evt, T1 arg1, T2 arg2) =>
            DispatchTo(evt, null, arg1, arg2);
        public void Dispatch<T>(string evt, T arg) => DispatchTo(evt, null, arg);
        public void Dispatch(string evt) => DispatchTo(evt, null);

        private Delegate[] GetMethods(string evt) => _listeners.TryGetValue(evt, out var listener)
            ? listener.GetInvocationList()
            : null;
        private static void LogException(Exception e) => UnityEngine.Debug.LogException(e);

        private class DelegateData {
            public Delegate Delegate { get; set; }
            public void Combine(Delegate @delegate) {
                Delegate = Delegate.Combine(Delegate, @delegate);
                _dirty = true;
            }
            public void Remove(Delegate @delegate) {
                Delegate = Delegate.Remove(Delegate, @delegate);
                _dirty = true;
            }
            public Delegate[] GetInvocationList() {
                if (!_dirty) { return _invocationList; }
                _invocationList = Delegate.GetInvocationList();
                _dirty = false;
                return _invocationList;
            }

            private Delegate[] _invocationList;
            private bool _dirty = true;
        }

        private readonly Dictionary<string, DelegateData> _listeners =
            new Dictionary<string, DelegateData>();
    }
    // ReSharper restore MemberCanBePrivate.Global
    // ReSharper restore UnusedMember.Global
}
