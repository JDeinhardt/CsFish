# CsFish

A chess engine coded in C# <br>
This is a hobby project and not meant to compete with top engines like Stockfish.

A demonstration video of the engine playing against itself (start is cut off because the engine used an opening book and played the moves almost instantly):

https://github.com/user-attachments/assets/cd17eb78-0d7a-4c81-9aab-488c78884dcf

## Features
- legal move generation including all special moves (castling, en passant, promotion)
- uci compatible
- NegaMax Search with alpha-beta-pruning
- time controls
- opening book
- transposition-tables

## Perft
The legal move generation is validated using PERFT. 
The (expected) results are:

Perft(1): 20 4ms <br>
Perft(2): 400 0ms <br>
Perft(3): 8902 6ms <br>
Perft(4): 197281 132ms <br>
Perft(5): 4865609 2872ms <br>
Perft(6): 119060324 67371ms

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
- 'isendgame' - prints whether the engine has switched into endgame-mode

## How to Play

You can download the engine from this github repo and use it with a chess GUI of your choice, for example Arena or Cutechess.
