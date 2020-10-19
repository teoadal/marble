# Marble

[![.NET Core](https://github.com/teoadal/marble/workflows/.NET%20Core/badge.svg?branch=master)](https://github.com/teoadal/marble/actions)
[![codecov](https://codecov.io/gh/teoadal/marble/branch/master/graph/badge.svg)](https://codecov.io/gh/teoadal/marble)
[![NuGet](https://img.shields.io/nuget/v/marble.svg)](https://www.nuget.org/packages/marble) 
[![NuGet](https://img.shields.io/nuget/dt/marble.svg)](https://www.nuget.org/packages/marble)

Based on [MediatR](https://github.com/jbogard/MediatR).

Three times faster than MediatR. Infrastructure should not slow down.

## Performance

|  Method |     Mean |    Error |   StdDev | Ratio |   Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------- |---------:|---------:|---------:|------:|--------:|------:|------:|----------:|
| MediatR | 86.66 μs | 0.462 μs | 0.386 μs |  1.00 | 12.2070 |     - |     - |  75.31 KB |
|  Marble | 21.17 μs | 0.068 μs | 0.053 μs |  0.24 |  1.4038 |     - |     - |   8.74 KB |

Five request handlers, behaviour, two pre processors and one post processor

## Install from nuget

Install Marble with the following command [from nuget](https://www.nuget.org/packages/marble/):

```ini
Install-Package Marble
```

## Add mediator

Scans assemblies to add handlers, preprocessors, behaviours and postprocessors implementations to the container. 
To use, with an `IServiceCollection` instance:

```cs
services.AddMediator(typeof(MyHandler).Assembly)
```

### Add concrete mediator parts

```cs
services.AddMediator(typeof(MyHandler), typeof(MyPreProcessor), typeof(YourHandler))
```

### Scan registered services with custom request registration

```cs
services.AddMediator(options => options.RegisterRequest<MyRequest>())
```

