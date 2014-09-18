using System;
using System.IO;
using System.Collections.Generic;

namespace Sproto {
	public class SprotoTypeBase {
		protected SprotoTypeFieldOP has_field;
		protected SprotoTypeSerialize serialize;
		protected SprotoTypeDeserialize deserialize;


		public SprotoTypeBase(int max_field_count) {
			this.has_field = new SprotoTypeFieldOP (max_field_count);
			this.serialize = new SprotoTypeSerialize (max_field_count);
		}


		public SprotoTypeBase(int max_field_count, byte[] buffer) {
			this.has_field = new SprotoTypeFieldOP (max_field_count);
			this.deserialize = new SprotoTypeDeserialize (buffer);
		}

		public virtual byte[] encode () {
			throw new Exception ("no encode function.");
		}

		public byte[] pack(byte[] buffer) {
			return null;
		}

		public byte[] unpack(byte[] buffer) {
			return null;
		}

		public void clear(){
			// clear has slot
			this.has_field.clear_field ();

			// clear serialize
			if(this.serialize != null)
				this.serialize.clear ();

			// clear deserialize
			if (this.deserialize != null)
				this.deserialize.clear ();
		}
	}

}