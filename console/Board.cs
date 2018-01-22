using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace KnightTourProblemSolver
{
    public class PointI
    {
        public int x { get; set; }
        public int y { get; set; }

        public PointI() { }
        public PointI(int x1, int y1)
        {
            x = x1;
            y = y1;
        }

        public PointI GetNextMove(int xoffset, int yoffset)
        {
            return new PointI(x + xoffset, y + yoffset);
        }
    }

    public class BoardPosition : PointI
    {
        public BoardPosition() { MoveIndex = 0; PossibleMovesCount = 0; }

        public int MoveIndex {get;set;}
        public bool IsUnoccupied()
        {
            return MoveIndex == 0;
        }
        public void SetUnoccupied()
        {
            MoveIndex = 0;
        }

        public int PossibleMovesCount { get; set; }
    }

    public class LogicsBoard
    {

        static int[] horizontal = new int[] { 2, 1, -1, -2, -2, -1, 1, 2 };
        static int[] vertical = new int[] { -1, -2, -2, -1, 1, 2, 2, 1 };
        static int MaxPossMoves = horizontal.Length;
        
        static int rowCount;
        static int colCount;

        BoardPosition[,] m_board;

        public LogicsBoard(int row, int col)
        {
            rowCount = row;
            colCount = col;
            m_board = new BoardPosition[colCount + 1, rowCount + 1];
        }

        public BoardPosition GetPos(int x, int y)
        {
            if (m_board[x, y] == null)
            {
                m_board[x, y] = new BoardPosition();
                m_board[x, y].x = x;
                m_board[x, y].y = y;
            }
            return m_board[x, y];
        }
        public BoardPosition GetPos(PointI c)
        {
            return GetPos(c.x, c.y);
        }
        
        //check if this position is valid or not
        public bool IsPosValid(PointI pos1)
        {
            if ((pos1.x < 1) || (pos1.y < 1) || (pos1.x > colCount) || (pos1.y > rowCount))
                return false;
            return true;
        }

        //check if the next position is valid and if it is occupied
        public bool IsNextPosValid(PointI next)
        {
            return (IsPosValid(next) == true && GetPos(next).IsUnoccupied());
        }
       
        public List<PointI> GetPossibleMoves(PointI curr)
        {
            PointI next;
            List<PointI> PossibleMoves = new List<PointI>();

            for (int moveNumber = 0; moveNumber < MaxPossMoves; moveNumber++)
            {
                next = curr.GetNextMove(horizontal[moveNumber], vertical[moveNumber]);

                if ( IsNextPosValid(next) )
                {
                    PossibleMoves.Add(next);
                }
            }
            return PossibleMoves;
        }        
    }

    public class Algorithm
    {
        static int rowCount;
        static int colCount;
        

        LogicsBoard m_logicsBoard;

        public Algorithm(int row, int col)
        {
            rowCount = row;
            colCount = col;
            m_logicsBoard = new LogicsBoard(row, col);            
        }

        public void SetIndex(PointI c, int index)
        {
            m_logicsBoard.GetPos(c).MoveIndex = index;
        }
        public int GetIndex(PointI c)
        {
            return m_logicsBoard.GetPos(c).MoveIndex;
        }
        
        public void Init()
        {
            int i, j;
            for (i = 1; i <= rowCount; i++)
                for (j = 1; j <= colCount; j++)
                {
                    m_logicsBoard.GetPos(i, j).SetUnoccupied();
                }

            //initialize the m_possibleMovesMatrix array
            for (i = 1; i <= rowCount; i++)
                for (j = 1; j <= colCount; j++)
                {                    
                    PointI p = new PointI(i, j);
                    m_logicsBoard.GetPos(i, j).PossibleMovesCount = GetPossibleMovesCount(p);
                }
        }

        public int GetPossibleMovesCount(PointI curr)
        {
            return m_logicsBoard.GetPossibleMoves(curr).Count;
        }

        public PointI DetermineNextMove(PointI curr)
        {
            // There can be multiple cells with the same lowest move count: up to 8
            // get smallest possible move count array for level 1            
            List<BoardPosition> L1Candidates = GetSmallestAccessArray(curr);

            if (L1Candidates.Count == 0)
                return null;

            if (L1Candidates.Count == 1)
                return L1Candidates[0];

            // level 2 forward check
            int smallestAccessValIndex_Level1 = 0;
            BoardPosition candidatePos = L1Candidates[smallestAccessValIndex_Level1];
            int smallestAccessVal_Level1 = candidatePos.PossibleMovesCount;

            // designate 1st one is the smallest
            int smallestAccessVal_Level2 = GetSmallestAccessVal(candidatePos);

            int candidate_smallestAccessVal_Level2 = -1;
            for (int level1_i = 1; level1_i < L1Candidates.Count; level1_i++)
            {
                candidate_smallestAccessVal_Level2 = GetSmallestAccessVal(L1Candidates[level1_i]);
                // if the level 2 
                if (smallestAccessVal_Level2 > candidate_smallestAccessVal_Level2)
                    smallestAccessValIndex_Level1 = level1_i;
            }

            return L1Candidates[smallestAccessValIndex_Level1];
        }

        public void UpdateAccessibility(PointI curr)
        {
            List<BoardPosition> PossiblePos = GetPossibleMovePositions(curr);
            foreach (BoardPosition p in PossiblePos)
            {
                p.PossibleMovesCount--;
            }
        }

        #region PrivateMethods

        List<BoardPosition> GetPossibleMovePositions(PointI curr)
        {
            List<PointI> PossiblePoints = m_logicsBoard.GetPossibleMoves(curr);
            List<BoardPosition> PossiblePos = new List<BoardPosition>();

            foreach (PointI p in PossiblePoints)
            {
                PossiblePos.Add(m_logicsBoard.GetPos(p));
            }

            return PossiblePos;
        }

        int GetSmallestAccessVal(List<BoardPosition> Positions)
        {
            if (Positions == null || Positions.Count == 0)
                return -1;

            int smallestIndex = 0;
            for ( int i=1; i < Positions.Count; i++)
            {
                if ( Positions[i].PossibleMovesCount < Positions[smallestIndex].PossibleMovesCount )
                    smallestIndex = i;
            }
            int smallestValue = Positions[smallestIndex].PossibleMovesCount;
            return smallestValue;
        }

        int GetSmallestAccessVal(PointI curr)
        {
            // Get all possible moves from a given position
            List<BoardPosition> Positions = GetPossibleMovePositions(curr);

            if (Positions == null || Positions.Count == 0)
                return -1;

            return GetSmallestAccessVal(Positions);
        }

        List<BoardPosition> GetSmallestAccessArray(PointI curr)
        {
            // Get all possible moves from a given position
            List<BoardPosition> Positions = GetPossibleMovePositions(curr);

            // Get the smallest value
            int smallestValue = GetSmallestAccessVal(Positions);

            // Remove non-smallest positions
            for (int i = 0; i < Positions.Count; i++)
            {
                if (Positions[i].PossibleMovesCount > smallestValue)
                {
                    Positions.RemoveAt(i);
                    i--;
                }
            }

            return Positions;
        }

        #endregion PrivateMethods
    }
}
