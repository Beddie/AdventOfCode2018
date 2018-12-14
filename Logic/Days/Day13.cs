using Logic.Interface;
using Logic.Properties;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Logic.Days
{
    public class Day13 : AdventBase
    {
        public Day13()
        {
            //Test = true;
            PuzzleInput = Test ? part2puzzle : puzzle;
            ID = 13;
            Name = "Day 13: Mine Cart Madness";
            trackLines = PuzzleInput.Replace("\r\n", "").ToCharArray();
            originalTrack = PuzzleInput.Replace("\r\n", "").Replace(">", "-").Replace("<", "-").Replace("^", "|").Replace("v", "|").ToCharArray();
            stride = PuzzleInput.Substring(0, PuzzleInput.IndexOf("\r\n")).Length;
        }

        public override string[] Solution()
        {
            return new string[] {
                "80,100",
                "16,99"
            };
        }

        private static char[] trackLines { get; set; }
        private static char[] originalTrack { get; set; }
        public static List<Direction> Directions = new List<Direction> { Direction.Up, Direction.Right, Direction.Down, Direction.Left };
        public static List<Direction> DirectionMemory = new List<Direction> { Direction.Left, Direction.Straight, Direction.Right };
        public static int stride { get; set; }

        private static Direction NextDirection(Direction direction)
        {
            var index = Directions.IndexOf(direction);
            return (index < Directions.Count - 1) ? Directions[++index] : Directions[0];
        }

        private static Direction PreviousDirection(Direction direction)
        {
            var index = Directions.IndexOf(direction);
            return (index > 0) ? Directions[--index] : Directions[Directions.Count - 1];
        }

        private static Direction NextMemory(Direction direction)
        {
            var index = DirectionMemory.IndexOf(direction);
            return (index < DirectionMemory.Count - 1) ? DirectionMemory[index + 1] : DirectionMemory[0];
        }

        public enum Direction
        {
            Up = 1,
            Right = 2,
            Down = 3,
            Left = 4,
            Straight = 5
        }

        public class Cart
        {
            public Cart(int index, char direction, int id)
            {
                Index = index;
                switch (direction)
                {
                    case '>':
                        facingDirection = Direction.Right;
                        previousTrack = '-';
                        break;
                    case 'v':
                        facingDirection = Direction.Down;
                        previousTrack = '|';
                        break;
                    case '<':
                        facingDirection = Direction.Left;
                        previousTrack = '-';
                        break;
                    case '^':
                        facingDirection = Direction.Up;
                        previousTrack = '|';
                        break;
                    default:
                        throw new NotSupportedException();
                }
                ID = id;
            }

            public int ID { get; set; }
            public int Index { get; set; }

            private bool horizontal => facingDirection == Direction.Right || facingDirection == Direction.Left;
            private Direction facingDirection { get; set; }
            private Direction memoryDirection { get; set; } = Direction.Left;
            private char previousTrack { get; set; }

            public int? Move()
            {
                var currentIndex = Index;

                SetNextIndex();
                var nextTrackSpecification = trackLines[Index];

                if ("<>^v".Contains(nextTrackSpecification))
                {
                    trackLines[Index] = originalTrack[Index];
                    trackLines[currentIndex] = originalTrack[currentIndex];
                    return Index;
                }

                switch (nextTrackSpecification)
                {
                    case '\\':
                        facingDirection = horizontal ? NextDirection(facingDirection) : PreviousDirection(facingDirection);
                        break;
                    case '/':
                        facingDirection = horizontal ? PreviousDirection(facingDirection) : NextDirection(facingDirection);
                        break;
                    case '+':
                        if (memoryDirection != Direction.Straight) facingDirection = (memoryDirection == Direction.Right) ? NextDirection(facingDirection) : PreviousDirection(facingDirection);
                        memoryDirection = NextMemory(memoryDirection);
                        break;
                    default:
                        break;
                }

                trackLines[currentIndex] = previousTrack;
                trackLines[Index] = GetCartChar();
                previousTrack = nextTrackSpecification;

                return null;
            }

            private char GetCartChar()
            {
                switch (facingDirection)
                {
                    case Direction.Up:
                        return '^';
                    case Direction.Right:
                        return '>';
                    case Direction.Down:
                        return 'v';
                    case Direction.Left:
                        return '<';
                    default:
                        throw new NotSupportedException();
                }
            }

            private void SetNextIndex()
            {
                switch (facingDirection)
                {
                    case Direction.Up:
                        Index -= stride;
                        break;
                    case Direction.Right:
                        Index++;
                        break;
                    case Direction.Down:
                        Index += stride;
                        break;
                    case Direction.Left:
                        Index--;
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public override string Part1()
        {
            var Carts = PuzzleInput.Replace("\r\n", "").Select((c, index) => new { c, index }).Where(d => d.c == '>' || d.c == 'v' || d.c == '<' || d.c == '^').Select((e, eindex) => new Cart(e.index, e.c, eindex + 1)).ToList();
            var coordinates = string.Empty;
            var crash = false;
            while (!crash)
            {
                foreach (var Cart in Carts)
                {
                    if (Cart.Move().HasValue)
                    {
                        crash = true;
                        coordinates = GetCoordinates(Cart.Index);
                    };
                }
                if (Test) Print();
            }
            return coordinates;
        }

        private string GetCoordinates(int index)
        {
            var ystride = trackLines.Length / stride;
            for (int y = 0; y < ystride; y++)
            {
                for (int x = 0; x < stride; x++)
                {
                    if (index == (x + (y * stride)))
                    {
                        return $"{x},{y}";
                    };
                }
            }
            return string.Empty;
        }

        private void Print()
        {
            // if (Test)
            // {
            var ystride = trackLines.Length / stride;
            var sb = new StringBuilder();
            for (int y = 0; y < ystride; y++)
            {
                for (int x = 0; x < stride; x++)
                {
                    sb.Append(trackLines[x + (y * stride)]);
                }
                sb.AppendLine();
            }
            Debug.WriteLine(sb.ToString());
        }

        public override string Part2()
        {
            var Carts = PuzzleInput.Replace("\r\n", "").Select((c, index) => new { c, index }).Where(d => d.c == '>' || d.c == 'v' || d.c == '<' || d.c == '^').Select((e, eindex) => new Cart(e.index, e.c, eindex + 1)).ToList();
            var coordinates = string.Empty;
            var crash = false;
            while (!crash)
            {
                List<int> cartsTobeRemoved = new List<int>();
                foreach (var Cart in Carts.OrderBy(c => c.Index))
                {
                    if (Cart.Move().HasValue)
                    {
                        cartsTobeRemoved.AddRange(Carts.Where(c => c.Index == Cart.Index).Select(c => c.ID).ToList());
                    };
                }
                Carts.RemoveAll(c => cartsTobeRemoved.Contains(c.ID));

                if (Carts.Count == 1)
                {
                    coordinates = GetCoordinates(Carts.First().Index);
                    crash = true;
                }
                if (Test) Print();
            }
            return coordinates;
        }


        private string puzzletest = @"/->-\        
|   |  /----\
| /-+--+-\  |
| | |  | v  |
\-+-/  \-+--/
  \------/   ";

        private string puzzle = @"XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX/----------------------------\XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
XX/-------------------------------+----------------------------+-----------------------\XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
XX|XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX|XXXXXXXXXXXXXXXXXXXXXXXXXXXX|XXXXXXXXXXXXXXXXXXXXX/-+------------------------------------------------\XXXXXXXXXXXXX
XX|XXXXXXXXXXXXXXX/---------------+----------------------------+---------------------+-+-\XXXXXXXXXXXXX/---------\XXXXXXXXXXXXXXXXXXXXXX|XXXXXXXXXXXXX
XX|XX/------------+------------\XX|XXXXXXXXXXXXXXXXXXXXXXXXXXXX|XXXXXXXXXXXXXXXXXXX/-+-+-+-------------+---------+----------------\XXXXX|XXXXXXXXXXXXX
XX|XX|XXXXXXXXXXXX|XXXXXXXXXXXX|XX|XXXXXXXXXXXXXXXXXXXXXXXXXXXX|XXXXXXXXXXXXXXXXXXX|X|X|X|XXXXXXXXXXXXX|XXXXXXXXX|XXXXXXXXXX/-----+---\X|XXXXXXXXXXXXX
XX|XX|XXXXXXXX/---+------------+--+-------------------------\XX|/------------------+-+-+-+-----------\X|XXXXXXXXX|XXXXXXXXXX|XXXXX|XXX|X|XXXXXXXXXXXXX
XX|XX|XXXXXXXX|XXX|XXXXXXXX/---+--+-------------------------+--++------------------+-+-+-+-----------+-+---------+---\XXXXXX|XX/--+---+-+------\XXXXXX
XX|XX|XXXXXXXX|XXX|XXXXXXXX|/--+--+-------------------------+--++------\XXXXXXXXXXX|X|X|X|XXXXXXXXXXX|X|XXXXXXXXX|XXX|XXXXXX|XX|XX|XXX|X|XXXXXX|XXXXXX
XX|XX|XXXXXXXX|XXX|XXXXXXXX||XX|XX|XXXXXXXXXXXXXXXXXXXXXXXXX|XX||XXXXX/+-----------+-+-+-+-----------+-+---------+---+------+--+--+---+-+------+-\XXXX
XX|XX|X/------+---+--------++--+--+-------------------------+--++-----++-----------+-+-+\|XXXXXXXXXXX|X|XXXXXXXXX|XXX|XXXXXX|XX|XX|XXX|X|XXXXXX|X|XXXX
XX|XX|X|XXXXXX|XXX|XXXXXXXX||XX|XX|XXXXXXXXXXXXXXXXXXXX/----+--++-----++-----------+-+-+++-----------+-+---------+---+------+--+--+---+\|XXXXXX|X|XXXX
XX|XX|X|XXXX/-+---+--------++--+--+--------------------+----+--++-<---++----------\|X|X|||XXXXXXXXXXX|X|/--------+---+-\XXXX|XX|XX|XXX|||XXXXXX|X|XXXX
XX|XX|X|XXXX|X|XX/+--------++--+--+--\XXXXXXXXXXXXXXX/-+----+--++-----++----------++-+-+++-----------+-++--------+---+-+----+--+--+---+++-----\|X|XXXX
XX|XX|X|XXXX|X|XX||XXXXXXX/++--+--+--+-----------\XXX|X|XXXX|XX||XXXXX||XX/-------++-+-+++-----------+-++--------+---+-+--\X|XX|XX|XXX|||XXXXX||X|XXXX
XX|X/+-+----+-+--++-------+++--+--+--+-----------+---+-+----+--++-----++--+-------++-+-+++-----------+-++-------\|XXX|X|XX|X|XX|XX|XXX|||XXXXX||X|XXXX
XX|X||X|XXXX|X|XX||XXXXXXX|||/-+--+--+-----------+---+-+----+--++-----++--+-------++-+-+++\XXXXXXXXXX|X||XXXXXXX||XXX|X|XX|X|XX|XX|XXX|||XXXXX||X|XXXX
XX|X||X|XXXX|X|XX||XXXXXXX||||X|XX|XX|XXXXXXXXXXX|XXX|X|XXXX|XX||XXXXX||XX|X/-----++-+-++++----------+\||XXXXXXX||XXX^X|X/+-+--+--+---+++-----++-+---\
XX|X||X|XXXX|X|XX||XXXXXXX||||X|XX|XX|XXXXXXXXXXX|XXX|X|XXXX|XX||XXXXX||XX|X|XXXXX||X|X||||XXXXXXXXXX||||XXXXXXX||XXX|X|X||X|XX|XX|XXX|||XXXXX||X|XXX|
XX|X||X|XXXX|X|XX||XXXXXXX||||X|XX|XX|XXXXXXXXXXX|XXX|X|XX/-+--++-----++--+-+-----++-+-++++---\XXXXXX||||XXXXXXX||XXX|X|X||X|XX|XX|XXX|||XXXXX||X|XXX|
XX|X||X|XXXX|X|XX||XXXXXXX||||X|XX|XX|XXXXXXXXXXX|XXX|X|XX|X|XX||XXXXX||XX|X|XXXXX||X|X||||XXX|XXXXXX||||XXXXXXX||XXX|X|X||X|XX|XX|XXX|||XXXXX||X|XXX|
XX|X||X|XXXX|X|XX||XXXXXXX||||X|XX|/-+-----------+---+-+--+-+--++-----++--+-+-----++-+-++++---+------++++-------++---+-+-++-+--+--+---+++-\XXX||X|XXX|
XX\-++-+----+-+--++-------++++-+--++-+-----------+---+-+--+-+--++-----++--+-+-----++-+-/|||XXX|XXXXXX||||XXXXXXX||XXX|X|X||X|XX|XX|XXX|||X|XXX||X|XXX|
XXXX||X|XXXX|X|XX||XXXXX/-++++-+--++-+-----------+---+-+--+-+--++-----++--+-+-----++-+--+++---+------++++-------++---+-+-++-+--+\X|XXX|||X|XXX||X|XXX|
XXXX||X|XXXX|X|XX||X/---+-++++-+--++-+-----------+---+-+--+-+--++--\XX||XX|X|XXXXX||X|XX|||XXX|XXXXXX||||XXXXXXX||XXX|X|X||X|XX||X|XXX|||X|XXX||X|XXX|
XXXX||X|X/--+-+--++-+---+-++++-+--++-+-----------+---+-+--+-+--++--+--++--+-+-----++-+-\|||XXX|XXXXXX||||XXXXXXX||XXX|X|X||X|XX||X|XXX|||X|XXX||X|XXX|
XXXX||X|X|XX|X|XX||X|XXX|X|||\-+--++-+-----------+---+-+--+-+--++--+--++--+-+-----++-+-+++/XXX|XXXXXX||||XXXXXXX||XXX|X|X||X|XX||X|XXX|||X|XXX||X|XXX|
XXXX||X|X|XX|X|XX||X|XXX|X|||XX|/-++-+---------\X|XXX|X|XX|X|XX||XX|XX||XX|X|XXXXX||X|X|||XXXX|XXXXXX||||XXXXXXX||XXX|X|X||X|XX||X|XXX||^X|XXX||X|XXX|
XX/-++-+-+--+-+--++-+---+-+++--++-++-+---------+-+---+-+--+-+--++--+--++--+-+-----++-+-+++----+-----\||||XXXXXXX||XXX|X|X||X|XX||X|XXX|||X|XXX||X|XXX|
XX|X||X|X|XX|X\--++-+---+-+++--++-++-+---------+-+---+-+--+-/XX||XX|XX||XX|X|XXXXX||X|/+++----+-----+++++-\XXXXX||XXX|X|X||X|XX||X|XXX|||X|XXX||X|XXX|
XX|X||X|X|XX|XXXX||X|XXX|X|||XX||X||X|XXXXXXXXX|X|XXX|X|XX|XXXX||XX|XX||XX|X|XXXXX||X|||||XXXX|XXXXX|||||X|XXXXX||XXX|X|X||X|XX||X|XXX|||X|XXX||X|XXX|
XX|X||X|X|XX|XXXX||X|XXX|X|||XX||X||X|XXXXXXXXX|X|XXX|X|XX|XXXX||XX|/-++--+-+-----++-+++++----+-----+++++-+-----++---+\|X||X|XX||X|XXX|||X|XXX||X|XXX|
XX|X||X|X|XX|XXXX||X|XXX|X|||XX||X||X|XXXXXXXXX|X|XXX|X|/-+----++--++-++--+-+-----++-+++++----+-----+++++-+-----++---+++-++-+-\||X|XXX|||X|XXX||X|XXX|
XX|X||X|X|XX|XXXX||X|XXX|X|\+--++-++-+---------+-+---+-++-+----++--++-++--+-+-----++-+++++----+-----+++++-+-----++---/||X||X|X|||X|XXX|||X|XXX||X|XXX|
XX|X|\-+-+--+----++-+---+-+-+--/|X||X|XXXXXXXXX|X|XXX|X||X|XX/-++--++-++--+-+-\XXX||X|||||XXXX|XXXXX|||\+-+-----+/XXXX||X||X|X|||X|XXX|||X|XXX||X|XXX|
XX|X|XX\-+--+----++-+---+-+-+---+-++-+---------+-+---+-++-+--+-++--++-++--+-+-+---++-+++/|XXXX|XXXXX|||X|X|XXXXX|/----++-++-+-+++\|XXX|||X|XXX||X|XXX|
XX|X|XXXX|XX|X/--++-+---+-+\|XXX|X||X|XXXXXXXXX|X|XXX|X||X|XX|X||XX||X||XX|X|X|XXX||X\++-+----+-----+++-+-+-----++----++-++-+-+++++---++/X|XXX||X|XXX|
/-+-+----+--+-+--++-+---+-+++--\|X\+-+---------+-+---+-++-+--+-/|XX||X||XX|X|X|XXX||XX||X|XXXX|XXXXX|||X|X|XXXXX||XXXX||X||X|X|||||XXX||XX|XXX||X|XXX|
|X|X|XXXX|XX|X|XX||X|XXX|X|||XX||XX|X|XXXXXXXXX|X|XXX|X||X|XX|XX|XX||X||XX|X|X|XXX||XX||X|XXXX|XXXXX|||X|X|XXXXX||XXXX||X||X|X|||||XXX||XX|XXX||X|XXX|
|X|X|XXXX|X/+-+--++-+---+-+++--++--+-+---------+-+---+-++-+--+-\|XX||X||XX|X|X|/--++--++-+----+-----+++-+-+-----++----++-++-+\|||||XXX||XX|XXX||X|XXX|
|X|X|XXXX|X||X|XX||X|XXX|X|||XX||XX|X|XX/------+-+---+-++-+--+-++--++-++--+-+-++--++--++-+----+-----+++-+-+---\X||XXXX||X||X|||||||XXX||XX|XXX||X|XXX|
|X|X|XXXX|X||X|XX||X|XXX|X|||XX||XX|X|XX|X/----+-+---+-++-+--+-++--++-++--+-+\||XX||XX||X|XXXX|XXXXX|||X\-+---+-++----+/X||X|||||||XXX||XX|XXX||X|XXX|
|X|X|XXX/+-++-+--++-+---+-+++--++--+-+--+-+----+-+---+-++-+--+-++--++-++--+-++++--++--++-+----+-----+++---+\XX|/++----+--++-+++++++--\||XX|XXX||X|XXX|
|X|X|XXX||X||X|XX||X|XXX|X|||XX||XX|X|XX|X|XXXX|X|XXX|X||X|XX|X||XX||X||XX|X||||XX||XX||X|XXXX|XXXXX|||XXX||XX||||XXXX|XX||X|||||||XX|||XX|XXX||X|XXX|
|X|X|XXX||X||X|XX||X|X/-+-+++--++--+-+--+-+----+-+---+-++-+--+-++--++-++--+\||||X/++--++-+----+-----+++--\||XX||||XXXX|XX||X|||\+++--+++--+---+/X|XXX|
|X|X|XXX||X||X|XX||X|X|X|X|||XX||XX|X|XX|X|XXXX|X|XXX|X||X\--+-++--++-++--++++++-+++--++-+----/XXXXX|||XX|||XX||||XXXX|XX||X|||X|||XX|||XX|XXX|XX|XXX|
|X|X|XXX||X|\-+--++-+-+-+-+++--++--+-+--+-+----+-+---+-++----+-++--++-++--++++++-+/|XX||X|XXXXXXXXXX|||XX|||XX||||XXXX|XX||X|||X|||XX|||XX|XXX|XX|XXX|
|X|X|XXX||X|/-+--++-+-+-+-+++--++--+-+--+-+----+-+---+-++----+-++--++-++--++++++-+-+--++-+---\XXXXXX|||XX|||XX||||XXXX|XX||X|||X|||XX|||XX|XXX|XX|XXX|
|X|X|XXX||X||/+--++-+-+-+-+++--++--+-+\X|X|XXXX|X|/--+-++----+-++--++-++--++++++-+-+--++-+---+-\XXXX|||XX|||XX||||XXXX|XX||X|||X|||XX|||XX|XXX|XX|XXX|
|X|X|XXX||X||||XX||X|X|X|X|||XX||XX|X||X|X|XX/-+-++>-+-++----+-++--++-++--++++++-+-+--++-+---+-+----+++--+++--++++----+--++-+++-+++--+++--+---+\X|XXX|
|X|X|X/-++-++++--++-+-+-+-+++--++--+\||X|X|XX|X|X||XX|X||XXXX|X||XX||X||XX|||||\-+-+--++-+---+-+----+++--+++--++++----+--++-+/|X|||XX|||XX|XXX||X|XXX|
|X^X|X|X||X||||XX||X|X|X|X|||XX||XX||||X|X|XX|X|X||XX|X\+----+-++--++-++--+++++--+-+--++-+---+-+----+++--+++--++++----+--++-+-+-+++--++/XX|XXX||X|XXX|
|X|X|X|X||/++++--++-+-+-+-+++--++--++++-+-+--+-+-++--+--+----+-++--++<++--+++++--+-+--++-+---+-+-\XX|||XX|||XX||||XXXX|XX||/+-+-+++--++---+--\||X|XXX|
|X|X|X|X|||||||XX||X|X|X|X|||XX||XX||||/+-+--+-+-++--+--+----+-++--++-++--+++++--+-+--++-+--\|X|X|XX|||XX|||XX||||XXXX|XX||||X|X|||XX||XXX|XX|||X|XXX|
|X|X|X|X|||||||XX||X|X|X|X|||XX||XX||||||X|XX|X|X||XX|XX|XXXX\-++--++-++--++++/X/+-+--++-+--++-+-+--+++--+++--++++----+-\||||X|X|||XX||XXX|XX|||X|XXX|
|X|X|X|X|||||||XX||X|X|X|X|||XX||XX||||||X|XX|X|X||XX|X/+------++--++-++\X\+++--++-+--++-+--++-+-+--+++--+++--++++----+-++/||X|X|||XX||XXX|XX|||X|XXX|
|X|X|X|X|||||||XX||X|X|X|X|||XX||XX||||||X|XX|X|X||XX|X||/-----++--++-+++--+++--++-+--++-+--++-+-+--+++--+++--++++----+-++-++-+-+++--++---+--+++\|XXX|
|X|X|X|X|||\+++--++-+-+-+-+++--++--++++++-+--+-+-++--+-+++-----/|XX||X|||XX|||XX||X|XX||X|XX||X|X|XX|||XX|||XX||||XXXX|X||X||X|X|||XX||XXX|XX|||||XXX|
|X|X|X|X|||X|||XX||X|X|X|X|||XX||XX||||||X|/-+-+-++--+-+++------+--++-+++--+++--++-+--++-+--++-+-+--+++--+++--++++----+-++-++-+-+++\X||XXX|XX|||||XXX|
|X|X|X|X|||X|\+--++-+-+-+-+++--++--+++/||X||X|X|X||XX|X|||XXXXXX|XX||X|||XX|||XX||X|X/++-+--++\|X|XX|||XX|||XX||||XXXX|X||X||X|X||||X||XXX|XX|||||XXX|
|X|X|X|X|||X|X|XX||X|X|X|X|||XX||XX|||X||X||X|X|X||XX|X|||XXXXXX|XX||X|||XX|||XX||X|X|||X|XX||||/+--+++--+++--++++--\X|X||X||X|X||||X||XXX|XX|||||XXX|
|X|X|X|X|||X|/+--++-+-+-+-+++--++\X|||X||X||X|X|X||XX|X|||XXXXXX|XX||X|||XX|||XX||X|X|||X|XX||||||XX|||XX|||XX||||XX|X|X||X||X|X||||X||XXX|XX|||||XXX|
|X|X|X|X|||X|||XX||X|X|X|X|||XX|||X|||X||X||/+-+-++--+\|||XXXXXX|XX||X|||XX|||XX||X|X|||X|X/++++++--+++--+++--++++--+-+-++-++\|X||||X||XXX|XX|||||XXX|
|X|X|X|X|||X|||XX||X|X|X\-+++--+++-+++-++-++++-+-++--+++++------+--++-+++--+++--++-+-+++-+-+++++++--+++--+++--++++--+-+-++-++++-/|||X||XXX|XX|||||XXX|
|X|X|X|X|||X|||XX||X|X|XXX|||XX|||X|||X||X\+++-+-++--+++++------+--++-+++--++/XX||X|X|||X|X|||||||XX|||XX|||XX||||XX|X|X||X||||XX|||X||XXX|XX|||||XXX|
|X|XvX|X|\+-+++--++-+-+---+++--+++-+++-++--+++-+-++--+++++------+--++-+++--++---++-+-++/X|X|||||||XX|||XX|||XX||||XX|X|X||X||||XX|||X||XXX|XX|||||XXX|
|X|X|X|X|X|X|||XX||X|X|XXX|||XX|||X|||X||XX|||X|X||XX|||\+------+--++-+++--++---++-+-++--+-+++++++--+++--+++--++++--+-+-++-+++/XX|||X||XXX|XX|||||XXX|
|X|X|X|X|X|X|||XX||X|/+---+++--+++-+++-++--+++\|X||XX|||X|XXXXXX|XX||X|||/>++---++-+-++--+-+++++++--+++--+++--++++--+-+-++-+++---+++-++--\|XX|||||XXX|
|X|X|X|X|X|X|||XX||/+++---+++--+++-+++-++--+++++\||XX|||X|XXXXXX|XX||X||||X||/--++-+-++--+-+++++++--+++--+++-\||||XX|X|X||X|||XXX|||X||XX||XX|||||XXX|
|X|X|X|/+-+-+++--++++++---+++--+++-+++-++--++++++++\X|||X|XXXXXX|XX||/++++-+++--++-+\||XX|X|||||||XX|||XX|||X|||||XX|X|X||X|||XXX|||X||XX||XX|||||XXX|
|X|X|X\++-+-+++--++++++---+++--+++-+/|X||XX|||||||||X|||X|XXXXXX|XX|||||||X|||XX||X||||XX|X|||||||/-+++--+++-+++++-\|X|X||X|||XXX|||X||XX||XX|||||XXX|
|X|X|XX||X|X|||XX||||||XXX|||XX|||X|X|X||XX|||||||||X|||X|XXXXX/+--+++++++-+++--++-++++--+-++++++++-+++--+++-+++++-++-+-++-+++---+++-++--++--+++++\XX|
|X|X|XX||X|X|||XX||||||XXX|||XX|||X\-+-++--+++++++++-+++-+-----++--+++++++-+++--++-++++--+-++++++++-+++--+++-+++++-++-+-++-+++---+++-++--+/XX||||||XX|
|X|X|XX||X|X|||XX||||||XXX|||XX|||XXX|X||XX|||||||||X|||X|XXXXX||XX|||||||X|||XX||X||||XX|X||||||||X|||XX|||X|||||X||X|X||X|||XXX|||X||XX|XXX||||||XX|
|X|X|XX||X|X||\--++++++---+/|X/+++---+-++--+++++++++-+++-+-----++--+++++++-+++--++-++++--+-++++++++-+++--+++-+++++-++-+-++-+++--\|||X||XX|XXX||||||XX|
|X\-+--++-+-++---++++++---+-+-++++---+-++--+++++++++-+++-+-----++--+++++++-+++--++-++++--+-++++++++-/||XX|||X|||||X||X|X||X|||XX||||X||XX|XXX||||||XX|
|XXX|XX||X|X||XXX||||||XXX|X|X||||XXX|X||XX|||||||||X|||X|XXXXX||/-+++++++-+++\X||X||||XX|X||||||||XX||XX|||X|||||X||X|X||X|||XX||||X||XX|XXX||||||XX|
|XXX|XX||X|X||XXX||||||XXX|X|X||||XXX|X||XX|||||||||X|||X|XXXXX|||X|||||||X||||X||X||||XX|X||||||||XX||XX|||X|||||X||X|X||X|||XX||||X||XX|XXX||||||XX|
|XXX|XX||X|X||XXX||||||XXX|X|X||||XXX|X||XX|||||||||X|||X|XXXXX|||X|||||||X||||X||X\+++--+-++++++++--++--+++-+++++-++-+-++-+++--++/|X||XX|XXX||||||XX|
|XXX|XX||X|X||XXX||||||XXX|X|X||||XXX|X||XX|||||||||X|||X|XXXXX|||X|||||||X||||X||XX|||XX|X||||||||XX||XX|||X|||||X||X|X||X|||XX||X|X||XX|XXX||||||XX|
|XXX|XX||X|X||XXX||||||XXX|X|X||||XXX|X||XX|||||||||X|||X|XXXXX|||X|||||||X||||X||XX|||XX|X\+++++++--++--+++-+++++-++-+-++-++/XX||X|X||XX|XXX||||||XX|
|XXX|XX||X|X||XXX||||||XXX|X|X||||X/-+-++--+++++++++-+++-+-----+++-+++++++-++++-++--+++--+--+++++++--++\X|||X|||||X||X|X||X||XXX||X|X||XX|XXX||||||XX|
|XXX|XX||X|X||XXX||||||XXX|X|X||||X|X|X||XX|\+++++++-+/|X\-----+++-+++++++-++++-++--+++--+--+++++++--+++-+++-+++++-++-+-++-++---++-+-++--+---+++/||XX|
|XXX|XX||X|X||X/-++++++---+-+\||||X|X|X||X/+-+++++++-+-+\XXXXXX|||X|||\+++-++++-++--+++--+--+++++++--+++-+++-+++++-++-+-++-++---++-+-++--+---+++-/|XX|
|XXX|/-++-+-++-+-++++++---+-++++++-+-+-++-++-+++++++-+\||XXXXXX|||X|||X||vX||||X||XX|||XX|XX|||||||XX|||X|||X|||||X||X|X||X||XXX||X|X||XX|XXX|||XX|XX|
|XXX||X||X|X||X|X||||||XXX|X||||||X|X|X||X||X|||||||X||||XXXXXX|||X|\+-+++-++++-++--+++--+--+++++++--+++-+++-+++++-++-/X^|X||XXX||X|X||XX|XXX|||XX|XX|
|XXX||X||X|X||X|X||||||XXX|X||||\+-+-+-++-++-++/||||X||||XXXXXX|||X|X|X|||X||\+-++--+++--+--+++++++--+++-+++-/||||X||XXX||X||XXX||X|X||XX|XXX|||XX|XX|
|XXX||X||X|X||X|X||||||XXX|X||||X|X|X|X||X||X||/++++-++++---\XX|||X|X|X|||X||X|X||XX|||XX|XX|||||||XX|||X|||XX|||\-++---++-++---+/X|X||XX|XXX|||XX|XX|
|XXX||X|\-+-++-+-++++++---+-++++-+-+-+-++-++-+++++++-++++---+--+++-+-+-+++-++-+-++--+++--+--+++++++--+++-++/XX|||XX||XXX||X||XXX|XX|X||XX|XXX|||XX|XX|
|XXX||X|XX|X||X|X||||||XXX|X||||X|X|X|X||X||X|||||||X\+++---+--+++-+-+-+++-++-+-++--+++--+--+++++++--+++-++---+++--++---++-++---+--+-++--+---+/|XX|XX|
|XXX||X|XX|X||X|X||||||XXX\-++++-+-+-+-++-++-++++/||XX|||XXX|XX|||X|X|X|||X||X|X||XX|||XX|XX|||||||XX|||X||XXX|||XX||XXX||X||XXX|XX|X||XX|XXX|X|XX|XX|
|XXX||X|XX|X||X|X||||||XXXXX||||X|X|X|X||X||X||||X||XX|||XXX|XX|||X|X|X|||X||X|X||XX|||XX|XX||||\++--+++-++---+++--+/XXX||X||XXX|XX|X||XX|XXX|X|XX|XX|
|XXX||X|XX|X||X|X||||||XXXXX||||X|X|X|X||X||X||||X||XX|||XXX|XX|||X|X|X|||X||X|X|\--+++--+--++++-++--+++-/|XXX|||XX|XXXX||X||XXX|XX|X||XX|XXX|X|XX|XX|
|XXX||X|XX|X||X|X||||||XXXXX||||X|X|X|X||X||X||||X||XX|||XXX|XX|||X|X|/+++-++-+\|XXX|||XX|XX||||X||XX|||XX|XXX|||XX|XXXX||X||XXX|XX|X||XX|XXX|X|XXvXX|
|XXX||X|XX\-++-+-++++++-----++++-+-+-+-++-++-++++-++--+++---+--+++-+-+++++-++-+++---+++--+--++++-/|XX|||XX|XXX|||XX|XXXX|\-++---+--+-++--+---+-+--+--/
|XXX|\-+----++-+-++++++-----++++-+-+-+-++-++-++++-++--/||XXX|XX|||X|X|||||X||X|||XXX|||XX|XX||||XX|XX|||XX|XXX|||XX|XXXX|XX||XXX|XX|X||XX|XXX|X|XX|XXX
|XXX|XX|X/--++-+-++++++-----++++-+-+-+-++-++-++++-++-\X||XXX|XX|||X|X\++++-++-+++---/||XX|XX||||XX|XX|||XX|XXX|||XX|XXXX|XX|\---+--+-+/XX|XXX|X|XX|XXX
|XXX|XX|X|XX||X|X||||||/----++++-+-+-+-++\||X||||X||X|X||XXX|XX\++-+--++++-++-+++----++--+--++++--+--+++--+---+++--+----+--+----+--+-+---+---+-+--/XXX
|XXX|XX|X|/-++-+-+++++++----++++-+-+-+-+++++-++++-++-+-++---+---++-+--++++-++-+++\XXX||XX|XX||||XX|XX|||XX|XXX|||XX|XXXX|XX|XXXX|XX|X|XXX|XXX|X|XXXXXX
|XXX|XX|X||/++-+-+++++++----++++-+-+-+-+++++\||||X||X|X||XXX|XXX||X|XX||||X||X||||XXX||XX|XX||||XX|XX|||XX|XXX|||XX|XXXX|XX|XXXX|XX|X|XXX|XXX|X|XXXXXX
|XXX|XX|X|||||X|X\++++++----++++-+-+-/X||||||||||X||/+-++---+---++-+--++++-++-++++---++--+--++++--+-\|||XX|XXX|||XX|XXXX|XX|XXXX|XX|X|XXX|XXX|X|XXXXXX
|XXX|XX|X|||||X\--++++++----+/||X|X|XXX\+++++++++-++++-++---+---++-+--++++-++-++++---++--+--/|||XX|X||||XX|XX/+++--+----+-\|XXXX|XX|X|XXX|XXX|X|XXXXXX
|XXX\--+-+++++----++++++----+-++-+-+----+++++++++-++++-++---+---++-+--++++-++-++++---++--+---+++--+-++++--+--+++/XX|XXXX|/++----+--+-+---+---+\|XXXXXX
|XXXXXX|X|||||XXXX||||||XXXX\-++-+-+----+++++++++-++++-++---+---++-+--+/||X|\-++++---++--+---+++--+-++/|XX|XX|||XXX|XXXX||||XXXX|XX|X|XXX|XXX|||XXXXXX
|XXXXXX|X|||||XXXX||\+++------++-+-+----+++++++++-++++-++---+---++-/XX|/++-+--++++---++--+---+++--+-++-+--+--+++---+----++++----+--+-+-\X|XXX|||XXXXXX
|XXXXX/+-+++++---\||X|||XXXXXX||X|X|XXXX|||||||||/++++-++---+---++----++++-+--++++---++--+---+++--+-++-+--+--+++---+----++++-\XX|XX|X|X|X|XXX|||XXXXXX
|XXXXX||X|||||XXX|||X|\+------++-+-+----++++++++++++++-++---+---++----++++-/XX||||XXX||XX|XXX|||XX|X||X|XX|XX|||XXX|XXXX||||X|XX|XX|X|X|X|XXX|||XXXXXX
|XXXXX||X|||||XXX|||X|X|XXXXXX||X|X|XXXX|||\++++++++++-++---+---++----++++----++++---++--+---+++--+-++-+--+--+++---+----++++-+--+--/X|X|X|XXX|||XXXXXX
|XXXXX||X|||||XXX|\+-+-+------++-+-+----+++-++++++++++-++---+---++----++++----++++---++--/XXX|||XX|X||X|XX|XX|||XXX|XXXX||||X|XX|XXXX|X|X|XXX|||XXXXXX
|XXXXX||X|||||XXX|X|X|X|XXXXXX||X|X|XXXX|||X||||||||||X||XXX|XXX\+----++++----++++---++------+++--+-+/X|XX|XX|||XXX|XXXX||||X|XX|XXXX|X|X|XXX|||XXXXXX
|XXXXX||X|||||XXX|X|X|X|XXXXXX||X|X|XXXX\++-++++++++++-++---+----+----++++----++++---++------+++--+-+--+--+--+/|XXX|XXXX||||X|XX|XXXX|X|X|XXX|||XXXXXX
|XXXXX||X|||||XXX|X|X|X|XXXXXX||X|X|XXXXX||X||||||||||X||XXX|XXXX|XXXX||||XXXX||||XXX||XXXXXX|||XX|X|XX|XX|XX|X|XXX|XXXX||||X|XX|XXXX|X|X|XXX|||XXXXXX
|XXXXX||X|||||XXX|X|X|X|XXXXXX||X|X|XXXXX||X||||||||||X||XXX|XXXX|XXXX||||XXXX||||XXX||XXXXXX|||XX|X|XX|XX|XX|X|XXX|XXXX||||X|XX|XXXX|X|X|XXX|||XXXXXX
|XXXXX|\-+++++---+-+-+-+------++-+-+-----++-+++++++/||X||XXX|XXXX|XXXX||||XXXX||||XXX||XXXXXX|||XX|X|XX|XX|XX|X|XXX|XXXX||||X|XX|XXXX|X|X|XXX|||XXXXXX
|XXXXX|XX|||||XXX|X|X|X|XXXXXX||X|X|XXXXX|\-+++++++-++-+/XXX|XXXX|XXXX||||XXXX||||XXX||XXXXXX|||XX|X|XX|XX|XX|X|XXX|XXXX||||X|XX|XXXX|X|X|XXX|||XXXXXX
|XXXXX|XX|||||XXX|X|X|X|XXXXXX||X|X|XXXXX|XX|||||||X\+-+----+----+----++++----++++---++------+++--+-/XX|XX|XX|X|XXX|XXXX||||X|/-+----+-+-+---+++---\XX
|XXXXX|XX|||||XXX|X|X|X|XXXXXX\+-+-+-----+--+++++++--+-+----+----+----++++----++++---++------+++--+----+--+--+-+---+----++++-++-/XXXX|X|X|XXX|||XXX|XX
|XXXXX|XX|||||XXX|X|X|X|XXXXXXX|X|X|XXXXX|XX|||||||XX|/+----+----+----++++----++++---++------+++--+----+--+--+-+---+----++++-++------+-+-+---+++---+\X
|XXXXX|XX|||||XXX|X|X|X|XXXXXXX|X|X|XXXXX|XX|||||||XX|||XXXX|XXXX|XXXX|\++----++++---++------+++--+----+--+--+-+---+----++++-++------+-/X|XXX|||XXX||X
|XXXXX|XX\++++---+-+-+-+-------+-+-+-----+--+++++++--/||XXXX|XXXX|XXXX|X||/---++++---++------+++--+----+--+--+-+---+----++++-++--\XXX|XXX|XXX|||XXX||X
|XXXXX|XXX||||XXX|X|X|X|XXXXXXX|X|X|XXXXX|XX|||||||XXX||XXXX|XXXX|XXXX|X|||XXX||\+---++--<---+++--+----+--+--+-+---+----/|||X||XX|XXX|XXX|XXX|||XXX||X
|XXXXX|XXX||||XXX|X|X|X|XXXXXXX|X|X|XXXXX|XX|\+++++---++----+----+----+-+++---++-+---++------+++--+----+--+--+-+---+-----+++-++--+---+---+---++/XXX||X
|XXXXX|XXX||||XXX|X|X|X|XXXXXXX|X|X|XXXXX|XX|X|||||XXX||XXXX|XXXX|XXXX|X|||XXX||X|XXX||XXXXXX|||XX|XXXX|XX|XX|X|XXX|XXXXX|||X||XX|XXX|XXX|XXX||XXXX||X
|XXXXX|XXX||||XXX|X|X|X|XXXXXXX|X|X|XXXXX|XX|X|||||XXX||XXXX|XXXX|XXXX|X|||XXX||X|XXX\+------+/|XX|XXXX|XX|XX|X|XXX|XXXXX|||X||XX|XXX|XXX|XXX||XXXX||X
|XXXXX|XXX||||XXX|X|X|X|XXXXXXX|X|X|XXXXX|XX|X|||||XXX||XXXX|XXXX|XXXX|X|||XXX||X|XXXX|XXXXXX|X|XX|XXXX|XX|XX|X|XXX|XXXXX|||X||XX|XXX|XXX|XXX||XXXX||X
|XXXXX|XXX\+++---+-+-+-+-------+-+-+-----+--+-+++++---++----+----+----+-+++---++-/XXXX|XXXXXX|X|XX|XXXX|XX|XX|X|XXX|XXXXX|||X||XX|XXX|XXX|XXX||XXXX||X
|XXXXX|XXXX|||XXX|X|X|X|XXXXXXX|X|X|XXX/-+--+-+++++---++----+----+----+-+++---++------+------+-+--+----+--+\X|X|XXX|XXXXX|||X||XX|XXX|XXX|XXX||XXXX||X
|XXXXX|XXXX||\---+-+-+-+-------+-/X|XXX|X|XX|X|\+++---++----/XXXX\----+-+++---/|XXXXXX|XXXXXX|X|XX|XXXX|XX||X|X|XXX|XXXXX|||X||XX|XXX|XXX|XXX||XXXX||X
|XXXXX|XXXX||X/--+-+-+-+-------+---+\XX|X|XX|X|X|||XXX||XXXXXXXXXXXXXX|X|||XXXX|XXXXXX\------+-+--+----+--/|X\-+---+-----+/|X||XX|XXX|XXX|XXX||XXXX||X
|XXXXX|XXXX\+-+--+-+-+-+-------+---++--+-+--/X|X|||XXX||XXXXXXXXXXXXXX|X|||XXXX|XXXXXXXXXXXXX|X|XX|XXXX|XXX|XXX|XXX|XXXXX|X|X||XX|XXX|XXX|XXX||XXXX||X
|XXXXX|XXXXX|X|XX|X|/+-+-------+---++--+-+----+-+++---++--------\XXXXX|X|||XXXX|XXXXXXXXXXXXX|X|XX|XXXX|XXX|XXX|XXX|XXXXX|X|X||XX|XXX|XXX|XXX||XXXX||X
|XXXXX|XXXX/+-+--+-+++-+-------+---++--+-+----+-+++---++--------+-----+-+++----+----------->-+-+--+----+---+---+\XX|XXXXX|X|X|\--+---+---+---++----/|X
|XXXXX|XXXX||X|XX|X|||X|X/-----+--\||XX|X|XXXX|X|||XXX||XXXXXXXX|XXXXX|X|||XXXX|XXXXXXXXXXXXX|X|XX|X/--+---+---++--+-----+-+-+---+--\|XXX|XXX||XXXXX|X
|XXXXX|XXXX||X|XX|X|||X|X|XXXXX|XX|||XX|X|XXXX|X|||XXX||XXXXXXXX|XXXXX|X||\----+-------------+-+--+-+--+---+---++--+-----+-+-+---/XX||XXX|XXX||XXXXX|X
|XXXXX|XXXX||X|XX|X|||X|X|XXXXX|XX|||XX|X|XXXX|X|||XXX||XXXXXXXX|XXXXX|X||XXXXX|XXXXXXXXXXXXX|X|XX|X|XX|XXX|XXX||XX|XXXXX|X|X|XXXXXX||XXX|XXX||XXXXX|X
\-----+----++-+--+-+++-+-+-----/XX|||XX|X|XXXX|X|||XXX||XXXXXXXX|XXXXX|X||XXXXX|XXXXXXXXXXXXX|X|XX\-+--+---+---++--/XXXXX|X|X|XXXXXX||XXX|XXX||XXXXX|X
XXXXXX|XXXX||X|XX|X||\-+-+--------+++--+-+----/X|||XXX||XXXXXXXX|XXXXX|X||XXXXX|XXXXXXXXXXXXX|X|XX/-+--+---+---++--------+-+-+------++---+---++\XXXX|X
XXXXXX|XXXX||X|XX|X||XX|X|XXXXXXXX|||XX|X|XXXXXX|||XXX||XXXXXXXX|XXXXX|X||XXXXX|XXXXXXXXXXXXX|X|XX|X|XX|XXX|XXX||XXXXXXXX|X|X|XXXXXX||XXX|XXX|||XXXX|X
XXXXXX\>---++-+--/X||XX|X|XXXXXXXX|||XX\-+------+++---++--------+-----+-++-----+-------------+-+--+-+--+->-/XXX||XXXXXXXX\-+-+------++---+---+/|XXXX|X
XXXXXXXXXXX||X\----++--+-+--------++/XXXX|XXXXXX|\+---++--------+-----+-++-----+-------------+-+--+-+--+-------++----------+-/XXXXXX||XXX|XXX|X|XXXX|X
XXXXXXXXXXX||XXXXXX||XX|X|XXXXXXXX||XXXXX|XXXXXX|X|XXX\+--------+-----+-++-----+-------------+-+--+-+--+-------++----------+--------++---+---+-+----/X
XXXXXXXXXXX||XXXXXX||XX|X|XXXXXXXX||XXXXX|XXXXXX|X\----+--------+-----+-++-----+-------------+-/XX|X|XX|XXXXXXX||XXXXXXXXXX|XXXXXXXX||XXX|XXX|X|XXXXXX
XXXXXXXXXXX|\------++--+-+--------++-----+------+------+--------+-----+-++-----+-------------/XXXX|X|XX|XXXXXXX||XXXXXXXXXX\--------++---+---/X|XXXXXX
XXXXXXXXXXX|XXXXXXX|\--+-+---<----++-----+------+------+--------/XXXXX|X||XXXXX|XXXXXXXXXXXXXXXXXX|X|XX|XXXXXXX||XXXXXXXXXXXXXXXXXXX||XXX|XXXXX|XXXXXX
XXXXXXXXXXX|XXXXXXX|XXX|X\--------/|XXXXX|XXXXXX|XXXXXX\--------------+-/|XXXXX|XXXXXXXXXXXXXXXXXX|X\--+-------++-------------------/|XXX|XXXXX|XXXXXX
XXXXXXXXXXX|XXXXXXX|XXX|XXXXXXXXXXX\-----+------+---------------------+--+-----+-------<----------+----/XXXXXXX\+--------------------/XXX|XXXXX|XXXXXX
XXXXXXXXXXX|XXXXXXX\---+-----------------+------/XXXXXXXXXXXXXXXXXXXXX|XX|XXXXX|XXXXXXXXXXXXXXXXXX|XXXXXXXXXXXXX|XXXXXXXXXXXXXXXXXXXXXXXX|XXXXX|XXXXXX
XXXXXXXXXXX|XXXXXXXXXXX|XXXXXXXXXXXXXXXXX|XXXXXXXXXXXXXXXXXXXXXXXXXXXX\--+-----/XXXXXXXXXXXXXXXXXX\-------------+------------------------+-----/XXXXXX
XXXXXXXXXXX\-----------+-----------------+-------------------------------+--------------------------------------/XXXXXXXXXXXXXXXXXXXXXXXX|XXXXXXXXXXXX
XXXXXXXXXXXXXXXXXXXXXXX\-----------------/XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX\---------------------------------------------------------------/XXXXXXXXXXXX";

        private string ttpuzzle = @"| | |   || || |  || | /-+-+++--++--+-+--+-+----+-+---+-++-+--+-++--++-++--+\|||| /++--++-+----+-----+++--\||  ||||    |  || |||\+++--+++--+---+/ |   |
| | |   || || |  || | | | |||  ||  | |  | |    | |   | || \--+-++--++-++--++++++-+++--++-+----/     |||  |||  ||||    |  || ||| |||  |||  |   |  |   |
| | |   || |\-+--++-+-+-+-+++--++--+-+--+-+>-<-+-+---+-++----+-++--++-++--++++++-+/|  || |          |||  |||  ||||    |  || ||| |||  |||  |   |  |   |
| | |   || |/-+--++-+-+-+-+++--++--+-+--+-+----+-+---+-++----+-++--++-++--++++++-+-+--++-+---\      |||  |||  ||||    |  || ||| |||  |||  |   |  |   |
| | |   || ||/+--++-+-+-+-+++--++--+-+\ | |    | |/--+-++----+-++--++-++--++++++-+-+--++-+---+-\    |||  |||  ||||    |  || ||| |||  |||  |   |  |   |
| | |   || ||||  || | | | |||  ||  | || | |  /-+-++--+-++----+-++--++-++--++++++-+-+--++-+---+-+----+++--+++--++++----+--++-+++-+++--+++--+---+\ |   |
| | | /-++-++++--++-+-+-+-+++--++--+\|| | |  | | ||  | ||    | ||  || ||  |||||\-+-+--++-+---+-+----+++--+++--++++----+--++-+/| |||  |||  |   || |   |
| | | | || ||||  || | | | |||  ||  |||| | |  | | ||  | \+----+-++--++-++--+++++--+-+--++-+---+-+----+++--+++--++++----+--++-+-+-+++--++/  |   || |   |
| | | | ||/++++--++-+-+-+-+++--++--++++-+-+--+-+-++--+--+----+-++--+--++--+++++--+-+--++-+---+-+-\  |||  |||  ||||    |  ||/+-+-+++--++---+--\|| |   |
| | | | |||||||  || | | | |||  ||  ||||/+-+--+-+-++--+--+----+-++--++-++--+++++--+-+--++-+--\| | |  |||  |||  ||||    |  |||| | |||  ||   |  ||| |   |";

        private string part2puzzle = @"/>-<\  
v   |  
| /<+-\
| v | |
\>+</ |
  |   ^
  \<->/ ";
    }
}

