using System;
using System.IO;

namespace Sproto
{
	public class SprotoTypeSerialize {
		static readonly int sizeof_header = 2;
		static readonly int sizeof_length = 4;
		static readonly int sizeof_field  = 2;
		static readonly int encode_max_size = 0x1000000;

		private byte[] header;
		private int header_idx = 2;

		private MemoryStream data;

		private int lasttag = -1;
		private int index = 0;


		public SprotoTypeSerialize (int max_field_count) {
			this.header = new byte[sizeof_header + max_field_count * sizeof_field];
			this.data = new MemoryStream ();
		}

		private void set_header_fn(int fn) {
			this.header [0] = (byte)(fn & 0xff);
			this.header [1] = (byte)((fn >> 8) & 0xff);
		}

		private void write_header_record(int record) {
			this.header [this.header_idx] = (byte)(record & 0xff);
			this.header [this.header_idx + 1] = (byte)((record >> 8) & 0xff);

			this.header_idx+=2;
			this.index++;
		}

		private void write_tag(int tag, int value) {
			int stag = tag - this.lasttag - 1;
			if (stag > 0) {
				// skip tag
				stag = (stag - 1) * 2 + 1;
				if (stag > 0xffff)
					error ("tag is too big.");

				this.write_header_record (stag);
			}

			this.write_header_record (value);
			this.lasttag = tag;
		}

		private void fill_size(int sz) {
			if (sz <= 0)
				error ("fill invaild size.");

			this.data.WriteByte ((byte)(sz & 0xff));
			this.data.WriteByte ((byte)((sz >> 8) & 0xff));
			this.data.WriteByte ((byte)((sz >> 16) & 0xff));
			this.data.WriteByte ((byte)((sz >> 24) & 0xff));
		}

		private int encode_integer(UInt32 v) {
			this.fill_size (sizeof(UInt32));

			this.data.WriteByte ((byte)(v & 0xff));
			this.data.WriteByte ((byte)((v >> 8) & 0xff));
			this.data.WriteByte ((byte)((v >> 16) & 0xff));
			this.data.WriteByte ((byte)((v >> 24) & 0xff));

			return sizeof_length + sizeof(UInt32);
		}

		private int encode_uint64(UInt64 v) {
			this.fill_size (sizeof(UInt64));

			this.data.WriteByte ((byte)(v & 0xff));
			this.data.WriteByte ((byte)((v >> 8) & 0xff));
			this.data.WriteByte ((byte)((v >> 16) & 0xff));
			this.data.WriteByte ((byte)((v >> 24) & 0xff));
			this.data.WriteByte ((byte)((v >> 32) & 0xff));
			this.data.WriteByte ((byte)((v >> 40) & 0xff));
			this.data.WriteByte ((byte)((v >> 48) & 0xff));
			this.data.WriteByte ((byte)((v >> 56) & 0xff));

			return sizeof_length + sizeof(UInt64);
		}

		private int encode_string(string str){
			this.fill_size (str.Length);

			byte[] s = System.Text.Encoding.UTF8.GetBytes (str);
			this.data.Write (s, 0, s.Length);

			return sizeof_length + str.Length;
		}
			

		private static void error(string info) {
			throw new Exception (info);
		}





		// API
		public void write_integer(Int64 integer, int tag) {
			Int64 vh = integer >> 31;
			int sz = (vh == 0 || vh == -1)?(sizeof(UInt32)):(sizeof(UInt64));
			int value = 0;

			if (sz == sizeof(UInt32)) {
				UInt32 v = (UInt32)integer;
				if (v < 0x7fff) {
					value = (int)((v + 1) * 2);
					sz = 2;
				} else {
					sz = this.encode_integer (v);
				}

			} else if (sz == sizeof(UInt64)) {
				UInt64 v = (UInt64)integer;
				sz = this.encode_uint64 (v);

			} else {
				error("invaild integer size.");
			}

			this.write_tag (tag, value);
		}

		public void write_boolean(bool b, int tag) {
			Int64 v = (b)?(1):(0);
			this.write_integer (v, tag);
		}


		public void write_string(string str, int tag) {
			this.encode_string (str);
			this.write_tag (tag, 0);
		}

		public void write_obj(SprotoTypeBase obj, int tag) {
			error("TODO IT!");
		}


		public byte[] encode() {
			this.set_header_fn (this.index);
			int buffer_sz = this.header_idx + (int)this.data.Position;

			// fix me
			if (buffer_sz >= encode_max_size)
				error ("object is too large (>" + encode_max_size + ")");

			byte[] buffer = new byte[buffer_sz];

			// merge header and data
			for (int i = 0; i < this.header_idx; i++) {
				buffer [i] = this.header [i];
			}

			this.data.Seek (0, SeekOrigin.Begin);
			this.data.Read (buffer, this.header_idx, buffer_sz - this.header_idx);

			// clear state
			this.clear ();

			return buffer;
		}
			
		public void clear() {
			this.index = 0;
			this.header_idx = 2;
			this.lasttag = -1;
			this.data.Seek (0, SeekOrigin.Begin);
		}

	}
}

