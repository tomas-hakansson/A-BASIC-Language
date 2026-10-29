# A BASIC Language

ABL (A BASIC Language) is (will be) an implementation of Altair BASIC by Microsoft, with some extra commands.

[Read the manual!](https://abl.winsoft.se/)

WINDOW works both as a direct command and in numbered programs. It clears the screen before changing the text window. With no argument it
restores all 25 text rows. With an argument it uses that many rows at the bottom:
`WINDOW 10` calls `SetTextRenderLimit(15)`. The argument can be a numeric expression
and must evaluate to a whole number from 2 to 25. NEW and RUN also clear the screen and restore all 25 text rows when they reset variables.
GOSUB (also spelled GO SUB) jumps to a line and pushes the next instruction and
calling line onto a separate subroutine call stack. ON expression GOSUB line1,line2,...
selects a subroutine using the same rules as ON GOTO. RETURN pops the latest subroutine call and resumes at the instruction after GOSUB. A RETURN with no active subroutine reports RETURN without GOSUB.
It's currently an interpreter but compilation is a possibility in the future.

The initial goal is to be able to run all games in the book [101 BASIC Computer Games](http://www.bitsavers.org/pdf/dec/_Books/101_BASIC_Computer_Games_Mar75.pdf) (1975).

Of the 101 games, we support the ones followed by a 1:

| Game              | Supported |
|-------------------|-----------|
| 23matches         |     1     |
| 3dplot            |     1     |
| aceyducey         |     1     |
| amazing           |     1     |
| animal            |     0     |
| awari             |     0     |
| bagels            |     0     |
| banner            |     0     |
| basketball        |     0     |
| batnum            |     1     |
| battle            |     0     |
| blackjack         |     0     |
| bombardment       |     0     |
| bombsaway         |     0     |
| bounce            |     0     |
| bowling           |     0     |
| boxing            |     0     |
| bug               |     0     |
| bullfight         |     0     |
| bullseye          |     0     |
| bunny             |     0     |
| buzzword          |     0     |
| calendar          |     0     |
| change            |     1     |
| checkers          |     0     |
| chemist           |     1     |
| chief             |     0     |
| chomp             |     0     |
| civilwar          |     0     |
| combat            |     1     |
| craps             |     1     |
| cube              |     0     |
| depthcharge       |     0     |
| diamond           |     0     |
| dice              |     1     |
| digits            |     0     |
| evenwins          |     1     |
| flipflop          |     0     |
| football          |     0     |
| ftball            |     0     |
| furtrader         |     0     |
| gameofevenwins    |     1     |
| golf              |     0     |
| gomoko            |     0     |
| guess             |     1     |
| gunner            |     0     |
| hammurabi         |     0     |
| hangman           |     0     |
| hello             |           |
| hexapawn          |           |
| hi-lo             |           |
| highiq            |           |
| hockey            |           |
| horserace         |           |
| hurkle            |           |
| kinema            |           |
| king              |           |
| lem               |           |
| letter            |           |
| life              |           |
| lifefortwo        |           |
| litquiz           |           |
| love              |           |
| lunar             |           |
| mastermind        |     0     |
| mathdice          |     1     |
| mugwump           |     1     |
| name              |     0     |
| nicomachus        |     1     |
| nim               |     0     |
| number            |           |
| onecheck          |           |
| orbit             |           |
| pizza             |           |
| poetry            |           |
| poker             |           |
| qubic             |           |
| queen             |           |
| reverse           |           |
| rocket            |           |
| rickscissors      |           |
| roulette          |           |
| russianroulette   |           |
| salvo             |           |
| sinewave          |           |
| slalom            |           |
| slots             |           |
| splat             |           |
| stars             |           |
| stockmarket       |           |
| superstartrek     |           |
| superstartrekins  |           |
| synonym           |     0     |
| target            |     0     |
| tictactoe1        |     1     |
| tictactoe2        |     1     |
| tower             |     0     |
| train             |     1     |
| trap              |     0     |
| war               |     0     |
| weekday           |     0     |
| word              |     0     |

## Documentation

Documentation is located here: [https://abl.winsoft.se/](https://abl.winsoft.se/)

## Repository

[https://github.com/tomas-hakansson/A-BASIC-Language](https://github.com/tomas-hakansson/A-BASIC-Language)

This program uses the [TerminalPixelMatrixLibrary](https://github.com/Anders-H/TerminalPixelMatrixLibrary).

## Version history

### Version 1.0

- Emulates all games listed in the book 101 BASIC Computer Games (1975)
