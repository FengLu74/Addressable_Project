using System.IO;
namespace Net {
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Buffer {
        public Buffer() {
            _memoryStream = new MemoryStream();
            _writer = new BinaryWriter(_memoryStream);
            _reader = new BinaryReader(_memoryStream);
        }
        ~Buffer() {
            if (_memoryStream == null) { return; }
            _memoryStream.Dispose();
            _memoryStream = null;
        }
        public BinaryReader BeginRead() {
            _memoryStream.Seek(0, SeekOrigin.Begin);
            return _reader;
        }
        public BinaryWriter BeginWrite() {
            _memoryStream.Seek(0, SeekOrigin.Begin);
            _memoryStream.SetLength(0);
            return _writer;
        }
        public void WriteData(byte[] data, int offset, int len) {
            BeginWrite();
            _writer.Write(data, offset, len);
        }
        public byte[] GetData() => _memoryStream.GetBuffer();
        public int GetDataLength() => (int)_memoryStream.Length;

        private MemoryStream _memoryStream;
        private readonly BinaryWriter _writer;
        private readonly BinaryReader _reader;
    }
}
