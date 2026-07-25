using CsFish.Core;
using CsFish.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace CsFish.Ressources
{
    public struct BookMove(Move m, int w)
    {
        public Move move = m;
        public int Weight = w;
    }

    public class OpeningBook
    {
        private readonly Dictionary<string, List<BookMove>> _book = [];

        public void LoadOpeningBook(string path, Board b)
        {
            string currentFen = "";

            foreach(var line in File.ReadLines(path))
            {
                if (line.StartsWith("pos"))
                {
                    currentFen = line[4..];
                    _book[currentFen] = []; 
                }
                else if (!string.IsNullOrWhiteSpace(line))
                {
                    var parts = line.Split(' ');

                    Move move = Utils.ParseMove(parts[0], b);
                    int weight = int.Parse(parts[1]);

                    _book[currentFen].Add(new BookMove
                    {
                        move = move,
                        Weight = weight
                    });
                }
            }
        }

        public bool TryGetMove(Board board, out Move move)
        {
            string fen = Utils.BoardToFEN(board);

            if (!_book.TryGetValue(fen, out var moves))
            {
                move = default;
                return false;
            }


            int total = moves.Sum(x => x.Weight);

            int random = Random.Shared.Next(total);


            foreach (var m in moves)
            {
                random -= m.Weight;

                if (random < 0)
                {
                    move = m.move;
                    return true;
                }
            }


            move = moves[0].move;
            return true;
        }
    }
}
