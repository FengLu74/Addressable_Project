using System;
using System.IO;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Kit {
    public static partial class Extensions {
        private static readonly byte[] Bytes4 = new byte[4];

        public static void ReadBytes(this Stream stream, byte[] bytes) {
            for (var i = 0; i < bytes.Length; i++) {
                bytes[i] = (byte)stream.ReadByte();
            }
        }

        public static int ReadInt(this Stream stream) {
            ReadBytes(stream, Bytes4);
            return BitConverter.ToInt32(Bytes4, 0);
        }

        public static int ReadShort(this Stream stream) {
            ReadBytes(stream, Bytes4);
            return BitConverter.ToInt16(Bytes4, 0);
        }

        public static float ReadFloat(this Stream stream) {
            ReadBytes(stream, Bytes4);
            return BitConverter.ToSingle(Bytes4, 0);
        }

        // ReSharper disable once ParameterTypeCanBeEnumerable.Global
        public static void WriteBytes(this Stream stream, byte[] bytes) {
            foreach (var t in bytes) {
                stream.WriteByte(t);
            }
        }

        public static void WriteInt(this Stream stream, int val) =>
            WriteBytes(stream, BitConverter.GetBytes(val));

        public static void WriteShort(this Stream stream, short val) =>
            WriteBytes(stream, BitConverter.GetBytes(val));

        public static void WriteFloat(this Stream stream, float val) =>
            WriteBytes(stream, BitConverter.GetBytes(val));
    }
}

// ReSharper restore UnusedMember.Global
// ReSharper restore MemberCanBePrivate.Global
