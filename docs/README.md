# Building Documentation
The documentation can be built using the docfx tool.
```sh
dotnet tool update -g docfx
docfx docfx.json
```
Documentation is generated from xml code comments and combined with the files located in the */docs* folder to produce html based documentation. Output is collected in the *_site/* folder. Open "*index.html*" to view the generated documentation.

## Tooling & Directories
Tool dependencies are installed/updated as follows:
- docfx - documentation generation tool
- docs\docfx.json
    - Documentation
