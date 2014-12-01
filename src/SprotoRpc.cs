using System;

namespace Sproto
{
	public class SprotoRpc
	{
		public class Client {
			private SprotoStream stream = new SprotoStream();
			private SprotoPack spack = new SprotoPack();

			public struct ResponseInfo {
				public SprotoTypeBase Obj;
				public int Session;
			};

			public Client () {
			}

			public byte[] Request(SprotoProtocolBase protocol, int session) {
				PackageType.Package package = new PackageType.Package();
				package.type = protocol.GetTag ();
				package.session = session;

				this.stream.Seek (0, System.IO.SeekOrigin.Begin);
				int len = package.encode (this.stream);
				SprotoTypeBase request = protocol.GetRequest ();
				if (request != null) {
					len += request.encode (this.stream);
				}

				byte[] buffer = new byte[len];
				stream.Seek (0, System.IO.SeekOrigin.Begin);
				stream.Read (buffer, 0, len);

				return this.spack.pack(buffer);
			}

			public ResponseInfo Dispatch(byte[] buffer) {
				buffer = this.spack.unpack (buffer);
				PackageType.Package package = new PackageType.Package();
				int offset = package.init (buffer);

				ResponseInfo info;
				info.Obj = ProtocolFunctionDictionary.GenResponse ((int)package.type, buffer, offset); 
				info.Session = (int)package.session;

				return info;
			}
		}

		public class Service {
			private SprotoStream stream = new SprotoStream();
			private SprotoPack spack = new SprotoPack();

			public delegate byte[] respFunc (SprotoProtocolBase protocol);

			public Service () {
			}

			public struct RequestInfo {
				public SprotoTypeBase Obj;
				public int Session;
				public  respFunc Response;
			};

			public RequestInfo Dispatch(byte[] buffer) {
				buffer = this.spack.unpack (buffer);
				PackageType.Package package = new PackageType.Package();
				int offset = package.init (buffer);

				int tag = (int)package.type;
				ProtocolFunctionDictionary.ProtocolInfo pinfo = ProtocolFunctionDictionary.GetProtocolInfo(tag);

				RequestInfo info;
				info.Obj = ProtocolFunctionDictionary.GenRequest ((int)package.type, buffer, offset); 
				info.Session = (int)package.session;
				if (pinfo.Response == null) {
					info.Response = null;
				} else {
					info.Response = delegate (SprotoProtocolBase protocol) {
						if (pinfo.Response != protocol.GetResponse().GetType ()) {
							throw new Exception ("response type: " + protocol.GetType ().ToString () + " not is expected. [" + pinfo.Response.ToString () + "]");
						}

						this.stream.Seek(0, System.IO.SeekOrigin.Begin);
						package.encode (this.stream);
						protocol.GetResponse ().encode (this.stream);

						int len = stream.Position;
						byte[] data = new byte[len];
						stream.Seek (0, System.IO.SeekOrigin.Begin);

						stream.Read (data, 0, len);
						return this.spack.pack(data);
					};
				}

				return info;
			}

		}
	}
}

