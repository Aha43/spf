# Changelog

All notable changes to this project will be documented in this file.

## [1.0.0] - Initial Release
### Added
- Core framework for handling command-based prompt applications.
- Automatic discovery and registration of `ISpfPromptHandler` implementations.
- Namespace-based routing for structured commands.
- Support for dependency injection in handlers.
- Custom exit behavior via `ISpfExitor`.
- Custom handling of unknown commands via `ISpfNoPromptMatchHandler`.
- Verbose mode (`--verbose`) for debugging output.
- Makefile with common targets (`build`, `test`, `run`, `format`, etc.).
- Initial README with usage instructions.
- MIT License.
