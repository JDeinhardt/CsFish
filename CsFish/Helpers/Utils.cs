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

        public static int[] LoadPositionFromFEN(string fen)
        {
            int[] squares = new int[64];

            string fenBoard = fen.Split(' ')[0];
            int file = 0;
            int rank = 7;

            foreach (char c in fenBoard)
            {
                if (c == '/')
                {
                    file = 0;
                    rank--;
                }
                else
                {
                    if (char.IsDigit(c))
                        file += (int)char.GetNumericValue(c);
                    else
                    {
                        int color = char.IsUpper(c) ? Piece.White : Piece.Black;
                        int type = PieceTypeFromSymbol[char.ToLower(c)];
                        squares[rank * 8 + file] = type | color;
                        file++;
                    }
                }
            }

            return squares;
        }
    }
}
