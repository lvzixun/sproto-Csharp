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
byte[] data = address.encode ();                  // encode 

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
namespace TestProtocol{ 
  
  public class Foobar {
    public const int tag = 1;

    public TestType.foobar.request request;
    public TestType.foobar.response response;
  }

}
~~~~


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
| sproto-Csharp | 4.773s         | 6.480s     |
| sproto-Csharp(unpack) | 2.550s | 5.207s     |
| [protobuf-net](https://github.com/mgravell/protobuf-net) | 8.768s | 10.825s |







