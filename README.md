# VJson 🍣

> A JSON serializer/deserializer (with JsonSchema support) library written in pure C#.

[![CircleCI](https://circleci.com/gh/yutopp/VJson.svg?style=svg)](https://circleci.com/gh/yutopp/VJson)
[![npm](https://img.shields.io/npm/v/net.yutopp.vjson)](https://www.npmjs.com/package/net.yutopp.vjson)
[![NuGet Badge](https://buildstats.info/nuget/vjson)](https://www.nuget.org/packages/VJson/)
[![codecov](https://codecov.io/gh/yutopp/VJson/branch/master/graph/badge.svg)](https://codecov.io/gh/yutopp/VJson)
[![license](https://img.shields.io/github/license/yutopp/VJson.svg)](https://github.com/yutopp/VJson/blob/master/LICENSE_1_0.txt)

`VJson` is a JSON serializer/deserializer (with JsonSchema support) library written in pure C#. Supported versions are `.NET Standard 2.0` or higher.  
This library is developed as a purely C# project, however it also supports that be build with `Unity 2019.4.22f1` or higher.

## Installation

### For standard C# projects

You can use [Nuget/VJson](https://www.nuget.org/packages/VJson/).

```bash
dotnet add package VJson
```

### For Unity projects

#### stable

Add scoped registry information shown below to your `Packages/manifest.json` if not exists.

```json
{
  "scopedRegistries": [
    {
      "name": "yutopp.net",
      "url": "https://registry.npmjs.com",
      "scopes": [
        "net.yutopp"
      ]
    }
  ]
}
```

And add `net.yutopp.vjson` to your `Packages/manifest.json` like below.

```json
{
  "dependencies": {
    "net.yutopp.vjson": "*"
  }
}
```

#### nightly

Add a url for VJson git repository to your `Packages/manifest.json` like below.

```json
{
  "dependencies": {
    "net.yutopp.vjson": "https://github.com/yutopp/VJson.git?path=Packages/net.yutopp.vjson"
  }
}
```

(TODO: Provide unity packages)

## Usage example

### Serialize/Deserialize

```csharp
//
// For serialization
//
var serializer = new VJson.JsonSerializer(typeof(int));

// You can get JSON strings,
var json = serializer.Serialize(42);

// OR write json data(UTF-8) to streams directly.
using (var s = new MemoryStream())
{
    serializer.Serialize(s, 42);
}
```

```csharp
//
// For deserialization
//
var serializer = new VJson.JsonSerializer(typeof(int));

// You can get Object from strings,
var value = serializer.Deserialize(json);

// OR read json data(UTF-8) from streams directly.
using (var s = new MemoryStream(Encoding.UTF8.GetBytes(json)))
{
    var value = serializer.Deserialize(s);
}
```

`VJson` supports serializing/deserializing of some `System.Collections(.Generics)` classes listed below,

- List<T>
- Dictionary<string, T>
- Array

and user defined classes. For user defined classes, converting only all public fields are supported.

e.g.

(It is strongly recommended to always add VJson attributes such as [JsonField] to fields that you want to treat as Json. This will avoid problems with IL2CPP, especially when using Unity.)

```csharp
class SomeObject
{
    private float _p = 3.14f;    // Fields which are non-public will not be exported by default.
    [JsonField] long _p2 = 4;    // Fields which are non-public BUT having [JsonField] (+etc) attributes will BE exported!
    public int X;                // Fields which are public will be exported by default, but we strongly recommended to add [JsonField] attributes like below.
    [JsonField] public string Y;
}

var obj = new SomeObject {
    X = 10,
    Y = "abab",
},

var serializer = new VJson.JsonSerializer(typeof(SomeObject));
var json = serializer.Serialize(obj);
// > {"_p2": 4,"X":10,"Y":"abab"}

var v = serializer.Deserialize("{\"X\":10,\"Y\":\"abab\"}");
// > v becomes same as obj.
```

#### Attributes

...

### JSON Schema and validation

`VJson` supports JSON Schema draft7.

(TODO: Write examples)

#### Attributes

(TODO: Write examples)

Other use cases are available at [here](https://github.com/yutopp/VJson/tree/master/Assets/VJson/Editor/Tests).

## Tasks

- [ ] Performance tuning

## License

[Boost Software License - Version 1.0](./LICENSE_1_0.txt)

## References

- [JSON](https://www.json.org/)
- [JSON Schema Specification](https://json-schema.org/specification.html)

## Author

- [@yutopp](https://github.com/yutopp)
