using System;
using Sproto;

using TestRpcType;

namespace sprotoCsharp
{
	public class TestCaseRpc : TestCaseBase
	{
		public TestCaseRpc ()
		{
		}

		public override void run() {
			SprotoRpc client = new SprotoRpc ();
			SprotoRpc service = new SprotoRpc (Protocol.TestRpc.Protocol);
			SprotoRpc.RpcRequest clientRequest = client.Attach (Protocol.TestRpc.Protocol);

			// ===============foobar=====================
			// request
			TestRpcType.foobar.request obj = new TestRpcType.foobar.request ();
			obj.what = "foo";
			byte[] req = clientRequest.Invoke<Protocol.TestRpc.foobar> (obj, 1);
			assert (req, new byte[] {0X55, 0X02, 0X04, 0X04, 0X01, 0Xc4, 0X03, 0X66, 0X6f, 0X01, 0X6f});

			// dispatch
			SprotoRpc.RpcInfo sinfo = service.Dispatch (req);
			assert (sinfo.type == SprotoRpc.RpcType.REQUEST);
			assert (sinfo.requestObj.GetType () == typeof(TestRpcType.foobar.request));
			assert (sinfo.Response != null);
			TestRpcType.foobar.request req_obj = (TestRpcType.foobar.request)sinfo.requestObj;
			assert (req_obj.what == "foo");

			// response
			TestRpcType.foobar.response obj2 = new foobar.response ();
			obj2.ok = true;
			byte[] resp = sinfo.Response (obj2);
			assert (resp, new byte[] {0X55, 0X02, 0X01, 0X04, 0X01, 0X01, 0X04});

			// dispatch
			sinfo = client.Dispatch (resp);
			assert (sinfo.type == SprotoRpc.RpcType.RESPONSE);
			assert (sinfo.session == 1);
			assert (((TestRpcType.foobar.response)sinfo.responseObj).ok == true);
	
			// ================foo====================
			// request
			req =  clientRequest.Invoke<Protocol.TestRpc.foo> (null, 2);
			assert (req, new byte[] {0X15, 0X02, 0X06, 0X06});

			// dispatch
			sinfo = service.Dispatch (req);
			assert (sinfo.type == SprotoRpc.RpcType.REQUEST);
			assert (sinfo.tag == Protocol.TestRpc.foo.Tag);
			assert (sinfo.requestObj == null);

			// response
			TestRpcType.foo.response obj3 = new foo.response();
			obj3.ok = false;
			resp = sinfo.Response (obj3);
			assert (resp, new byte[] {0X55, 0X02, 0X01, 0X06, 0X01, 0X01, 0X02});

			// dispatch
			sinfo = client.Dispatch (resp);
			assert (sinfo.type == SprotoRpc.RpcType.RESPONSE);
			assert (sinfo.session == 2);
			assert (((TestRpcType.foo.response)sinfo.responseObj).ok == false);

			// ================bar====================
			// request
			req = clientRequest.Invoke<Protocol.TestRpc.bar> ();
			assert (req, new byte[] { 0X05, 0X01, 0X08, });

			// dispatch
			sinfo = service.Dispatch (req);
			assert (sinfo.type == SprotoRpc.RpcType.REQUEST);
			assert (sinfo.requestObj == null);
			assert (sinfo.tag == Protocol.TestRpc.bar.Tag);
			assert (sinfo.Response == null);

			// ================blackhole====================
			// request
			req = clientRequest.Invoke<Protocol.TestRpc.blackhole> ();
			assert (req, new byte[]{ 0X05, 0X01, 0X0a });
		}
	}
}

