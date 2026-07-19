using CsFish.Core;
using CsFish.MoveGeneration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CsFish.Helpers
{
    public static class Utils
    {
        public static readonly Dictionary<int, char> PieceTypeToSymbol = new()
        {
            { Piece.Pawn, 'p'}, { Piece.Rook, 'r' }, { Piece.Bishop, 'b' },
            { Piece.Knight, 'n' }, { Piece.King, 'k' }, { Piece.Queen, 'q' },
            { Piece.None, '.' }
        };

        public static readonly Dictionary<char, int> PieceTypeFromSymbol = new()
        {
            ['k'] = Piece.King,
            ['p'] = Piece.Pawn,
            ['n'] = Piece.Knight,
            ['r'] = Piece.Rook,
            ['b'] = Piece.Bishop,
            ['q'] = Piece.Queen
        };

        public static string NotationFromSquare(int square)
        {
            int file = square % 8;
            int rank = square / 8;

            char fileChar = (char)('a' + file);
            char rankChar = (char)('1' + rank);

            return $"{fileChar}{rankChar}";
        }

        public static int SquareFromNotation(string notation)
        {
            int file = notation[0] - 'a';
            int rank = notation[1] - '1';

            return rank * 8 + file;
        }

        public static Move ParseMove(string move)
        {
            if (string.IsNullOrEmpty(move) || move.Length < 4)
                throw new ArgumentException("Invalid move format. Expected e2e4 or e7e8q");

            int fromSquare = SquareFromNotation(move.Substring(0, 2));
            int toSquare = SquareFromNotation(move.Substring(2, 2));

            MoveFlag flag = MoveFlag.None;
            if (move.Length == 5)
            {
                flag = move[4] switch
                {
                    'q' => MoveFlag.PromoteToQueen,
                    'r' => MoveFlag.PromoteToRook,
                    'b' => MoveFlag.PromoteToBishop,
                    'n' => MoveFlag.PromoteToKnight,
                    _ => throw new ArgumentException($"Unknown promotion piece: {move[4]}")
                };
            }

            return new Move(fromSquare, toSquare, flag);
        }
    }
}
