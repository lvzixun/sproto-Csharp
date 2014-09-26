using System;
using System.IO;
using System.Collections.Generic;

namespace Sproto {
	public abstract class SprotoTypeBase {
		protected SprotoTypeFieldOP has_field;
		protected SprotoTypeSerialize serialize;
		protected SprotoTypeDeserialize deserialize;


		public SprotoTypeBase(int max_field_count) {
			this.has_field = new SprotoTypeFieldOP (max_field_count);
			this.serialize = new SprotoTypeSerialize (max_field_count);
			this.deserialize = new SprotoTypeDeserialize ();
		}

		public long init (byte[] buffer){
			this.clear ();
			this.deserialize.init (buffer);
			this.decode ();

			return this.deserialize.size ();
		}

		public long init (SprotoTypeReader reader) {
			this.clear ();
			this.deserialize.init (reader);
			this.decode ();

			return this.deserialize.size ();
		}

		public SprotoTypeBase(int max_field_count, byte[] buffer) {
			this.has_field = new SprotoTypeFieldOP (max_field_count);
			this.serialize = new SprotoTypeSerialize (max_field_count);
			this.deserialize = new SprotoTypeDeserialize (buffer);
		}

		public abstract byte[] encode ();
		protected abstract void decode ();

		public void clear(){
			// clear has slot
			this.has_field.clear_field ();

			// clear serialize
			this.serialize.clear ();

			// clear deserialize
			this.deserialize.clear ();
		}
	}

}