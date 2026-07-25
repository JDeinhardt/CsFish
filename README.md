# CsFish

**!! ENGINE STILL UNDER CONSTRUCTION !!**

A simple chess engine coded in C#. <br>
This is only a hobby project and is not made to compete with engines like Stockfish.

While this engine is still under active development, the following core features already exist (but may be updated):
- Move generation (with special moves like en-passant, castling, and all four promotion types)
- Draw handling (50-move rule and 3-fold repetition)
- Evaluation function with material + piece-square tables and endgame detection
- Search with iterative deepening, NegaMax, and alpha-beta pruning
- Transposition tables
- Opening book (58,000+ positions with weighted random selection)

## UCI / Commands

The engine is UCI compatible and can handle the following commands:
- `position startpos [moves ...]` — set the starting position with optional move list
- `position fen <fen> [moves ...]` — load a FEN with optional move list
- `go depth x` — search to a fixed depth
- `go movetime x` — search for a given time in milliseconds
- `go wtime x btime x winc x binc x` — clock-aware time management
- `uci` — identify the engine
- `isready` — synchronize
- `ucinewgame` — reset the board and clear the transposition table
- `quit` — exit the engine

Custom non-UCI debug commands include:
- `printboard` — print the current board state to the console
- `boardtofen` — print the current FEN to the console
- `isenpassantavailable` — display whether an en-passant capture is available
- `isendgame` — display whether the position is an endgame

## How to Play

You can play with it live on Lichess! [Here](https://lichess.org/@/CsFishBot) <br>
The Lichess bot only accepts bullet and blitz games.

Optionally, you can also download the engine yourself and play with it using a chess GUI like Arena.

## Building

Requires .NET 10 or later.

```bash
dotnet build
To publish a standalone build:
dotnet publish -c Release
