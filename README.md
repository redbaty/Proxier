# Proxier

## Introduction

Have you ever had a class that is generated at build but you need to add attributes to it? This is the solution! You can add attributes and even properties to classes at runtime by defining an extension class!

## Code Samples

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

*This documentation is incomplete and may change overtime.*
