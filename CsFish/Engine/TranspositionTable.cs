using CsFish.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CsFish.Engine
{
    public enum TTFlag
    {
        Exact = 0,
        LowerBound = 1, // score at least this value
        HigherBound = 2, // score max this value
    }

    public struct TTEntry
    {
        public ulong Key;
        public int Score;
        public int Depth;
        public TTFlag Flag;
        public Move BestMove;
    }

    public class TranspositionTable
    {
        private const int SIZE_BITS = 21; // 2^21 
        private TTEntry[] _table;

        public TranspositionTable()
        {
            _table = new TTEntry[1 << SIZE_BITS];
        }

        public void Clear() => Array.Clear(_table);

        private int Index(ulong key) => (int)(key & (1UL << SIZE_BITS) - 1);

        public (bool hit, int score, Move bestMove) Probe(ulong key, int alpha, int beta, int depth)
        {
            ref TTEntry entry = ref _table[Index(key)];

            if (entry.Key != key)
                return (false, 0, default);

            if (entry.Depth < depth)
                return (false, 0, default);

            int score = entry.Score;

            return entry.Flag switch
            {
                TTFlag.Exact => (true, score, entry.BestMove),
                TTFlag.LowerBound => score >= beta ? (true, score, entry.BestMove) : (false, 0, entry.BestMove),
                TTFlag.HigherBound => score <= alpha ? (true, score, entry.BestMove) : (false, 0, entry.BestMove),
                _ => (false, 0, entry.BestMove)
            };

        }

        public void Store(ulong key, int score, int depth, TTFlag flag, Move bestMove)
        {
            ref TTEntry entry = ref _table[Index(key)];

            if (entry.Key == 0 || entry.Depth <= depth)
            {
                entry.Key = key;
                entry.Score = score;
                entry.Flag = flag;
                entry.BestMove = bestMove;
                entry.Depth = depth;
            }

        }
    }
}
