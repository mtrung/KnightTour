using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Threading;

namespace KnightTourProblemSolver
{
	public partial class UIBoard : UserControl, INotifyPropertyChanged
	{
        static int nRowCount = 8, nColCount = 8;
        static int StrokeThickness = 3;
        static Size cellSize = new Size(330 / nColCount, 250 / nRowCount);

        TextBlock[,] texts = new TextBlock[nColCount, nRowCount];
        Shape[,] cells = new Rectangle[nColCount, nRowCount];

        SolidColorBrush blackBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        SolidColorBrush whiteBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        //SolidColorBrush borderBrush = new SolidColorBrush(Color.FromArgb(255, 64, 128, 64));
        SolidColorBrush borderBrush = new SolidColorBrush(Color.FromArgb(255, 0, 92, 0));
        SolidColorBrush yellowBrush = new SolidColorBrush(Color.FromArgb(255, 238, 246, 14));
        SolidColorBrush redBrush = new SolidColorBrush(Color.FromArgb(255, 246, 14, 14));
        //SolidColorBrush redBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));

        Algorithm mainBoard;
        
        int currentX;
        int currentY;//0-based
        
        bool b1stTime = true;
        PointI curr = new PointI();// 1-based
        bool isAlive = false;

        public UIBoard()
		{
			// Required to initialize variables
			InitializeComponent();                      

            for (int y = 0; y < nRowCount; y++)
            {
                for (int x = 0; x < nColCount; x++)
                {
                    //Ellipse ellipse = new Ellipse();
                    Rectangle ellipse = new Rectangle();
                    ellipse.Fill = blackBrush;
                    ellipse.Stroke = borderBrush;
                    ellipse.StrokeThickness = StrokeThickness;
                    ellipse.Width = cellSize.Width - StrokeThickness; //32;
                    ellipse.Height = cellSize.Height - StrokeThickness; //32;             
                    Canvas.SetLeft(ellipse, 2 + x * (cellSize.Width));
                    Canvas.SetTop(ellipse, 2 + y * (cellSize.Height));

                    cells[x, y] = ellipse;

                    TextBlock text = new TextBlock();
                    text.Text = "1";
                    //text.Height = cellSize.Height - StrokeThickness; 
                    text.Width = cellSize.Width - StrokeThickness;
                    text.TextAlignment = TextAlignment.Center;
                    //text.VerticalAlignment = VerticalAlignment.Center;
                    Canvas.SetLeft(text, 2 + x * (cellSize.Width));
                    Canvas.SetTop(text, 2 + y * (cellSize.Height) + cellSize.Height/4);
                    texts[x, y] = text;


                    LayoutRoot.Children.Add(ellipse);
                    LayoutRoot.Children.Add(text);
                }
            }
            //LayoutRoot.Background = borderBrush;
            NewGame();
        }

        #region Methods

        public void NewGame()
        {
            mainBoard = new Algorithm(nColCount, nRowCount);
            mainBoard.Init();

            Clear();
            b1stTime = true;
            IsAlive = false;

            RefreshBoard();
        }

        public void RefreshBoard()
        {
            // highlight if current pos
            for (int y = 0; y < nRowCount; y++)
            {
                for (int x = 0; x < nColCount; x++)
                {
                    if ((x == currentX) && (y == currentY) && (!IsAlive))
                        cells[x, y].Stroke = whiteBrush;
                    else
                        cells[x, y].Stroke = borderBrush;
                }
            }
        }

        #endregion

        #region Events

        private void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Translate2LogicalPos(e);
            MoveNext();
        }

        public bool IsAlive 
        {
            get { return this.isAlive; }
            set
            {
                if (value != this.isAlive)
                {
                    this.isAlive = value;
                    NotifyPropertyChanged("IsAlive");
                }
            }
        }

		// for Move Next button IsEnabled status
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public void MoveNext()
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
				
				if (boardindex == 64 )
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
        void writexyC(PointI curr, int boardindex, bool bHighlight)
        {
            if (bHighlight)
                cells[curr.x-1, curr.y-1].Fill = redBrush;
            else cells[curr.x-1, curr.y-1].Fill = yellowBrush;
            texts[curr.x-1, curr.y-1].Text = boardindex.ToString();
        }

        void Clear()
        {
            for (int y = 0; y < nRowCount; y++)
            {
                for (int x = 0; x < nColCount; x++)
                {
                    cells[x, y].Fill = blackBrush;
                }
            }
        }

        private void Translate2LogicalPos(MouseEventArgs e)
        {
            currentX = (int)(e.StylusDevice.GetStylusPoints(LayoutRoot)[0].X / cellSize.Width);
            if (currentX < 0)
                currentX = 0;
            if (currentX > nColCount)
                currentX = nColCount;

            currentY = (int)(e.StylusDevice.GetStylusPoints(LayoutRoot)[0].Y / cellSize.Height);
            if (currentY < 0)
                currentY = 0;
            if (currentY > nRowCount)
                currentY = nRowCount;
        }

        private void LayoutRoot_MouseMove(object sender, MouseEventArgs e)
        {
            Translate2LogicalPos(e);

            RefreshBoard();
        }

        #endregion
    }
}