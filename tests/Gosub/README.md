Build Gosub.csproj with Visual Studio MSBuild and run bin/Debug/Gosub.exe.

Checks cover GOSUB/GO SUB, ON dispatch, expressions, nested stack order, the exact
return instruction, IF branches, missing targets, invalid arguments and syntax,
and the absence of stack pushes for GOTO or ON fallthrough. Additional RETURN checks cover recursive and nested calls, same-line and next-line resumption, ON GOSUB, IF/ELSE, FOR loops, FN calls, empty stacks, and restored error line numbers.
Tests also inspect the private call stack to verify the saved positions.
