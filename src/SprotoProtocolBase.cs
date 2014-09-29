using System;

namespace Sproto
{
	public abstract class SprotoProtocolBase {
		public abstract int GetTag();
		public abstract SprotoTypeBase GetRequest();
		public abstract SprotoTypeBase GetResponse();
	}
}

