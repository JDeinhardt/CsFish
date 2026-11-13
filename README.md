# CsFish

A chess engine coded in C# <br>
This is a hobby project and not ment to compete with top engines like Stocksich.

## UCI / Commands

The engine is UCI compatible and can handle the following commands:

- 'uci' - identify the engine
- 'isready'
- 'ucinewgame' reset the Board state and clear transposition table
- 'position startpos [moves...]' - set the starting position with optional move list
- 'position fen <FEN> [moves...]' - load a position from a FEN string with optional move list
- 'go depth x' - search to a fixed depth
- 'go movetime x' - search for a given time
- 'go wtime x btime x winc x binc x' - search with time management
- 'quit' - stops the engine and exits

Custom debugging uci commands include:

- 'printboard' - prints the board to the console
- 'isenpassantavailable' - prints if an en passant capture is available
- 'boardtofen' - prints the current board position as a FEN string
- 'isendgame' - prints wether the engine has switched into endgame-mode

## How to Play

You can download the engine from this github repo and use it with a chess GUI of your choice, for example Arena or Cutechess.
