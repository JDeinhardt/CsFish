using System;
using System.Collections.Generic;
using System.Text;

namespace CsFish.Core
{
    public static class Piece
    {
        public const int TypeMask = 7;
        public const int ColorMask = 24;

        public const int White = 8;
        public const int Black = 16;

        public const int None = 0;
        public const int Pawn = 1;
        public const int Rook = 2;
        public const int Bishop = 3;
        public const int Knight = 4;
        public const int King = 5;
        public const int Queen = 6;

        public static bool IsType(int p, int type)
        {
            return (p & TypeMask) == type;

            /*
             * 10110 -> 10: black; 110: Queen
             * 00111 -> type mask
             * 00110 -> bitwise and
             * 
             * 00110 -> Queen type
             * 
             * 00110 == 00110 -> true
             * */
        }

        public static bool IsColor(int p, int color)
        {
            return (p & ColorMask) == color;

            /*
             * 10110 -> 10: black; 110: Queen
             * 11000 -> color mask
             * 10000 -> bitwise and
             * 
             * 10000 -> black color
             * 
             * 10000 == 1000 -> true
             * */
        }

        public static int GetType(int p)
        {
            return (p & 0b111);
        }

        public static int GetColor(int p)
        {
            return (p >> 3) & 0b11;
        }
    }
}
