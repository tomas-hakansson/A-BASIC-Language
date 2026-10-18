# ON GOTO regression checks

Build with Visual Studio MSBuild from the repository root:

```powershell
MSBuild tests/OnGoto/OnGoto.csproj /p:Configuration=Debug
./tests/OnGoto/bin/Debug/OnGoto.exe
```

The executable checks computed jumps, fallthrough after a colon, numeric expressions,
arrays, built-in and user-defined functions, IF branches, FOR loops, runtime errors,
invalid target lists, and rejection in direct mode. It uses an in-memory terminal.
