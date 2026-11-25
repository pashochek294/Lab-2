using System;
using System.Collections.Generic;

namespace TicTacToeLab
{
    public class MovePosition
    {
        public int Row { get; set; }
        public int Col { get; set; }

        public MovePosition(int row, int col)
        {
            Row = row;
            Col = col;
        }
    }

    public class TicTacToeGame
    {
        public const int BoardSize = 3;
        public const char EmptyCell = ' ';
        public const char HumanMark = 'X';
        public const char ComputerMark = 'O';

        public const int LevelRandom = 1;
        public const int LevelBlock = 2;
        public const int LevelSmart = 3;

        private readonly char[,] _board = new char[BoardSize, BoardSize];
        private readonly Random _random = new Random();

        public bool IsGameOver { get; private set; }
        public char Winner { get; private set; }

        public TicTacToeGame()
        {
            Reset();
        }

        public void Reset()
        {
            for (int r = 0; r < BoardSize; r++)
            {
                for (int c = 0; c < BoardSize; c++)
                {
                    _board[r, c] = EmptyCell;
                }
            }

            IsGameOver = false;
            Winner = EmptyCell;
        }

        public char GetCell(int row, int col)
        {
            return _board[row, col];
        }

        public bool MakeMove(int row, int col, char mark)
        {
            if (IsGameOver)
            {
                return false;
            }

            if (row < 0 || row >= BoardSize || col < 0 || col >= BoardSize)
            {
                return false;
            }

            if (_board[row, col] != EmptyCell)
            {
                return false;
            }

            _board[row, col] = mark;
            Winner = CheckWinner();

            if (Winner != EmptyCell || IsDraw())
            {
                IsGameOver = true;
            }

            return true;
        }

        public List<MovePosition> GetAvailableMoves()
        {
            List<MovePosition> moves = new List<MovePosition>();

            for (int r = 0; r < BoardSize; r++)
            {
                for (int c = 0; c < BoardSize; c++)
                {
                    if (_board[r, c] == EmptyCell)
                    {
                        moves.Add(new MovePosition(r, c));
                    }
                }
            }

            return moves;
        }

        public char CheckWinner()
        {
            for (int i = 0; i < BoardSize; i++)
            {
                if (IsLineWinning(i, 0, 0, 1))
                {
                    return _board[i, 0];
                }

                if (IsLineWinning(0, i, 1, 0))
                {
                    return _board[0, i];
                }
            }

            if (IsLineWinning(0, 0, 1, 1))
            {
                return _board[0, 0];
            }

            if (IsLineWinning(0, BoardSize - 1, 1, -1))
            {
                return _board[0, BoardSize - 1];
            }

            return EmptyCell;
        }

        private bool IsLineWinning(int startRow, int startCol, int stepRow, int stepCol)
        {
            char first = _board[startRow, startCol];
            if (first == EmptyCell)
            {
                return false;
            }

            for (int i = 1; i < BoardSize; i++)
            {
                int r = startRow + i * stepRow;
                int c = startCol + i * stepCol;
                if (_board[r, c] != first)
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsDraw()
        {
            if (CheckWinner() != EmptyCell)
            {
                return false;
            }

            return GetAvailableMoves().Count == 0;
        }

        public MovePosition? ChooseComputerMove(int level)
        {
            List<MovePosition> available = GetAvailableMoves();
            if (available.Count == 0)
            {
                return null;
            }

            if (level == LevelSmart)
            {
                MovePosition? winMove = FindWinningMove(ComputerMark, available);
                if (winMove != null)
                {
                    return winMove;
                }
            }

            if (level == LevelBlock || level == LevelSmart)
            {
                MovePosition? blockMove = FindWinningMove(HumanMark, available);
                if (blockMove != null)
                {
                    return blockMove;
                }
            }

            int randomIndex = _random.Next(available.Count);
            return available[randomIndex];
        }

        private MovePosition? FindWinningMove(char mark, List<MovePosition> available)
        {
            for (int i = 0; i < available.Count; i++)
            {
                int r = available[i].Row;
                int c = available[i].Col;

                _board[r, c] = mark;
                char winner = CheckWinner();
                _board[r, c] = EmptyCell;

                if (winner == mark)
                {
                    return available[i];
                }
            }

            return null;
        }
    }
}
