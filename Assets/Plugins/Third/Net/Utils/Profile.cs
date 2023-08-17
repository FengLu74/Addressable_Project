using System;
namespace Net {
    public class Profile {
        public Profile(string name) => _name = name;
        public void Start() => _startTime = DateTime.Now;
        public void Stop() {
            var time = DateTime.Now - _startTime;

            if (time.TotalMilliseconds >= 100) {
                Dbg.WarningMsg($"Profile::profile(): '{_name}' took {time.TotalMilliseconds} ms");
            }
        }

        private readonly string _name;
        private DateTime _startTime;
    }
}
