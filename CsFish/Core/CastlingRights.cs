using System;
using System.Collections.Generic;
using System.Text;

namespace CsFish.Core
{
    public struct CastlingRights(bool wk, bool wq, bool bk, bool bq)
    {
        public bool CanWhiteCastleQueenSide = wq;
        public bool CanWhiteCastleKingSide = wk;
        public bool CanBlackCastleQueenSide = bq;
        public bool CanBlackCastleKingSide = bk;

        public int ToIndex() =>
            (CanWhiteCastleKingSide ? 1 : 0) |
            (CanWhiteCastleQueenSide ? 2 : 0) |
            (CanBlackCastleKingSide ? 4 : 0) |
            (CanBlackCastleQueenSide ? 8 : 0);
    }
}
