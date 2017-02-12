using System;

namespace Engine.Extensions {
    public static class ArrayExtensions {
        public static byte[] ToByteArray(this uint[] array) {
            int length = array.Length * sizeof(uint);
            byte[] bytes = new byte[length];
            Buffer.BlockCopy(array, 0, bytes, 0, length);
            return bytes;
        }
    }
}
