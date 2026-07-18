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
    }
}
