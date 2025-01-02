### Based on exercise
https://learn.microsoft.com/en-us/training/modules/implement-observability-cloud-native-app-with-opentelemetry/4-exercise-add-observability-cloud-native-app

### Add OpenTelemetry to a cloud-native application

Add a diagnostic project to the solution

Add OpenTelemetry packages
```bash
dotnet add package OpenTelemetry.Exporter.Console
dotnet add package OpenTelemetry.Extensions.Hosting
dotnet add package OpenTelemetry.Instrumentation.AspNetCore
dotnet add package OpenTelemetry.Instrumentation.EventCounters --prerelease
dotnet add package OpenTelemetry.Instrumentation.Runtime
dotnet add package OpenTelemetry.Instrumentation.SqlClient --prerelease
dotnet add package OpenTelemetry.Instrumentation.Http
```

Add the code to use OpenTelemetry

Add Diagnostics project reference in Products

In program.cs add the following code
```csharp
builder.Services.AddObservability("Products", builder.Configuration);
```

build
