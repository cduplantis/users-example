# Example CLI
The original version of this was build as a singular file. It is a decent example of how to write a simple utility with embedded unit testing.


## Setting up the development container

Follow these steps to open this sample in a container:

1. Open a locally cloned copy of the code:

   - Clone this repository to your local filesystem.
   - Press <kbd>F1</kbd> and select the **Remote-Containers: Open Folder in Container...** command.
   - Select the cloned copy of this folder, wait for the container to start.


## Requirements

- Dotnet Core 3.1

**Recommended**

- VS Code


## To Run

One you have this project opened in a container, you'll be able to work with it like you would locally. Optionally, you may run this from your host/local machine.

> **Note:** This container runs as a non-root user with sudo access by default. Comment out `"remoteUser": "vscode"` in `.devcontainer/devcontainer.json` if you'd prefer to run as root.



1. **Restore Packages:** When notified by the C# extension to install packages, click Restore to trigger the process from inside the container. Alternately, press <kbd>ctrl</kbd>+<kbd>shift</kbd>+<kbd>\`</kbd> and enter `dotnet restore` in the terminal window.

1. **Run Tests:** When notified by the C# extension to install packages, click Restore to trigger the process from inside the container. Alternately, press <kbd>ctrl</kbd>+<kbd>shift</kbd>+<kbd>\`</kbd> and enter `dotnet restore` in the terminal window.
   - Press <kbd>Ctl-Shift-D</kbd> to open the Run pannel.
   - Choose the`.NET Core Test (console)` configuration with will pass `test` as an argument to starting application.
   - Press <kbd>CTL-F5</kbd> to launch the app in the container.
   - Alternatively, press <kbd>F5</kbd> to launch the app in the container to debug the tests.

> **Note:** Unit tests of the applicatin do throw tests. If you have enabled the debugger to break on All Exceptions, the application will halt on the throwing test when running via the debugger.

1. **Run:**
   - Press <kbd>Ctl-Shift-D</kbd> to open the Run pannel.
   - Choose the`.NET Core Launch (console)` configuration.
   - Press <kbd>CTL-F5</kbd> to launch the app in the container.
   - Alternatively, press <kbd>F5</kbd> to launch the app in the container to debug.

Licensed under the MIT License. See LICENSE in the project root for license information.

## Contributing

This project welcomes contributions and suggestions. Policy for submissions, Terms of Use and Code of Conduct may change and will be updated in the corresponding repo.


## License

Copyright © Casey Duplantis All rights reserved.<br />
