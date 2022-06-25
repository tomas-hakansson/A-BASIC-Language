// See https://aka.ms/new-console-template for more information

var pathToMain = Path.GetFullPath(args[0]);
Interpreter eval = new(pathToMain);
eval.Run();
