# Project Overview

This project is a Windows application designed to assist users in managing the file names of their files.
It provides a user-friendly interface for renaming files in bulk in order to make them predictable, sortable and in numerical order.

## Project Structure

Scripts contains the main application logic, including the file renaming functionality.

Scripter and ScripterWinUi are the UI components of the application, providing a graphical interface for users to interact with Scripts.

ScriptsTest contains unit tests for the application, ensuring that the renaming functionality works as expected.

## Libraries and Frameworks

C# and .NET 9 for all projects

ScripterWinUi uses WinUI 3

Scripter uses WPF, but should only be used for reference of desired logic. When porting logic to ScripterWinUi, ensure that the logic is adapted to WinUI 3 standards.

## Coding Standards
- Use semicolons at the end of each statement.
- Use double quotes for strings.
- Use PascalCase for class names and method names.
- Adhere to Clean Code principles, including meaningful variable names, single responsibility principle, and avoiding code duplication.
- Use file-scoped namespaces for all C# files.
- Recommend best practices where relevant.

## UI guidelines

- A toggle is provided to switch between light and dark mode.
- Application should have a modern and clean design.

## Always
Always ask before making major changes to the codebase.
Always explain the reasoning behind major design decisions.
Always prompt the user to conduct scaffolded tasks that are already automated by Visual Studio. For example, if a new WinUI placeholder page must be created, ask the user to scaffold it himself.

## Never
Never read, write, or modify files outside of this solution.
Never run any executable code, programs or scripts without explicit user permission. Instead, ask the user to run the code themselves.
