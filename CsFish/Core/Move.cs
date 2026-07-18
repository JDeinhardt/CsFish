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
        PromoteToBishop
        // TODO: add castling
    }

    public readonly struct Move(int f, int t, MoveFlag flag = MoveFlag.None)
    {
        public readonly int From = f;
        public readonly int To = t;
        public readonly MoveFlag Flag = flag;
    }
}
