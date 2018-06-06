System.CommandLine
==================

This repository contains the code for System.CommandLine, an API for parsing command line arguments in .NET console applications.

## Why another command line parser?

Several command line parsers are already available, so why create another? The overall goal is: 

_Result in the user of the application or tool having a very good experience, without the author of the application or tool needing work very hard or know very much about command line parsing_

* Clean separation of CLI definition from parsing as separate concerns.
* Definition of expected arguments (CLI) that is simple for simple cases, and understandable for complex cases.
  * Support for subcommands.
* Parsing into strongly typed results.
* Support for multiple command line parser app models (different ways to define expected arguments).

To support the end user:

* Automatic creation of consistent Help and, depending on the app model, display of Help.
* Automatic creation of tab completion.
* Flexibility in how options are entered, especially for help.

Console applications and global tools vary in the complexity of the command line arguments. In non-trivial cases, the effort to properly manage command line arguments may exceed the effort the other aspects of the application or tool. This tool is built on the experience of several command line parsers and largely based on the .NET CLI 2.0 command parser. Since it is expected to replace the CLI command parser, it must handle very complex and evolving CLIs. A core part of the design was to avoid complicating the simple cases to accommodate the complex ones. This is done through simplification of the API and through simple app model approaches.

## Overall structure

There are two aspects of this command line parser - the central CommandLineParser and optional app models. The code name for the 

### Contributing

See the [Contributing guide](CONTRIBUTING.md) for developer documentation.

## License

This project is licensed under the [MIT license](LICENSE.TXT).

## .NET Foundation

.NET is a [.NET Foundation](http://www.dotnetfoundation.org/projects) project.

