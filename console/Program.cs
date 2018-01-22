using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KnightTourProblemSolver;


namespace knighttour_console
{
    class Program
    {
        static void Main(string[] args)
        {
            mainBoard = new Algorithm(8, 8);
            mainBoard.Init();

            //Clear();
            currentX = 0;
            currentY = 0;
            b1stTime = true;
            IsAlive = false;

            for ( int i=0; i < 64; i++)
                MoveNext();
        }

        static bool b1stTime = true;
        static PointI curr = new PointI();// 1-based
        static bool IsAlive = false;

        static Algorithm mainBoard;

        static int currentX;
        static int currentY;//0-based

        static public void MoveNext()
        {
            if (b1stTime)
            {
                b1stTime = false;
                curr.x = currentX + 1;
                curr.y = currentY + 1;
                mainBoard.SetIndex(curr, 1);
                writexyC(curr, 1, true);//highlight color
                IsAlive = true;
                return;
            }

            int poss_num = mainBoard.GetPossibleMovesCount(curr);
            int boardindex = mainBoard.GetIndex(curr);

            if (poss_num != 0 && boardindex <= 64)
            {
                writexyC(curr, boardindex, false);//reset color
                mainBoard.UpdateAccessibility(curr);//update current position accessibility

                curr = mainBoard.DetermineNextMove(curr);
                mainBoard.SetIndex(curr, ++boardindex);
                writexyC(curr, boardindex, true);//highlight color

                if (boardindex == 64)
                {
                    IsAlive = false;
                }
            }
            else
            {
                IsAlive = false;
            }
        }

        //write number to the board w/ color
        static void writexyC(PointI curr, int boardindex, bool bHighlight)
        {
            if (bHighlight)
                Console.WriteLine("({0},{1}): Highlight. index= {2}", curr.x - 1, curr.y - 1, boardindex);
            else Console.WriteLine("   ({0},{1}): Unhighlight. index= {2}", curr.x - 1, curr.y - 1, boardindex);
        }

        void Clear()
        {
        }
    }
}
