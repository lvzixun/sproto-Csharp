using System;
using Sproto;
using System.Collections.Generic;


namespace sprotoCsharp
{
	public class TestCaseSprotoTypeDeserialize : TestCaseBase
	{
		public TestCaseSprotoTypeDeserialize ()
		{
		}

		private void test_field() {

//			.Test {
//				var1 0: integer
//				var2 3: string
//				var3 6: boolean
//				var4 9: integer
//				var5 1: integer
//				var6 2: integer 
	//			var7 4: *integer
	//			var8 10: *boolean
	//			var9 11: *string
//			}

			byte[] data = {
				0X0b, 0X00, 0X06, 0X00, 0X00, 0X00, 0X00, 0X00, 0X00, 0X00, 0X00, 0X00, 0X01,
				0X00, 0X04, 0X00, 0X03, 0X00, 0X00, 0X00, 0X00, 0X00, 0X00, 0X00, 0X08, 0X00,
				0X00, 0X00, 0X66, 0X55, 0X44, 0X33, 0X22, 0X11, 0X00, 0X00, 0X08, 0X00, 0X00,
				0X00, 0X89, 0X99, 0Xaa, 0Xbb, 0Xcc, 0Xdd, 0Xee, 0Xff, 0X03, 0X00, 0X00, 0X00,
				0X61, 0X62, 0X63, 0X39, 0X00, 0X00, 0X00, 0X08, 0X0b, 0X00, 0X00, 0X00, 0X00,
				0X00, 0X00, 0X00, 0Xfe, 0Xff, 0Xff, 0Xff, 0Xff, 0Xff, 0Xff, 0Xff, 0X44, 0X33,
				0X22, 0X11, 0X00, 0X00, 0X00, 0X00, 0X00, 0X00, 0X00, 0X00, 0X00, 0X00, 0X00,
				0X00, 0X07, 0X00, 0X00, 0X00, 0X00, 0X00, 0X00, 0X00, 0X66, 0X55, 0X44, 0X33,
				0X22, 0X11, 0X00, 0X00, 0X89, 0X99, 0Xaa, 0Xbb, 0Xcc, 0Xdd, 0Xee, 0Xff, 0X04,
				0X00, 0X00, 0X00, 0Xfc, 0Xff, 0Xff, 0Xff, 0X04, 0X00, 0X00, 0X00, 0X01, 0X00,
				0X01, 0X00, 0X16, 0X00, 0X00, 0X00, 0X03, 0X00, 0X00, 0X00, 0X61, 0X61, 0X62,
				0X03, 0X00, 0X00, 0X00, 0X63, 0X63, 0X63, 0X04, 0X00, 0X00, 0X00, 0X64, 0X64,
				0X64, 0X64,
			};

			SprotoTypeDeserialize deserialize = new SprotoTypeDeserialize(data);

			int tag = -1;
			while (-1 != (tag = deserialize.read_tag ())) {
				switch (tag) {
				case 0:
					assert (deserialize.read_integer () == 2);
					break;
				case 3:
					assert (deserialize.read_string () == "abc");
					break;
				case 6:
					assert (deserialize.read_boolean () == true);
					break;
				case 9:
					assert (deserialize.read_integer () == -4);
					break;
				case 1:
					assert (deserialize.read_integer () == 0x112233445566);
					break;
				case 2:
					assert (deserialize.read_integer() == -0x11223344556677);
					break;
				case 4:
					assert (deserialize.read_integer_list(), new Int64[] {11, -2, 0x11223344, 0, 7, 0x112233445566, -0x11223344556677});
					break;
				case 11:
					assert (deserialize.read_string_list(), new string[] {"aab", "ccc", "dddd"});
					break;
				case 10:
					assert (deserialize.read_boolean_list(), new bool[] {true, false, true, false});
					break;
				default:
					deserialize.read_unknow_data ();
					Console.WriteLine ("unknow field tag: " + tag);
					break;
				}
			}
		}
			
		public override void run() {
			this.test_field();
		}
	}
}

