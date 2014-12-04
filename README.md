sproto-Csharp
=============

A pure C# implementation of [sproto](https://github.com/cloudwu/sproto). and using [sprotodump](https://github.com/lvzixun/sproto-Csharp/blob/master/tools/sprotodump.lua) compiler for C# language on your `.sproto` file to generate data access classes.

## Tutorials
You write a `Member.sproto` file :
```
  .Person {
    name 0 : string
    id 1 : integer
    email 2 : string

    .PhoneNumber {
        number 0 : string
        type 1 : integer
    }

    phone 3 : *PhoneNumber
}

.AddressBook {
    person 0 : *Person
}
```
Then you compile it with [sprotodump](https://github.com/lvzixun/sproto-Csharp/blob/master/tools/sprotodump.lua), to produce code in C#.


```
$ lua sprotodump.lua
usage: lua sprotodump.lua [option] <sproto_file>  <outfile_name>

  option:
    -cs              dump to cSharp code file, is default
    -spb             dump to binary spb  file
$
$ lua sprotodump.lua Member.sproto Member.cs
```

Then you use that code like this:

~~~~.c#
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
~~~~

serialize and deserialize :
~~~~.c#
byte[] data = address.encode ();                  // encode to bytes

Sproto.SprotoStream stream = new SprotoStream (); // encode to stream
address.encode(stream);

Sproto.SprotoPack spack = new Sproto.SprotoPack ();
byte[] pack_data = spack.pack (data);             // pack
byte[] unpack_data = spack.unpack(pack_data);     // unpack

AddressBook obj = new AddressBook(unpack_data);   // decode
~~~~

## protocol
the `Test.sproto` file:
```
Foobar 1 {
  request {
    what 0 : string
  }
  response {
    ok 0 : boolean
  }
}
```

dump to c# code:
~~~~.c#
namespace Protocol{ 
  public class Test {
    public static readonly ProtocolFunctionDictionary Protocol = new ProtocolFunctionDictionary ();
    static TestRpc() {
      Protocol.SetProtocol<Foobar> (Foobar.Tag);
      Protocol.SetRequest<TestRpcType.Foobar.request> (Foobar.Tag);
      Protocol.SetResponse<TestRpcType.Foobar.response> (Foobar.Tag);

    }
    
    public class Foobar {
      public const int Tag = 1;
    }

  }
}
~~~~

## RPC API
Read [TestCaseRpc.cs](https://github.com/lvzixun/sproto-Csharp/blob/master/testCase/TestCaseRpc.cs) for detail.



## sprotodump
[sprotodump](https://github.com/lvzixun/sproto-Csharp/blob/master/tools/sprotodump.lua) used to generate C# code or `.spb` binary file.

#### require
  [lpeg](http://www.inf.puc-rio.br/~roberto/lpeg/)

#### option commands
```
  -cs       generate c# code, is default option.
  -spb      generate `.spb` binary file. 
```

## benchmark

in my i5-3470 @3.20GHz :

| library | encode 1M times | decode 1M times |
|:-------:|:---------------:|:---------------:|
| sproto-Csharp | 2.84s         | 3.00s     |
| sproto-Csharp(unpack) | 1.36s | 2.12s     |
| [protobuf-net](https://github.com/mgravell/protobuf-net) | 6.97s | 8.09s |







