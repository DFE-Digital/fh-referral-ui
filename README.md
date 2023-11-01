# fh-referral-ui

Ui for the management of a person/family referral to a Local Authority, Voluntary, Charitable or Faith organisation.

# fh-service-referral-ui

## Requirements

* .Net 7.0 and any supported IDE for DEV running.

## About

The Family Hubs service referral allows users to search for local authorities, voluntary, charitably, and faith organisations for vulnerable children and families.

This repos has been built using the "Clean Architecture Design" taken from Steve Smith (ardalis)


## Local running

You need fh-service-directory-api repos running locally

## CSS & JavaScript

Run this command in the root of the project to install the required node modules:

```
npm install
```

Once the packages have installed, the next time Visual Studio is run, the CSS and JavaScript files will be generated when a source file is saved.

### Regenerate the CSS

The CSS is generated from the SASS (.scss) files. If you are using Visual Studio, any changes to the SASS files should automatically trigger the compilation and minification of the CSS files. In Rider, follow these [instructions](https://www.jetbrains.com/help/rider/Using_Gulp_Task_Runner.html#ws_gulp_running_tasks_from_tasks_tree).

To manually generate and minify the CSS files, run the `sass-to-min-css` gulp task, or run the `sass-to-min-css:watch` gulp task to initiate a watcher that will automatically recompile the CSS files when the SASS files are changed. Gulp tasks can be run manually from the Task Runner Explorer window.

The entry point for the site's SASS is `styles\scss\application.scss`.

## Regenerate the JavaScript

In Visual Studio, any changes to the Typescript (or JavaScript) files, should automatically trigger the transpiling, bundling and minification of the Javascript files. In Rider, follow these [instructions](https://www.jetbrains.com/help/rider/Using_Gulp_Task_Runner.html#ws_gulp_running_tasks_from_tasks_tree).

The bundling process supports the use of ECMAScript modules.

To manually transpile, bundle and minify the js files, run the `js` gulp task, or run the `js:watch` gulp task to initiate a watcher that will automatically run the process when the ts/js files are changed. Gulp tasks can be run manually from the Task Runner Explorer window.

The entry point for the site's JavaScript is `scripts\app.ts`.

## Debugging the JavaScript in Visual Studio

To debug the JavaScript in Visual Studio, set breakpoints in the JavaScript files under the `Script documents` folder in the Solution Explorer when debugging.

(Note, we might switch to [environment-based bundling and minification at a later point](https://learn.microsoft.com/en-us/aspnet/core/client-side/bundling-and-minification?view=aspnetcore-6.0).)

## TODO

* ConnectCache not being created

* open / close filter button doesn't work - create a component from the find site.

* journey edge case

* sign-in expiry (cookie expires with session, but no session)

* add prg to telephone, text & letter & switch to standard ErrorState

* change ErrorId to static const class with const ints?
