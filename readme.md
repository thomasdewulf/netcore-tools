# .NET Core Tools

Some filters, middleware, classes I use regularly in projects. Made so I can reuse these and also to learn how GitHub packages works.also

## What's inside

- ModelStateIsValidAttribute: an attribute that can be added to Controllers/Actions. Controllers decorated with the attribute will return a 400 with errors if the ModelState is not valid.
- ApplicationErrorHandlerMiddleware: middleware that returns an appropriate response for all exceptions thrown in the application.

## Installing the package

See https://help.github.com/en/packages/using-github-packages-with-your-projects-ecosystem/configuring-dotnet-cli-for-use-with-github-packages.also

## Publishing a new version

GithHub actions handles the publishing of a new version. Each push to the master branch triggers a build and release.
