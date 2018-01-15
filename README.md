<div align="center">
  <a href="https://github.com/MahApps/MahApps.Metro">
    <img alt="MahApps.Metro" width="200" heigth="200" src="https://image.flaticon.com/icons/svg/51/51650.svg">
  </a>
  <h1>Proxier</h1>
    
[![GitHub license](https://img.shields.io/github/license/redbaty/Proxier.svg)](https://github.com/redbaty/Proxier/blob/master/LICENSE) [![Twitter](https://img.shields.io/twitter/url/https/github.com/redbaty/Proxier.svg?style=social)](https://twitter.com/intent/tweet?text=Wow:&url=https%3A%2F%2Fgithub.com%2Fredbaty%2FProxier) [![NuGet](https://img.shields.io/nuget/dt/Proxier.svg)](https://www.nuget.org/packages/Proxier/) [![Build status](https://ci.appveyor.com/api/projects/status/3v3da0um3oy8ul5k?svg=true)](https://ci.appveyor.com/project/redbaty/proxier)
</div>


## Introduction :information_source:

Have you ever had a class that is generated during the build process or at runtime but you need to add attributes to it? This is the solution! You can add attributes and even properties to classes at runtime by defining an extension class!

## Features

* 🐱‍👤 Out of the box support for Ninject!
* 🕛 0 configuration time, **just start using**.
*  ℹ️ Semantic extensions, easy to use.
* 🌎 .NET Core support
* ⚙️ Fluent engine

## Code Samples :pencil2:

Say that you are using NSwag to generate swagger based classes, but they change a lot and you are worried that you forget to add some attributes to it and decide to use this library:

First you'll install the NuGet package
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

and then these properties will be added to a static mapper list, the `Mapper.TypesOverrides` with all the current extensions. To convert a object from the "MyClass" **type** to the injected type you would call:

```cs
typeof(MyClass).GetInjectedType();
```

To get a injected **object** you can use:

```cs
var InjectedMyClassObject = MyClassObject.GetInjectedObject();
```

Note that you can convert the inject object back to the original type by calling

```cs
InjectedMyClassObject.CopyTo(MyClassObject);
```

and all the common properties will be overriden by the ones on the injected type.

#### Fluent extensions
As of version 1.5.x fluent extensions are also supported. [You can check them out on the test class](https://github.com/redbaty/Proxier/blob/master/Proxier.Tests/PropertyTests.cs)


#### Dependency Injection

You can pass a Kernel to the Mapper initializer like this:

```cs
Mapper.InitializeMapperClasses(MyNinjectKernel);
```

and use the default `[Inject]` attribute from Ninject to get properties inside the extension class which in turn can add those into its current extended type. 

## Built With :wrench:

* [AttributeBuilder](https://github.com/michielvoo/Attribute-Builder) - Used to generate attributes, converted to .NET Standard

## License :books:

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE) file for details

*This documentation is incomplete and may change overtime.*
