# A BASIC Language

ABL (A BASIC Language) is (will be) an implementation of Altair BASIC by Microsoft, with some extra commands.

[Read the manual!](https://abl.winsoft.se/)

WINDOW works both as a direct command and in numbered programs. It clears the screen before changing the text window. With no argument it
restores all 25 text rows. With an argument it uses that many rows at the bottom:
`WINDOW 10` calls `SetTextRenderLimit(15)`. The argument can be a numeric expression
and must evaluate to a whole number from 2 to 25. NEW and RUN also clear the screen and restore all 25 text rows when they reset variables.
It's currently an interpreter but compilation is a possibility in the future.

The initial goal is to be able to run all games in the book [101 BASIC Computer Games](http://www.bitsavers.org/pdf/dec/_Books/101_BASIC_Computer_Games_Mar75.pdf) (1975).

Of the 101 games, we support the ones followed by a 1:

| Game              | Supported |
|-------------------|-----------|
| 23matches         |     1     |
| 3dplot            |     1     |
| aceyducey         |     1     |
| amazing           |     0     |
| animal            |     0     |
| awari             |     0     |
| bagels            |     0     |
| banner            |     0     |
| basketball        |     0     |
| batnum            |     0     |
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
| cube              |           |
| depthcharge       |           |
| diamond           |           |
| dice              |           |
| digits            |     0     |
| evenwins          |           |
| flipflop          |           |
| football          |           |
| ftball            |           |
| furtrader         |           |
| gameofevenwins    |           |
| golf              |           |
| gomoko            |           |
| guess             |           |
| gunner            |           |
| hammurabi         |           |
| hangman           |           |
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
| mathdice          |           |
| mugwump           |           |
| name              |     0     |
| nicomachus        |     1     |
| nim               |           |
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
| synonym           |           |
| target            |           |
| tictactoe1        |           |
| tictactoe2        |           |
| tower             |           |
| train             |           |
| trap              |           |
| war               |           |
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
