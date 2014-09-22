using System;
using MemberType;

namespace sprotoCsharp
{
	public class BenchMark
	{
		public BenchMark ()
		{

			AddressBook address = new AddressBook ();
			address.person = new System.Collections.Generic.List<Person> ();

			Person person = new Person ();
			person.name = "Alice";
			person.id = 10000;

			person.phone = new System.Collections.Generic.List<Person.PhoneNumber> ();
			Person.PhoneNumber num1 = new Person.PhoneNumber ();
			num1.number = "123456789";
			num1.type = 1;
			person.phone.Add (num1);

			Person.PhoneNumber num2 = new Person.PhoneNumber ();
			num2.number = "87654321";
			num2.type = 2;
			person.phone.Add (num2);

			address.person.Add (person);

			Person person1 = new Person ();
			person1.name = "Bob";
			person1.id = 20000;
			person1.phone = new System.Collections.Generic.List<Person.PhoneNumber> ();
			Person.PhoneNumber num3 = new Person.PhoneNumber ();
			num3.number = "01234567890";
			num3.type = 3;
			person1.phone.Add (num3);

			address.person.Add (person1);

			byte[] data = address.encode ();
			byte[] pack_data = address.pack (data);

			double b = this.cur_mseconds ();
			for (int i = 0; i < 1000000; i++) {
				address.init (data);
//				data = address.encode ();
//				address.pack (data);

//				byte[] unpack_data = address.unpack (pack_data);
//				address.init (unpack_data);
			}
			double e = this.cur_mseconds ();
			Console.WriteLine ("total: " + (e - b)/1000  +"s");
		}


		double cur_mseconds() {
			TimeSpan ts = DateTime.Now - new DateTime(1960, 1, 1);
			return ts.TotalMilliseconds;
		}
	}
}

