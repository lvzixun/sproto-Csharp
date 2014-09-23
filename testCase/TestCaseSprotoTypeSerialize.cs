using System;
using Sproto;
using System.Collections.Generic;

namespace sprotoCsharp
{
	public class TestCaseSprotoTypeSerialize : TestCaseBase
	{
		public TestCaseSprotoTypeSerialize ()
		{
		}

		private void test_field() {
			SprotoTypeSerialize serialize = new SprotoTypeSerialize (20);

			/*
			 * .Test {
			 *   var1 0: integer
			 * 	 var2 1: string
			 *   var3 5: intger
			 *   var4 7: boolean
			 * 	 var5 9: integer
			 * 	 var6 10: boolean
			 *   var7 12: intger
			 * }
			 * */

			byte[] test_result_data = {
				0X0b, 0X00, 0X00, 0X00, 0X00, 0X00, 0X05, 0X00, 0Xac, 0X88, 0X01, 0X00, 0X04, 
				0X00, 0X01, 0X00, 0X00, 0X00, 0X02, 0X00, 0X01, 0X00, 0X46, 0X22, 0X04, 0X00, 
				0X00, 0X00, 0Xde, 0Xff, 0Xff, 0Xff, 0X0b, 0X00, 0X00, 0X00, 0X74, 0X65, 0X73, 
				0X74, 0X5f, 0X73, 0X74, 0X72, 0X69, 0X6e, 0X67, 0X08, 0X00, 0X00, 0X00, 0X66, 
				0X55, 0X44, 0X33, 0X22, 0X11, 0X00, 0X00
			};

			serialize.write_integer (-34, 0);
			serialize.write_string ("test_string", 1);
			serialize.write_integer (0x4455, 5);
			serialize.write_boolean (true, 7);
			serialize.write_integer (0x112233445566, 9);
			serialize.write_boolean (false, 10);
			serialize.write_integer (0x1122, 12);


			byte[] buffer = serialize.encode ();
			Console.WriteLine ("======== encode buffer ===========");
			dump_bytes (buffer);
			assert(buffer, test_result_data);
		}

		private void test_array() {
			/*
			 * .Test {
			 * 	var1 0: *boolean
			 *  var2 4: *integer
			 *  var3 5: *string
			 * }
			 * */

			SprotoTypeSerialize serialize = new SprotoTypeSerialize (20);

			List<Int64> data = new List<Int64> ();
			data.Add (4);
			data.Add (0x1123);
			data.Add (0x1122334455);
			data.Add (-0x778899aabb);
			data.Add (-6);

			List<bool> b_data = new List<bool> ();
			b_data.Add (true);
			b_data.Add (false);
			b_data.Add (true);

			List<string> str_data = new List<string> ();
			str_data.Add ("abc");
			str_data.Add ("1234");
			str_data.Add ("fgcbvb");

			byte[] test_result_data = {
				0X04, 0X00, 0X00, 0X00, 0X05, 0X00, 0X00, 0X00, 0X00, 0X00, 0X03, 0X00, 0X00,
				0X00, 0X01, 0X00, 0X01, 0X29, 0X00, 0X00, 0X00, 0X08, 0X04, 0X00, 0X00, 0X00,
				0X00, 0X00, 0X00, 0X00, 0X23, 0X11, 0X00, 0X00, 0X00, 0X00, 0X00, 0X00, 0X55,
				0X44, 0X33, 0X22, 0X11, 0X00, 0X00, 0X00, 0X45, 0X55, 0X66, 0X77, 0X88, 0Xff,
				0Xff, 0Xff, 0Xfa, 0Xff, 0Xff, 0Xff, 0Xff, 0Xff, 0Xff, 0Xff, 0X19, 0X00, 0X00,
				0X00, 0X03, 0X00, 0X00, 0X00, 0X61, 0X62, 0X63, 0X04, 0X00, 0X00, 0X00, 0X31,
				0X32, 0X33, 0X34, 0X06, 0X00, 0X00, 0X00, 0X66, 0X67, 0X63, 0X62, 0X76, 0X62,
			};

			serialize.write_boolean (b_data, 0);
			serialize.write_integer (data, 4);
			serialize.write_string (str_data, 5);



			Console.Write ("====== array dump ========");
			byte[] buffer = serialize.encode ();
			dump_bytes (buffer);

			assert(buffer, test_result_data);
		}

		public override void run() {
			this.test_field();
			this.test_array ();
		}
	}
}

