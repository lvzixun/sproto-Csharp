using System;
using System.IO;

namespace Sproto {

	public class SprotoTypeBase {
		protected SprotoTypeFieldOP has_field;
		protected MemoryStream encode_buffer;

		protected SprotoTypeBase(int max_field_count){
			this.has_field = new SprotoTypeFieldOP (max_field_count);
			this.encode_buffer = new MemoryStream ();
		}

		protected void write_integer(Int64 integer){

		}

		protected void write_string(string str){
		
		}

		protected void write_boolean(bool b){
		
		}

		protected void write_obj(SprotoTypeBase obj){
			
		}

		protected void encode(MemoryStream stream){

		}


		// public API
		public void pack(){

		}

		public void unpack(){

		}

		public void clear(){

			// clear has slot
			this.has_field.clear_field ();

			// clear buffer
			this.encode_buffer.Seek (0, SeekOrigin.Begin);
		}
	}

}