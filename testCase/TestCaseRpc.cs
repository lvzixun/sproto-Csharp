using System;
using Sproto;

using TestRpcType;
using TestRpcProtocol;

namespace sprotoCsharp
{
	public class TestCaseRpc : TestCaseBase
	{
		public TestCaseRpc ()
		{
		}

		public override void run() {
			// ===============foobar=====================
			// request
			TestRpcProtocol.foobar obj = new TestRpcProtocol.foobar ();
			obj.request = new TestRpcType.foobar.request ();
			obj.request.what = "test_RPC!!!";
			byte[] req = SprotoRpc.Client.Request (obj, 1);

			// dispatch
			SprotoRpc.Service.RequestInfo sinfo = SprotoRpc.Service.Dispatch (req);
			assert (sinfo.Session == 1);
			TestRpcType.foobar.request req_obj = (TestRpcType.foobar.request)sinfo.Obj;
			assert (req_obj.what == "test_RPC!!!");

			// response
			TestRpcProtocol.foobar resp_obj = new TestRpcProtocol.foobar ();
			resp_obj.response = new TestRpcType.foobar.response ();
			resp_obj.response.ok = true;
			byte[] resp = sinfo.Response (resp_obj);

			// dispatch
			SprotoRpc.Client.ResponseInfo cinfo = SprotoRpc.Client.Dispatch (resp);
			assert (cinfo.Session == 1);
			TestRpcType.foobar.response resp_obj2 = (TestRpcType.foobar.response)cinfo.Obj;
			assert (resp_obj2.ok == true);

			// ================foo====================
			// request
			TestRpcProtocol.foo foo1 = new TestRpcProtocol.foo ();
			req = SprotoRpc.Client.Request (foo1, 2);

			// dispatch
			sinfo = SprotoRpc.Service.Dispatch (req);
			assert (sinfo.Session == 2);
			assert (sinfo.Obj == null);

			// response
			TestRpcProtocol.foo resp_foo2 = new TestRpcProtocol.foo();
			resp_foo2.response = new TestRpcType.foo.response();
			resp_foo2.response.ok = false;
			resp = sinfo.Response (resp_foo2);

			// dispatch
			cinfo = SprotoRpc.Client.Dispatch (resp);
			assert (cinfo.Session == 2);
			TestRpcType.foo.response foo3 = (TestRpcType.foo.response)cinfo.Obj;
			assert (foo3.ok == false);


			// ================blackhole====================
			// request
			TestRpcProtocol.blackhole bh1 = new TestRpcProtocol.blackhole ();
			bh1.request = new TestRpcType.blackhole.request ();
			req = SprotoRpc.Client.Request (bh1, 3);

			// dispatch
			sinfo = SprotoRpc.Service.Dispatch (req);
			assert (sinfo.Session == 3);

			// ================bar====================
			// request
			TestRpcProtocol.bar bar1 = new bar ();
			req = SprotoRpc.Client.Request (bar1, 4);

			// dispatch
			sinfo = SprotoRpc.Service.Dispatch (req);
			assert (sinfo.Session == 4);
			assert (sinfo.Obj == null);
		}
	}
}

