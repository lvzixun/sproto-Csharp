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
			SprotoRpc.Client client = new SprotoRpc.Client ();
			SprotoRpc.Service service = new SprotoRpc.Service ();

			// ===============foobar=====================
			// request
			TestRpcProtocol.foobar obj = new TestRpcProtocol.foobar ();
			obj.request = new TestRpcType.foobar.request ();
			obj.request.what = "foo";
			byte[] req = client.Request (obj, 1);
			assert (req, new byte[] {0X55, 0X02, 0X04, 0X04, 0X01, 0Xc4, 0X03, 0X66, 0X6f, 0X01, 0X6f});

			// dispatch
			SprotoRpc.Service.RequestInfo sinfo = service.Dispatch (req);
			assert (sinfo.Session == 1);
			TestRpcType.foobar.request req_obj = (TestRpcType.foobar.request)sinfo.Obj;
			assert (req_obj.what == "foo");


			// response
			TestRpcProtocol.foobar resp_obj = new TestRpcProtocol.foobar ();
			resp_obj.response = new TestRpcType.foobar.response ();
			resp_obj.response.ok = true;
			byte[] resp = sinfo.Response (resp_obj);

			// dispatch
			assert (resp, new byte[] {0X55, 0X02, 0X01, 0X04, 0X01, 0X01, 0X04});
			SprotoRpc.Client.ResponseInfo cinfo = client.Dispatch (resp);
			assert (cinfo.Session == 1);
			TestRpcType.foobar.response resp_obj2 = (TestRpcType.foobar.response)cinfo.Obj;
			assert (resp_obj2.ok == true);

			// ================foo====================
			// request
			TestRpcProtocol.foo foo1 = new TestRpcProtocol.foo ();
			req = client.Request (foo1, 2);
			assert (req, new byte[] {0X15, 0X02, 0X06, 0X06});

			// dispatch
			sinfo = service.Dispatch (req);
			assert (sinfo.Session == 2);
			assert (sinfo.Obj == null);

			// response
			TestRpcProtocol.foo resp_foo2 = new TestRpcProtocol.foo();
			resp_foo2.response = new TestRpcType.foo.response();
			resp_foo2.response.ok = false;
			resp = sinfo.Response (resp_foo2);

			// dispatch
			assert (resp, new byte[] {0X55, 0X02, 0X01, 0X06, 0X01, 0X01, 0X02});
			cinfo = client.Dispatch (resp);
			assert (cinfo.Session == 2);
			TestRpcType.foo.response foo3 = (TestRpcType.foo.response)cinfo.Obj;
			assert (foo3.ok == false);


			// ================blackhole====================
			// request
			TestRpcProtocol.blackhole bh1 = new TestRpcProtocol.blackhole ();
			bh1.request = new TestRpcType.blackhole.request ();
			req = client.Request (bh1, 3);

			// dispatch
			sinfo = service.Dispatch (req);
			assert (sinfo.Session == 3);

			// ================bar====================
			// request
			TestRpcProtocol.bar bar1 = new bar ();
			req = client.Request (bar1, 4);

			// dispatch
			sinfo = service.Dispatch (req);
			assert (sinfo.Session == 4);
			assert (sinfo.Obj == null);
		}
	}
}

