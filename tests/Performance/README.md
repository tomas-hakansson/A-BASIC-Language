Build Performance.csproj with Visual Studio MSBuild (Debug), then run from the repository root:

    tests\Performance\bin\Debug\Performance.exe A-BASIC-Language\A-BASIC-Language\Games\Source\amazing.bas

Checks compare TerminalAdapter output against the original character-at-a-time behavior,
including wrapping, CR/LF normalization, scrolling and cursor position. An endless BASIC
loop checks that event pumping still delivers a user break.

The benchmark runs AMAZING with 10x10 and 20x15 dimensions and reports parsing time,
total time and event-pump counts. It uses a text-recording terminal with real WinForms
event pumping and a status label attached to a StatusStrip. It excludes GUI terminal
rendering and human input time. Instruction tracing is disabled, as in the normal UI.
AMAZING uses random maze generation, so work and timings vary between runs; these
measurements are diagnostic, not timing assertions.

Example Debug measurements on the same machine before/after this change:
10x10: 2033 ms before, 84-130 ms after.
20x15: 8857 ms before, 61-76 ms after.
