# .NET Core Tools

Some filters, middleware, classes I use regularly in projects. Made so I can reuse these and also to learn how GitHub packages works.also

## What's inside

- ModelStateIsValidAttribute: an attribute that can be added to Controllers/Actions. Controllers decorated with the attribute will return a 400 with errors if the ModelState is not valid.
- ApplicationErrorHandlerMiddleware: middleware that returns an appropriate response for all exceptions thrown in the application.

## Installing

See https://help.github.com/en/packages/using-github-packages-with-your-projects-ecosystem/configuring-dotnet-cli-for-use-with-github-packages.also

## Publishing a new version

- Bump the version of the package in the .csproj file
- `dotnet pack --Configuration Release`
- `dotnet nuget push "ThomasDeWulf.Tools.Core/bin/Release/ThomasDeWulf.Tools.Core.<VERSION>.nupkg" --source "github"`

