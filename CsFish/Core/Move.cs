using CsFish.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace CsFish.Core
{
    public enum MoveFlag
    {
        None,
        EnPassant,
        PromoteToQueen,
        PromoteToRook,
        PromoteToKnight,
        PromoteToBishop,
        CastleWhiteQueenSide,
        CastleWhiteKingSide,
        CastleBlackQueenSide,
        CastleBlackKingSide
    }

    public readonly struct Move(int f, int t, MoveFlag flag = MoveFlag.None)
    {
        public readonly int From = f;
        public readonly int To = t;
        public readonly MoveFlag Flag = flag;

        public override string ToString()
        {
            string promotion = Flag switch
            {
                MoveFlag.PromoteToQueen => "q",
                MoveFlag.PromoteToRook => "r",
                MoveFlag.PromoteToBishop => "b",
                MoveFlag.PromoteToKnight => "n",
                _ => ""
            };
            return Utils.NotationFromSquare(From) + Utils.NotationFromSquare(To) + promotion;
        }

        public static bool AreEqual(Move m1, Move m2)
        {
            return m1.To == m2.To && m1.From == m2.From && m1.Flag == m2.Flag;
        }
    }
}
