using System;

namespace Sproto
{
	public class SprotoTypeReader
	{

		private byte[] buffer;
		private long begin;
		private long pos;
		private long size;

		public byte[] Buffer {
			get { return buffer;}
		}

		public long Position {
			get { return this.pos - this.begin; }
		}

		public long Offset {
			get { return this.pos; }
		}

		public long Length {
			get {return this.size - this.begin;}
		}

		public SprotoTypeReader (byte[] buffer, long offset, long size) {
			this.Init(buffer, offset, size);
		}

		public SprotoTypeReader() {
		}


		public void Init(byte[] buffer, long offset, long size) {
			this.begin = offset;
			this.pos = offset;
			this.buffer = buffer;
			this.size = offset + size;
			this.check ();
		}


		private void check() {
			if(this.pos > this.size || this.begin > this.pos) {
				SprotoTypeSize.error("invalid pos.");
			}
		}

		public byte ReadByte () {
			this.check();
			return this.buffer [this.pos++];
		}

		public void Seek (long offset) {
			this.pos = this.begin + offset;
			this.check ();
		}

		public void Read(byte[] data, long offset, long size) {
			long cur_pos = this.pos;
			this.pos += size;
			check ();

			for (long i = cur_pos; i < this.pos; i++) {
				data [offset + i - cur_pos] = this.buffer [i];
			}
		}
	}
}

