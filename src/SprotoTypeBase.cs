using System;
using System.IO;

namespace Sproto {

	public class SprotoTypeBase {
		protected SprotoTypeFieldOP has_field;
		private SprotoTypeSerialize serialize;


		protected SprotoTypeBase(int max_field_count){
			this.has_field = new SprotoTypeFieldOP (max_field_count);
			this.serialize = new SprotoTypeSerialize (max_field_count);
		}

		protected void write_integer(Int64 integer, int tag, int field_idx){
			this.has_field.set_field (field_idx, true);
			this.serialize.write_integer (integer, tag);
		}

		protected void write_string(string str, int tag, int field_idx){
			this.has_field.set_field (field_idx, true);
			this.serialize.write_string (str, tag);
		}

		protected void write_boolean(bool b, int tag, int field_idx){
			this.has_field.set_field (field_idx, true);
			this.serialize.write_boolean (b, tag);
		}

		protected void write_obj(SprotoTypeBase obj, int tag, int field_idx){
			this.has_field.set_field (field_idx, true);
			this.serialize.write_obj (obj, tag);
		}


		// public API
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