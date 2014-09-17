using System;
using System.IO;
using System.Collections.Generic;

namespace Sproto {
	public class SprotoTypeBase {
		protected SprotoTypeFieldOP has_field;
		private SprotoTypeSerialize serialize;


		public SprotoTypeBase(int max_field_count) {
			this.has_field = new SprotoTypeFieldOP (max_field_count);
			this.serialize = new SprotoTypeSerialize (max_field_count);
		}


		// write integer
		protected void write_integer(Int64 integer, int tag, int field_idx) {
			this.has_field.set_field (field_idx, true);
			this.serialize.write_integer (integer, tag);
		}

		protected void write_intger(List<Int64> integer_list, int tag, int field_idx) {
			this.has_field.set_field (field_idx, true);
			this.serialize.write_integer (integer_list, tag);
		}


		// write string
		protected void write_string(string str, int tag, int field_idx) {
			this.has_field.set_field (field_idx, true);
			this.serialize.write_string (str, tag);
		}

		protected void write_string(List<string> str_list, int tag, int field_idx) {
			this.has_field.set_field (field_idx, true);
			this.serialize.write_string (str_list, tag);
		}


		// write boolean
		protected void write_boolean(bool b, int tag, int field_idx) {
			this.has_field.set_field (field_idx, true);
			this.serialize.write_boolean (b, tag);
		}

		protected void write_boolean(List<bool> b_list, int tag, int field_idx) {
			this.has_field.set_field (field_idx, true);
			this.serialize.write_boolean (b_list, tag);
		}

		// write struct
		protected void write_obj(SprotoTypeBase obj, int tag, int field_idx) {
			this.has_field.set_field (field_idx, true);
			this.serialize.write_obj (obj, tag);
		}

		protected void srite_obj(List<SprotoTypeBase> obj_list, int tag, int field_idx) {
			this.has_field.set_field (field_idx, true);
			this.serialize.write_obj (obj_list, tag);
		}


		public byte[] encode() {
			return null;
		}

		public void pack(){

		}

		public void unpack(){

		}

		public void clear(){

			// clear has slot
			this.has_field.clear_field ();

			// clear serialize
			this.serialize.clear ();
		}
	}

}