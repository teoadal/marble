# Marble

[![.NET Core](https://github.com/teoadal/marble/workflows/.NET%20Core/badge.svg?branch=master)](https://github.com/teoadal/marble/actions)
[![codecov](https://codecov.io/gh/teoadal/marble/branch/master/graph/badge.svg)](https://codecov.io/gh/teoadal/marble)
[![NuGet](https://img.shields.io/nuget/v/marble.svg)](https://www.nuget.org/packages/marble) 
[![NuGet](https://img.shields.io/nuget/dt/marble.svg)](https://www.nuget.org/packages/marble)

Based on [MediatR](https://github.com/jbogard/MediatR).

Three times faster than MediatR. Infrastructure should not slow down.

## Performance

### Five request handlers, behaviour, two pre processors and one post processor

|  Method |     Mean |    Error |   StdDev | Ratio |   Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------- |---------:|---------:|---------:|------:|--------:|------:|------:|----------:|
| MediatR | 86.66 μs | 0.462 μs | 0.386 μs |  1.00 | 12.2070 |     - |     - |  75.31 KB |
|  Marble | 21.17 μs | 0.068 μs | 0.053 μs |  **0.24** |  1.4038 |     - |     - |   8.74 KB |

### One request handler

|  Method |      Mean |     Error |    StdDev | Ratio |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------- |----------:|----------:|----------:|------:|-------:|------:|------:|----------:|
| MediatR | 11.893 us | 0.1340 us | 0.1253 us |  1.00 | 2.3346 |     - |     - |  14.37 KB |
|  Marble |  1.702 us | 0.0085 us | 0.0075 us |  **0.14** | 0.1640 |     - |     - |   1.01 KB |

#### Notification handlers

|  Method | HandlersCount |      Mean |     Error |    StdDev | Ratio |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------- |-------------- |----------:|----------:|----------:|------:|-------:|------:|------:|----------:|
| MediatR |             5 |  6.778 us | 0.1116 us | 0.1044 us |  1.00 | 1.1292 |     - |     - |    7112 B |
|  Marble |             5 |  4.262 us | 0.0298 us | 0.0264 us |  **0.63** | 0.1068 |     - |     - |     712 B |
|         |               |           |           |           |       |        |       |       |           |
| MediatR |            10 | 10.821 us | 0.1403 us | 0.1313 us |  1.00 | 1.8921 |     - |     - |   11912 B |
|  Marble |            10 |  6.767 us | 0.0439 us | 0.0411 us |  **0.63** | 0.1755 |     - |     - |    1112 B |
|         |               |           |           |           |       |        |       |       |           |
| MediatR |            20 | 19.142 us | 0.2809 us | 0.2627 us |  1.00 | 3.4180 |     - |     - |   21512 B |
|  Marble |            20 | 11.999 us | 0.1088 us | 0.0965 us |  **0.63** | 0.2899 |     - |     - |    1912 B |


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

