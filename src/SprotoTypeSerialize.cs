using System;
using System.IO;
using System.Collections.Generic;

namespace Sproto
{
	public class SprotoTypeSerialize {
		private byte[] header;
		private int header_idx = 2;

		private MemoryStream data;

		private int lasttag = -1;
		private int index = 0;


		public SprotoTypeSerialize (int max_field_count) {
			this.header = new byte[
				SprotoTypeSize.sizeof_header + 
				max_field_count * SprotoTypeSize.sizeof_field];

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

		private void write_uint32_to_uint64_sign(bool is_negative) {
			byte v = (byte)((is_negative)?(0xff):(0));

			this.data.WriteByte (v);
			this.data.WriteByte (v);
			this.data.WriteByte (v);
			this.data.WriteByte (v);
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

		private void write_uint32(UInt32 v) {
			this.data.WriteByte ((byte)(v & 0xff));
			this.data.WriteByte ((byte)((v >> 8) & 0xff));
			this.data.WriteByte ((byte)((v >> 16) & 0xff));
			this.data.WriteByte ((byte)((v >> 24) & 0xff));
		}

		private void write_uint64(UInt64 v) {
			this.data.WriteByte ((byte)(v & 0xff));
			this.data.WriteByte ((byte)((v >> 8) & 0xff));
			this.data.WriteByte ((byte)((v >> 16) & 0xff));
			this.data.WriteByte ((byte)((v >> 24) & 0xff));
			this.data.WriteByte ((byte)((v >> 32) & 0xff));
			this.data.WriteByte ((byte)((v >> 40) & 0xff));
			this.data.WriteByte ((byte)((v >> 48) & 0xff));
			this.data.WriteByte ((byte)((v >> 56) & 0xff));
		}

		private void fill_size(int sz) {
			if (sz <= 0)
				error ("fill invaild size.");

			this.write_uint32 ((UInt32)sz);
		}

		private int encode_integer(UInt32 v) {
			this.fill_size (sizeof(UInt32));

			this.write_uint32 (v);
			return SprotoTypeSize.sizeof_length + sizeof(UInt32);
		}

		private int encode_uint64(UInt64 v) {
			this.fill_size (sizeof(UInt64));

			this.write_uint64 (v);
			return SprotoTypeSize.sizeof_length + sizeof(UInt64);
		}

		private int encode_string(string str){
			this.fill_size (str.Length);

			byte[] s = System.Text.Encoding.UTF8.GetBytes (str);
			this.data.Write (s, 0, s.Length);

			return SprotoTypeSize.sizeof_length + str.Length;
		}
			
		private int encode_struct(SprotoTypeBase obj){
			byte[] data = obj.encode ();

			this.fill_size (data.Length);
			this.data.Write (data, 0, data.Length);

			return SprotoTypeSize.sizeof_length + data.Length;
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

		public void write_integer(List<Int64> integer_list, int tag) {
			if (integer_list == null || integer_list.Count <= 0)
				return;

			long sz_pos = this.data.Position;
			this.data.Seek (sz_pos + SprotoTypeSize.sizeof_length, SeekOrigin.Begin);

			long begin_pos = this.data.Position;
			int intlen = sizeof(UInt32);
			this.data.Seek (begin_pos + 1, SeekOrigin.Begin);

			for (int index = 0; index < integer_list.Count; index++) {
				Int64 v = integer_list [index];
				Int64 vh = v >> 31;
				int sz = (vh == 0 || vh == -1)?(sizeof(UInt32)):(sizeof(UInt64));

				if (sz == sizeof(UInt32)) {
					this.write_uint32 ((UInt32)v);
					if (intlen == sizeof(UInt64)) {
						bool is_negative = ((v & 0x80000000) == 0) ? (false) : (true);
						this.write_uint32_to_uint64_sign (is_negative);
					}

				} else if (sz == sizeof(UInt64)) {
					if (intlen == sizeof (UInt32)) {
						this.data.Seek (begin_pos+1, SeekOrigin.Begin);
						for (int i = 0; i < index; i++) {
							UInt64 value = (UInt64)(integer_list[i]);
							this.write_uint64 (value);
						}
						intlen = sizeof(UInt64);
					}
					this.write_uint64 ((UInt64)v);

				} else {
					error ("invalid integer size(" + sz + ")");
				}
			}

			// fill integer size
			long cur_pos = this.data.Position;
			this.data.Seek (begin_pos, SeekOrigin.Begin);
			this.data.WriteByte ((byte)intlen);

			// fill array size
			int size = (int)(cur_pos - begin_pos);
			this.data.Seek (sz_pos, SeekOrigin.Begin);
			this.fill_size (size);

			this.data.Seek (cur_pos, SeekOrigin.Begin);
			this.write_tag (tag, 0);
		}


		public void write_boolean(bool b, int tag) {
			Int64 v = (b)?(1):(0);
			this.write_integer (v, tag);
		}

		public void write_boolean(List<bool> b_list, int tag) {
			if (b_list == null || b_list.Count <= 0)
				return;

			this.fill_size (b_list.Count);
			for (int i = 0; i < b_list.Count; i++) {
				byte v = (byte)((b_list [i])?(1):(0));
				this.data.WriteByte (v);
			}

			this.write_tag (tag, 0);
		}


		public void write_string(string str, int tag) {
			this.encode_string (str);
			this.write_tag (tag, 0);
		}

		public void write_string(List<string> str_list, int tag) {
			if (str_list == null || str_list.Count <= 0)
				return;

			// write size length
			int sz = 0;
			foreach (string v in str_list) {
				sz += SprotoTypeSize.sizeof_length + v.Length;
			}
			this.fill_size (sz);

			// write stirng
			foreach (string v in str_list) {
				this.encode_string (v);
			}

			this.write_tag (tag, 0);
		}


		public void write_obj(SprotoTypeBase obj, int tag) {
			this.encode_struct (obj);
			this.write_tag (tag, 0);
		}
			
		public void write_obj<T>(List<T> obj_list, int tag) where T :SprotoTypeBase {
			if (obj_list == null || obj_list.Count <= 0)
				return;

			long sz_pos = this.data.Position;
			this.data.Seek (SprotoTypeSize.sizeof_length, SeekOrigin.Current);

			foreach (SprotoTypeBase v in obj_list) {
				this.encode_struct (v);
			}

			long cur_pos = this.data.Position;
			int sz = (int)(cur_pos - sz_pos - SprotoTypeSize.sizeof_length);
			this.data.Seek (sz_pos, SeekOrigin.Begin);
			this.fill_size (sz);

			this.data.Seek (cur_pos, SeekOrigin.Begin);

			this.write_tag (tag, 0);
		}


		public byte[] encode() {
			this.set_header_fn (this.index);
			int buffer_sz = this.header_idx + (int)this.data.Position;

			// fix me
			if (buffer_sz >= SprotoTypeSize.encode_max_size)
				error ("object is too large (>" + SprotoTypeSize.encode_max_size + ")");

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

