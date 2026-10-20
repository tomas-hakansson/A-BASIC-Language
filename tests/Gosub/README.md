Build Gosub.csproj with Visual Studio MSBuild and run bin/Debug/Gosub.exe.

Checks cover GOSUB/GO SUB, ON dispatch, expressions, nested stack order, the exact
return instruction, IF branches, missing targets, invalid arguments and syntax,
and the absence of stack pushes for GOTO or ON fallthrough. Until RETURN is added,
tests inspect the private call stack to verify the saved positions.
