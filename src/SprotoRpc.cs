using System;

namespace Sproto
{
	public class SprotoRpc
	{
		public SprotoRpc ()
		{
		}

		public class Client {
			public struct ResponseInfo {
				public SprotoTypeBase Obj;
				public int Session;
			};

			static public byte[] Request(SprotoProtocolBase protocol, int session) {
				PackageType.Package package = new PackageType.Package();
				package.type = protocol.GetTag ();
				package.session = session;

				SprotoStream stream = new SprotoStream ();
				int len = package.encode (stream);
				SprotoTypeBase request = protocol.GetRequest ();
				if (request != null) {
					len += request.encode (stream);
				}

				byte[] buffer = new byte[len];
				stream.Seek (0, System.IO.SeekOrigin.Begin);
				stream.Read (buffer, 0, len);

				return buffer;
			}

			static public ResponseInfo Dispatch(byte[] buffer, int offset=0) {
				PackageType.Package package = new PackageType.Package();
				offset += package.init (buffer, offset);

				ResponseInfo info;
				info.Obj = ProtocolFunctionDictionary.GenResponse ((int)package.type, buffer, offset); 
				info.Session = (int)package.session;

				return info;
			}
		}

		public class Service {
			public delegate byte[] respFunc (SprotoProtocolBase protocol);

			public struct RequestInfo {
				public SprotoTypeBase Obj;
				public int Session;
				public  respFunc Response;
			};

			static public RequestInfo Dispatch(byte[] buffer, int offset=0) {
				PackageType.Package package = new PackageType.Package();
				offset += package.init (buffer, offset);

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

						SprotoStream stream = new SprotoStream ();
						package.encode (stream);
						protocol.GetResponse ().encode (stream);

						int len = stream.Position;
						byte[] data = new byte[len];
						stream.Seek (0, System.IO.SeekOrigin.Begin);

						stream.Read (data, 0, len);
						return data;
					};
				}

				return info;
			}

		}
	}
}

