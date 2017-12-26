# Proxier

[![GitHub license](https://img.shields.io/github/license/redbaty/Proxier.svg)](https://github.com/redbaty/Proxier/blob/master/LICENSE) [![Twitter](https://img.shields.io/twitter/url/https/github.com/redbaty/Proxier.svg?style=social)](https://twitter.com/intent/tweet?text=Wow:&url=https%3A%2F%2Fgithub.com%2Fredbaty%2FProxier) [![NuGet](https://img.shields.io/nuget/dt/Proxier.svg)](https://www.nuget.org/packages/Proxier/) [![VSTS](https://redbaty-ci-builds.visualstudio.com/_apis/public/build/definitions/f59de2e3-c972-498b-9cac-f0c489a7fef1/4/badge)]()

## Introduction :information_source:

Have you ever had a class that is generated at build but you need to add attributes to it? This is the solution! You can add attributes and even properties to classes at runtime by defining an extension class!

## Features

* 🐱‍👤 Out of the box support for Ninject!
* 🕛 0 configuration time required, just start using.
* :information_source: Semantic extensions, easy to use.

## Code Samples :pencil2:

Say that you are using NSwag to generate swagger based classes, but they change a lot and you are worried that you forget to add some attributes to it and decide to use this library:

First you install the NuGet package
> Install-Package Proxier

Then you'll create a extension class like so:

```cs
    public class MyClassExtension : AttributeMapper<MyClass> {
        public MyClassExtension(){
            //Here you can add any class attributes, properties attributes and even properties!
            AddPropertyAttribute(u => u.Business, () => new MyAttribute());
            AddProperty("MyProperty", typeof(string));
            AddClassAttribute(() => new MyAttribute());
        } 
    }
```

and then you these properties will be added to a static list with all the current extensions, you can convert the object from the "MyClass" type to the injected type by calling:

```cs
typeof(MyClass).GetInjectedType();
```

To get the injected type, or to get a injected object you can use:

```cs
var InjectedMyClassObject = MyClassObject.GetInjectedObject();
```

Note that you can convert the inject object back to the original type by calling

```cs
InjectedMyClassObject.CopyTo(MyClassObject);
```

and all the common properties will be overriden by the ones on the injected type.

## Built With :wrench:

* [AttributeBuilder](https://github.com/michielvoo/Attribute-Builder) - Used to generate attributes, converted to .NET Standard

## License :books:

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE) file for details

*This documentation is incomplete and may change overtime.*
