using System;
using System.Collections.Generic;
using System.Text;

namespace CsFish.Core
{
    public static class Zobrist
    {
        public static readonly ulong[,] PieceSquare = new ulong[23, 64]; // Piece.Queen (6) Piece.Blakc (16), 16 | 6 max, so 22
        public static readonly ulong[] Castling = new ulong[16];
        public static readonly ulong[] EnPassantFile = new ulong[8];
        public static readonly ulong SideToMove;

        static Zobrist()
        {
            var rng = new Random(20260720);

            for (int piece = 0; piece < 23; piece++)
                for (int square = 0; square < 64; square++)
                    PieceSquare[piece, square] = NextULong(rng);

            for (int i = 0; i < 16; i++)
                Castling[i] = NextULong(rng);

            for (int file = 0; file < 8; file++)
                EnPassantFile[file] = NextULong(rng);

            SideToMove = NextULong(rng);
        }

        private static ulong NextULong(Random rng)
        {
            var buffer = new byte[8];
            rng.NextBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }
    }
}
