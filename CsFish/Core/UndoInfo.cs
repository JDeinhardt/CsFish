using System;
using System.Collections.Generic;
using System.Text;

namespace CsFish.Core
{
    public struct UndoInfo(int cp, int prevEPS)
    {
        public int CapturedPiece { get; init; } = cp;
        public int PreviousEnPassantSquare { get; init; } = prevEPS;
        // TODO: castlingrights, halvmove clock, etc.
    }
}
