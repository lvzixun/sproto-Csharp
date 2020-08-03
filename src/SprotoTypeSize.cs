using System;
using System.Runtime.InteropServices;

namespace Sproto
{
	public class SprotoTypeSize {
		public static readonly int sizeof_header = 2;
		public static readonly int sizeof_length = 4;
		public static readonly int sizeof_field  = 2;
		public static readonly int encode_max_size = 0x1000000;

		public static void error(string info) {
			throw new Exception (info);
		}
	}

    [StructLayout(LayoutKind.Explicit)]
    public struct UnionValue
    {
        [FieldOffset(0)] public UInt64 integer_v;
        [FieldOffset(0)] public double real_v;
    };
}

