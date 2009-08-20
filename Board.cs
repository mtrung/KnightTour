using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Text;

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

        static int[] horizontal = new int[] { 0, 2, 1, -1, -2, -2, -1, 1, 2 };
        static int[] vertical = new int[] { 0, -1, -2, -2, -1, 1, 2, 2, 1 };

        //there are maximum 8 possible moves a particular position.
        //they are numbered from 1..8 and called moveNumber
        //return "GetNextMove" depending on moveNumber
        public PointI GetNextMove(int moveNumber)
        {
            PointI next = new PointI(
                x + horizontal[moveNumber],
                y + vertical[moveNumber]);
            return next;
        }
    }

    public class Board
    {
        static int rowCount;
        static int colCount;
        int MaxPossMoves = 8;

        int[,] m_possibleMovesMatrix;   //  = # of possible moves of a position
        int[,] m_moveIndexMatrix;       //  = move index of a position. If 0, it's unoccupied.

        public Board(int row, int col)
        {
            rowCount = row;
            colCount = col;
            m_possibleMovesMatrix = new int[colCount + 1, rowCount + 1];
            m_moveIndexMatrix = new int[colCount + 1, rowCount + 1];
        }

        #region MyCode

        //check if this position is valid or not
        bool IsPosValid(PointI pos1)
        {
            if ((pos1.x < 1) || (pos1.y < 1) || (pos1.x > colCount) || (pos1.y > rowCount))
                return false;
            return true;
        }

        //check if the next position is valid
        //whether it is occupied
        bool IsNextPosValid(PointI next)
        {
            if (IsPosValid(next) == true && 
                (GetIndex(next) == 0) // unoccupied
                )
                return true;
            else return false;
        }


        //find index that array[index] is smallest
        int FindMin(int[] array, int start, int end)
        {
            int smallest = start, i;
            for (i = start + 1; i <= end; i++)
                if (array[smallest] > array[i])
                    smallest = i;
            return smallest;
        }

        //return the smallest Accessibility number of all next possible moves
        //of current position
        int getSmallestAccessValue(PointI curr)
        {
            int i, smallest;
            int[] accessNext = new int[9], possMove = new int[9];
            PointI[] nexta = new PointI[9];

            GetPossibleMoves(curr, ref possMove);
            //get all possible move into database nexta[]
            for (i = 1; i <= possMove[0]; i++)
            {
                nexta[i] = curr.GetNextMove(possMove[i]);
                //nexta[i] contains postion of this GetNextMove i
                accessNext[i] = m_possibleMovesMatrix[nexta[i].x, nexta[i].y];
                //accessNext contains the rate of this GetNextMove i
            }
            smallest = FindMin(accessNext, 1, possMove[0]);
            return accessNext[smallest];
        }

        #endregion MyCode

        #region MyCodePublic

        public void SetIndex(PointI c, int index)
        {
            m_moveIndexMatrix[c.x, c.y] = index;
        }
        public int GetIndex(PointI c)
        {
            return m_moveIndexMatrix[c.x, c.y];
        }

        public void init()
        {
            int i, j;
            for (i = 1; i <= rowCount; i++)
                for (j = 1; j <= colCount; j++)
                {
                    m_moveIndexMatrix[i, j] = 0;
                }

            //initialize the m_possibleMovesMatrix array
            for (i = 1; i <= rowCount; i++)
                for (j = 1; j <= colCount; j++)
                {
                    m_possibleMovesMatrix[i, j] = GetPossibleMoves(new PointI(i, j));
                }
        }

        /*
        This function returns number of possible moves
        and possible_move array containing those moves

        Ex: there are 3 possible moves numbered: 3 5 8
        GetPossibleMoves returns 3 = number of moves
        possible_move[0]=3= number of moves
        possible_move[1]=3= 1st moveNumber
        possible_move[2]=5= 2nd moveNumber
        possible_move[3]=8= 3rd moveNumber */
        int GetPossibleMoves(PointI curr, ref int[] possible_move)
        {
            int possibleMoveCount = 0, moveNumber = 0;
            PointI next;

            while (moveNumber < MaxPossMoves)
            {
                moveNumber++;
                next = curr.GetNextMove(moveNumber);//interpret moveNumber to position
                //if this next move is valid then put the moveNumber into array
                if (IsNextPosValid(next))
                {
                    possibleMoveCount++;
                    possible_move[possibleMoveCount] = moveNumber;
                }
            }
            possible_move[0] = possibleMoveCount;
            return possibleMoveCount;
        }

        public int GetPossibleMoves(PointI curr)
        {
            int[] possible_move = new int[MaxPossMoves+1];
            return GetPossibleMoves(curr, ref possible_move);
        }

        //if current position determine the best next move
        public PointI determineNextMove(PointI curr)
        {
            int i, t1, t2, smallestIndex;
            int[] accessNext = new int[MaxPossMoves+1];
            int[] possMove = new int[MaxPossMoves+1];
            PointI[] nexta = new PointI[MaxPossMoves+1];

            //These codes till "---" are the same as getSmallestAccessValue
            //but it stores coordinate and m_possibleMovesMatrix of all possible moves
            //into nexta and accessNext

            int possibleMoveCount = GetPossibleMoves(curr, ref possMove);
            for (i = 1; i <= possibleMoveCount; i++)
            {
                //nexta[i] contains postion of this GetNextMove i
                nexta[i] = curr.GetNextMove(possMove[i]);
                accessNext[i] = m_possibleMovesMatrix[nexta[i].x, nexta[i].y];
                //accessNext contains the rate of this GetNextMove i
            }
            smallestIndex = FindMin(accessNext, 1, possMove[0]);
            //------------------

            //if there are many smallest
            int baseVal = smallestIndex;//because smallestIndex will be changed
            for (i = 1; i <= possibleMoveCount; i++)
            {
                //find another smallest value in accessNext
                if ((accessNext[i] == accessNext[baseVal]) && (i != baseVal))
                {
                    t1 = getSmallestAccessValue(nexta[smallestIndex]);
                    t2 = getSmallestAccessValue(nexta[i]);
                    if (t1 > t2)
                        smallestIndex = i;
                }
            }//end for

            return nexta[smallestIndex];
        }

        //update the Accessibility
        public void updateAccessibility(PointI curr)
        {
            int[] possibleMove = new int[MaxPossMoves+1];
            PointI[] NextPossibleMove = new PointI[MaxPossMoves+1];

            //get all possible moveNumber into possibleMove array
            int possibleMoveCount = GetPossibleMoves(curr, ref possibleMove);
            for (int i = 1; i <= possibleMoveCount; i++)
            {
                //get all possible move positions to NextPossibleMove array
                NextPossibleMove[i] = curr.GetNextMove(possibleMove[i]);
                //decrease by 1 the m_possibleMovesMatrix rate of those NextPossibleMove
                //because those next moves cannot move back to current position.
                --m_possibleMovesMatrix[NextPossibleMove[i].x, NextPossibleMove[i].y];
            }
        }

        #endregion MyCodePublic
    }
}
