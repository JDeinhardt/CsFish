using CsFish.Core;
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
    }
}
