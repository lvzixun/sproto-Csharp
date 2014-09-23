using System;
using Sproto;


namespace sprotoCsharp
{
	public class TestCaseSproto
	{
		public TestCaseSproto ()
		{
		}

		static public void run(){
			new TestCaseSprotoTypeFieldOP ().run ();
			new TestCaseSprotoTypeSerialize ().run ();
			new TestCaseSprotoTypeDeserialize ().run ();
			new TestCaseSprotoExtract ().run ();

			new TestCaseTestAll ().run ();
		}

	}
}

