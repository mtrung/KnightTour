//Name : Trung Vo
//Class: CS3335.001
//Instructor: Tracy Dobbs
//Extra credit: Knight's Tour problem

//Use Borland C++ Compiler Only
//Final revision date: Nov 9th 1999

/*One of the interesting puzzlers for chess buffs is the
Knight's Tour problem, originally proposed by the  mathematician Euler.
The question is: Can the chess piece called the knight move around
an empty chessboard and touch each of the 64 squares once and only once?

An array 9x9 will be created to simulate the chessboard, subscript 0 will
be reserved for further development. Put a 1 in the first square you move
to, a 2 in the second square, a 3 in the third...

Each potential move will be tested to make sure it doesn't land off the
board and doesn't revisit the old move.

This program will use accessibility heuristic. Accessibility of a square
will be equal precisely to the number of squares from which that square
may be reached. At any time, the knight should move to the square with
the lowest accessibility.

When encoutering a tie in accessibility between two or more squares,
decides what square to choose by looking ahead to those squares reachable
from the "tied" squares. The program should move to the square for which
the next move would arrive at a square with the lowest accessibility number.
*/


#include <iostream.h>
#include <iomanip.h>
#include <conio.h>

const int horizontal[9] = {0, 2, 1,-1,-2,-2,-1,1,2};
const int vertical  [9] = {0,-1,-2,-2,-1, 1, 2,2,1};
int accessibility [9][9];
int board [9][9];
const int basex=1;//base coordinate of the board
const int basey=4;

typedef struct tagpos {
 int x;
 int y;
 } pos;

void init();
void getdata(pos &curr);
void printFrame();
void drawline(int loop, int count, const char bar, const char start,
					const char mid,const char end);
void writexyC(pos curr, int color, int backg, int boardindex);
void pressEnter();

int  is_nextpos_valid(pos next);
int  is_thispos_valid(pos pos1);
pos  nextmove(pos curr, int moveNumber);
int  get_possible_move(pos curr, int possible_move[9]);
pos determineNextMove(pos curr);
void updateAccessibility(pos curr);

void main()
{
	pos curr;
	int poss_num, possible_move[9], boardindex=1;

	clrscr();//clear sreen
	init();//initialize the board and accessibility array
	printFrame();
	getdata(curr);//get initial position
	//this asm code turns cursor off 
	asm {
	mov ch,32
	mov ah,1
	int 0x10
	}

	
	board[curr.x][curr.y] = boardindex;//first position is given
	writexyC(curr, YELLOW, LIGHTRED, boardindex);//highlight color
	pressEnter();
	pressEnter();
	do	{
		writexyC(curr, WHITE,BLACK, boardindex);//reset color
		updateAccessibility(curr);//update current position accessibility
		poss_num = get_possible_move(curr, possible_move);
		if (poss_num!=0)	{
			curr = determineNextMove(curr);
			++boardindex;
			board[curr.x][curr.y] = boardindex;
			writexyC(curr, YELLOW, LIGHTRED,boardindex);//highlight color
			pressEnter();
		}//end if
	} while (poss_num!=0);
}//end main

//convert position in the board to real position in the screen
pos convert(pos curr)
{
	pos realpos;
	realpos.x = basex + 5*curr.x +1 ;
	realpos.y = basey + 2*curr.y;
	return realpos;
}

//write number to the board w/ color
void writexyC(pos curr, int color, int backg, int boardindex)
{
	pos realpos = convert(curr);
	gotoxy(realpos.x, realpos.y);
	textcolor(color);
	textbackground(backg);
	cprintf("%2i",boardindex);
}

//prompt
void pressEnter()
{
	gotoxy(1,23);
	cout<<"Press enter to continue...";
	cin.get();
}

//draw a line w/ the following format:
//Ex:  SvvvvCvvvvC....CvvvvE
// 'S' is start character
// 'v' is bar character
// 'C' is mid character
// 'E' is end character
//number of v is count
//number of sequence 'vvvv' is loop

void drawline(int loop, int count, const char bar, const char start,
						const char mid,const char end)
{
	register int i,j;
	cprintf("%c",start);
	for (i=1;i<=loop;i++)
	{
		for (j=1;j<=count;j++) cprintf("%c",bar);
		cprintf("%c",mid);
	}
	gotoxy(wherex()-1,wherey());
	cprintf("%c",end);
}

//print the frame
void printFrame()
{
	int i;
	const char VR = char(196); // the '-' character
	const char HZ = char(179); // the '|' character

	const char UPLEFT = char(218);
	const char UPRIGHT = char(191);
	const char UPCENTER = char(194);

	const char CENTERLEFT = char(195);
	const char CENTER = char(197);
	const char CENTERRIGHT = char(180);

	const char LOWRIGHT = char(217);
	const char LOWCENTER = char(193);
	const char LOWLEFT = char(192);

	window(1,1,80,1);
	textcolor(YELLOW);
	textbackground(BLUE);
	clrscr();
	cout << "\tWelcome to Knight's Tour Problem Solver.";

	window(1,25,80,25);
	textcolor(YELLOW);
	textbackground(RED);
	clrscr();
	cout << "\tWritten by Trung Vo.";

	window(1,2,80,24);
	textcolor(LIGHTCYAN);
	textbackground(BLACK);

	//write number
	gotoxy(basex,basey);
	cout << "     ";
	for(i=1; i<=8; i++) cout << "  "<<i<<"  ";

	gotoxy(basex+2,basey+3);
	for(i=1; i<=8; i++) {gotoxy(basex+2, basey+2*i);cout << i;}

	//draw board
	int j;
//MIDDLE
	for(j=1; j<=8; j++)
	{
	gotoxy(basex+4,basey+2*j);
	drawline(8,4,' ',HZ,HZ,HZ);

	gotoxy(basex+4,basey+2*j+1);
	drawline(8,4,VR,CENTERLEFT,CENTER,CENTERRIGHT);
	}
//first line
	gotoxy(basex+4,basey+1);
	drawline(8,4,VR,UPLEFT,UPCENTER,UPRIGHT);
//last line
	gotoxy(basex+4,basey+2*j-1);
	drawline(8,4,VR,LOWLEFT,LOWCENTER,LOWRIGHT);
}

//get the initial position
void getdata(pos &curr)
{
	gotoxy(1,1);
	cout<<"Enter the initial position of the knight(separate by a space) : ";
	cin>>curr.x>>curr.y;
	//check if user enters invalid data
	while ((curr.x>8)||(curr.x<1)||(curr.y>8)||(curr.y<1))	{
		gotoxy(1,1);
		cout<<"                                                                               ";
		gotoxy(1,1);
		cout<<"Please re-enter: ";
		cin>>curr.x>>curr.y;
	}
}

//initialize a 8x8 chess table
//set board to 0
//0 is not used
//initialize the accessibility array
void init()
{
	int i,j;
	for (i=1;i<=8;i++)
		for (j=1;j<=8;j++)
			board[i][j]=0;

//initialize the accessibility array
	for (i=1;i<=8;i++)
	 for (j=1;j<=8;j++)
	  accessibility[i][j]=4;

	accessibility[1][1]=accessibility[1][8]=
	accessibility[8][1]=accessibility[8][8]=2;

	accessibility[1][2]=accessibility[2][1]=
	accessibility[7][1]=accessibility[8][2]=
	accessibility[1][7]=accessibility[2][8]=
	accessibility[7][8]=accessibility[8][7]=3;

	for (i=2;i<=7;i++)
	 for (j=2;j<=7;j++)
	  accessibility[i][j]=6;
	accessibility[2][2]=accessibility[2][7]=4;
	accessibility[7][2]=accessibility[7][7]=4;

	for (i=3;i<=6;i++)
	 for (j=3;j<=6;j++)
	  accessibility[i][j]=8;
}

//check if this position is valid or not
int is_thispos_valid(pos pos1)
{
	if ( (pos1.x<1) || (pos1.y<1) || (pos1.x>8) || (pos1.y>8) ) return 0;
	else return 1;
}

//check if the next position is valid
//whether it is occupied
int is_nextpos_valid(pos next)
{
	if ( is_thispos_valid(next) && (board[next.x][next.y]==0) )
		return 1;
	else return 0;
}

//there are maximum 8 possible moves a particular position.
//they are numbered from 1..8 and called moveNumber
//return "nextmove" depending on moveNumber
pos nextmove(pos curr, int moveNumber)
{
	pos next;
	next.x = curr.x + horizontal[ moveNumber ];
	next.y = curr.y + vertical  [ moveNumber ];
	return next;
}

/*
This function returns number of possible moves
and possible_move array containing those moves

Ex: there are 3 possible moves numbered: 3 5 8
get_possible_move returns 3 = number of moves
possible_move[0]=3= number of moves
possible_move[1]=3= 1st moveNumber
possible_move[2]=5= 2nd moveNumber
possible_move[3]=8= 3rd moveNumber */
int get_possible_move(pos curr, int possible_move[9])
{
	int num=0, moveNumber=0;
	pos next;

	while (moveNumber <= 8)	{
		moveNumber++;
		next = nextmove(curr, moveNumber);//interpret moveNumber to position
		//if this next move is valid then put the moveNumber into array
		if (is_nextpos_valid(next))	{
			num++;
			possible_move[num] = moveNumber;
			}
	}
	possible_move[0] = num;
	return num;
}

//find index that array[index] is smallest
int findmin(int array[], int start, int end)
{
	int smallest = start , i;
	for( i = start+1; i<= end; i++)
			if (array[smallest] > array[i]) smallest = i;
	return smallest;
}

//return the smallest Accessibility number of all next possible moves
//of current position
int getSmallestAccessValue(pos curr)
{
	int i, smallest, accessNext[9], possMove[9];
	pos nexta[9];

	get_possible_move(curr,possMove);
	//get all possible move into database nexta[]
	for  (i=1; i<=possMove[0]; i++)
	{
		nexta[i] = nextmove(curr, possMove[i]);
		//nexta[i] contains postion of this nextmove i
		accessNext[i] = accessibility[ nexta[i].x ][nexta[i].y];
		//accessNext contains the rate of this nextmove i
	}
	smallest = findmin(accessNext, 1, possMove[0]);
	return accessNext[smallest];
}

//if current position determine the best next move
pos determineNextMove(pos curr)
{
	int i,t1,t2, smallestIndex, accessNext[9], possMove[9];
	pos nexta[9];

//These codes till "---" are the same as getSmallestAccessValue
//but it stores coordinate and accessibility of all possible moves
//into nexta and accessNext
	get_possible_move(curr, possMove);
	for  (i=1; i<=possMove[0]; i++)	{
		//nexta[i] contains postion of this nextmove i
		nexta[i] = nextmove(curr, possMove[i]);
		accessNext[i] = accessibility[ nexta[i].x ][nexta[i].y];
		//accessNext contains the rate of this nextmove i
	}
	smallestIndex = findmin(accessNext, 1 , possMove[0]);
//------------------

	//if there are many smallest
	int base = smallestIndex;//because smallestIndex will be changed
	for  (i=1; i<=possMove[0]; i++)	{
	 //find another smallest value in accessNext
	 if ( (accessNext[i]==accessNext[base]) && (i!=base) )
	 {
		t1=getSmallestAccessValue(nexta[smallestIndex]);
		t2=getSmallestAccessValue(nexta[i]);
		if ( t1 > t2) smallestIndex = i;
	 }
	}//end for

  return nexta[smallestIndex];
}

//update the Accessibility
void updateAccessibility(pos curr)
{
	int possibleMove[9], i , possNum;
	pos NextPossibleMove[9];

	//get all possible moveNumber into possibleMove array
	possNum = get_possible_move(curr, possibleMove);
	for( i=1; i<= possNum; i++)	{
//get all possible move positions to NextPossibleMove array
	NextPossibleMove[i] = nextmove(curr, possibleMove[i]);
	//decrease by 1 the accessibility rate of those NextPossibleMove
	//because those next moves cannot move back to current position.
	-- accessibility [NextPossibleMove[i].x] [NextPossibleMove[i].y];
	}
}