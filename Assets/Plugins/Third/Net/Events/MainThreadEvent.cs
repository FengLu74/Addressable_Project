using System;
using System.Collections.Generic;
using System.Threading;
namespace Net {
    public class MainThreadEvent {
        public Action<uint, object> Callback { private get; set; }
        public void AddEvent(uint id, object data) {
            Monitor.Enter(_eventItems);
            _eventItems.AddLast(new EventItem {EventId = id, EventData = data});
            Monitor.Exit(_eventItems);
        }
        public void Process() {
            Monitor.Enter(_eventItems);
            if (_eventItems.Count > 0) {
                var iter = _eventItems.GetEnumerator();
                while (iter.MoveNext()) {
                    _doingEventItems.AddLast(iter.Current);
                }
                iter.Dispose();
                _eventItems.Clear();
            }
            Monitor.Exit(_eventItems);

            while (_doingEventItems.Count > 0) {
                var t = _doingEventItems.First.Value;
                _doingEventItems.RemoveFirst();
                try {
                    Callback?.Invoke(t.EventId, t.EventData);
                } catch (Exception e) {
                    Dbg.ErrorMsg(
                        $"MainThreadEvent::Process event={t.EventId}::{t.EventData}\n{e}");
                }
                // // 可能在发送后调用了Reset
                // if (_doingEventItems.Count == 0) { break; }
            }
        }
        public void Reset() {
            Monitor.Enter(_eventItems);
            _eventItems.Clear();
            Monitor.Exit(_eventItems);

            _doingEventItems.Clear();
        }

        private struct EventItem {
            public uint EventId { get; set; }
            public object EventData { get; set; }
        }



        private readonly LinkedList<EventItem> _eventItems = new LinkedList<EventItem>();
        private readonly LinkedList<EventItem> _doingEventItems = new LinkedList<EventItem>();
    }
}
