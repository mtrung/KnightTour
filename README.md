# README #

* Wiki: http://en.wikipedia.org/wiki/Knight%27s_tour
* Using Warnsdorf's rule

### History
* Current version: C# for Silverlight: http://www.inoapp.com/silverlight/knighttour.html
* 1st version: C++ for DOS console

### Old notes from DOS version
* Use Borland C++ Compiler Only

- One of the interesting puzzlers for chess buffs is the
Knight's Tour problem, originally proposed by the  mathematician Euler.
The question is: Can the chess piece called the knight move around
an empty chessboard and touch each of the 64 squares once and only once?

- An array 9x9 will be created to simulate the chessboard, subscript 0 will
be reserved for further development. Put a 1 in the first square you move
to, a 2 in the second square, a 3 in the third...

- Each potential move will be tested to make sure it doesn't land off the
board and doesn't revisit the old move.

- This program will use accessibility heuristic. Accessibility of a square
will be equal precisely to the number of squares from which that square
may be reached. At any time, the knight should move to the square with
the lowest accessibility.

- When encountering a tie in accessibility between two or more squares,
decides what square to choose by looking ahead to those squares reachable
from the "tied" squares. The program should move to the square for which
the next move would arrive at a square with the lowest accessibility number.