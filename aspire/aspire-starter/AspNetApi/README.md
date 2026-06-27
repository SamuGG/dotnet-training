# Description

This project serves as the backend API for the frontend projects.

It's a ASP.NET Core app exposing a single `/weatherforecast` endpoint; which returns an array of:

```json
{
    date: string,
    temperatureC: number,
    temperatureF: number,
    summary: string
}
```

Run it individually with `dotnet run` or with the rest of the resources with `aspire run`
