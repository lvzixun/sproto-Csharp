using System;
using System.Collections.Generic;

namespace Sproto
{
	delegate SprotoTypeBase typeFunc (byte[] buffer, int offset);
	public class ProtocolFunctionDictionary
	{
		static Dictionary<int, KeyValuePair<Type, typeFunc>> ResponseDictionary;
		static Dictionary<int, KeyValuePair<Type, typeFunc>> RequestDictionary;

		static ProtocolFunctionDictionary ()
		{
			ResponseDictionary = new Dictionary<int, KeyValuePair<Type, typeFunc>> ();
			RequestDictionary = new Dictionary<int, KeyValuePair<Type, typeFunc>> ();
		}


		static public void SetRequest<T>(int tag) where T: SprotoTypeBase, new() {
			_set<T> (RequestDictionary, tag);
		}

		static public void SetResponse<T>(int tag) where T: SprotoTypeBase, new() {
			_set<T> (ResponseDictionary, tag);
		}

		static private void _set<T>( Dictionary<int, KeyValuePair<Type, typeFunc>> dictionary, int tag) 
			where T : SprotoTypeBase, new() {
			if (dictionary.ContainsKey (tag)) {
				SprotoTypeSize.error ("redefine tag: " + tag);
			}

			typeFunc _func = delegate (byte[] buffer, int offset) {
				T obj = new T();
				obj.init(buffer, offset);
				return obj;
			};

			KeyValuePair<Type, typeFunc> kv = new KeyValuePair<Type, typeFunc> (typeof(T), _func);
			dictionary.Add (tag, kv);
		}
			

		static private SprotoTypeBase _gen(Dictionary<int , KeyValuePair<Type, typeFunc>> dictionary, int tag, byte[] buffer, int offset=0) {
			KeyValuePair<Type, typeFunc> v;
			if (dictionary.TryGetValue (tag, out v)) {
				SprotoTypeBase obj = v.Value (buffer, offset);
				if (obj.GetType () != v.Key) {
					SprotoTypeSize.error ("sproto type: "+obj.GetType().ToString() + "not is expected. [" + v.Key.ToString() + "]");
				}
				return obj;
			}

			return null;
		}

		static public SprotoTypeBase GenResponse(int tag, byte[] buffer, int offset=0) {
			return _gen (ResponseDictionary, tag, buffer, offset);
		}

		static public SprotoTypeBase GenRequest(int tag, byte[] buffer, int offset=0) {
			return _gen (RequestDictionary, tag, buffer, offset);
		}

		public struct ProtocolInfo {
			public Type Request;
			public Type Response;
		};
			
		public static ProtocolInfo GetProtocolInfo(int tag) {
			KeyValuePair<Type, typeFunc> v;
			ProtocolInfo ret;
			ret.Request = null;
			ret.Response = null;

			if (RequestDictionary.TryGetValue (tag, out v)) {
				ret.Request = v.Key;
			}

			if (ResponseDictionary.TryGetValue (tag, out v)) {
				ret.Response = v.Key;
			}

			return ret;
		}
	}
}

