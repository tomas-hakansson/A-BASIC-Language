# Genomgång av rättningarna

Ändringarna använder fortsatt NuGet-paketet TerminalMatrix 2.7.3. Ingen lokal projektreferens till TerminalMatrix behövs.

## Läsordning

Alla källkodsnamn nedan är relativa till `A-BASIC-Language/A-BASIC-Language`.

1. `Language/BasicSession.cs` och `MainWindow.cs`: LIST och NEW hanteras som direktkommandon. RUN hämtar det lagrade programmet. Övrig inmatning parsas i direktläge. Sessionsvariabler lever kvar mellan direktkommandon och nollställs av RUN/NEW. Ett skydd mot återinträde hindrar meddelandepumpen från att starta en andra tolk mitt under körningen.
2. `Language/Parsing/BasicParser.cs`, `Language/Parser.cs` och `Language/ParseResult.cs`: direktläge får en intern rad 0; radslut normaliseras till LF. Parsningen stannar inte i en oavslutad sträng. Fel gör resultatet ogiltigt och följer med till användaren. Ingen del av ett program med syntaxfel exekveras. Parsern stöder nu unärt plus/minus och FOR/NEXT med TO och STEP.
3. `Language/Interpreter.cs`: de motsägande körlägeskontrollerna är borttagna. Förväntade körfel avbryter exekveringen och visas i terminalen. FOR/NEXT använder matchade instruktioner och en stack för nästlade loopar; nollsteg och omatchade loopar rapporteras som fel. Gräns och steg beräknas när FOR startar.
4. `Language/IBasicTerminal.cs` och `Language/TerminalAdapter.cs`: tolken behöver bara ett litet textgränssnitt. Adaptern håller reda på utskriftskolumnen och radbryter vid terminalens bredd. Därmed skriver PRINT-delar efter varandra, i stället för att skriva över varandra. Komma i PRINT går till nästa 14-kolumnszon.
5. `Language/ValueTypes/ValueBase.cs` och `Language/SpecificExecutors`: tilldelning validerar och konverterar värdet före skrivning till variabeln eller arrayen. Typfel lämnar tidigare värde oförändrat och stoppar programmet. INPUT till en strängvariabel behåller exempelvis inledande nollor.
6. `Language/Dimension.cs`: form, antal index och varje dimensions gränser kontrolleras innan den platta adressen beräknas. Ett ogiltigt index kan inte längre peka på ett annat giltigt element.
7. `MainWindowControllers/PreProcessorController.cs`: !QUIT använder en metod på fönstret för att stänga av bekräftelsen. Release refererar därför inte längre till ett fält i fel klass. Fönsterstängning sätter även terminalens QuitFlag.

## Verifiering

Testprojektet ligger i `A-BASIC-Language/A-BASIC-Language.Tests` och ingår i lösningen.

```powershell
dotnet test A-BASIC-Language/A-BASIC-Language.sln -c Debug
dotnet test A-BASIC-Language/A-BASIC-Language.sln -c Release
```

`InterpreterTests.cs` testar parser, körning, fel, sessionsvariabler, avbrott, loopar och arraygränser med en terminal i minnet. `TerminalIntegrationTests.cs` kör den riktiga Windows-kontrollen på en STA-tråd, inklusive kedjan tangenttryckning → lagrad programrad → LIST → RUN. Ett tidsbegränsat test täcker den tidigare parserlåsningen.

## Enkel manuell kontroll

```basic
10 FOR I=1 TO 3
20 PRINT "VARV ";I
30 NEXT I
LIST
RUN
```

Testa också `PRINT -1`, `PRINT "HELLO";"WORLD"` och en oavslutad sträng i en numrerad rad följd av RUN. Det sistnämnda ska ge syntaxfel utan att låsa fönstret.

Rättningarna innebär inte fullständig Altair BASIC-kompatibilitet. De tidigare påtalade exemplen med negativa tal och FOR/NEXT stöds nu; andra ännu ej implementerade språkdelar, exempelvis GOSUB/RETURN och DATA/READ, återstår. De ska ge felmeddelanden i stället för tysta misslyckanden.
