using System;
using System.IO;
using System.Collections.Generic;

namespace Sproto {
	public abstract class SprotoTypeBase {
		protected SprotoTypeFieldOP has_field;
		protected SprotoTypeSerialize serialize;
		protected SprotoTypeDeserialize deserialize;
		protected SprotoTypeExtract extract;


		public SprotoTypeBase(int max_field_count) {
			this.has_field = new SprotoTypeFieldOP (max_field_count);
			this.serialize = new SprotoTypeSerialize (max_field_count);
			this.extract = new SprotoTypeExtract ();
		}

		public virtual void init (byte[] buffer){
			this.clear ();
			this.deserialize = new SprotoTypeDeserialize (buffer);
		}

		public SprotoTypeBase(int max_field_count, byte[] buffer) {
			this.has_field = new SprotoTypeFieldOP (max_field_count);
			this.deserialize = new SprotoTypeDeserialize (buffer);
			this.extract = new SprotoTypeExtract ();
		}

		public abstract byte[] encode ();
		protected abstract void decode (byte[] buffer);

		public byte[] pack(byte[] buffer) {
			return this.extract.pack (buffer);
		}

		public byte[] unpack(byte[] buffer) {
			return this.extract.unpack (buffer);
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