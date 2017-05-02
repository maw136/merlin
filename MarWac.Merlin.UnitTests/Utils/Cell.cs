using System;

namespace MarWac.Merlin.UnitTests.Utils
{
    internal struct Cell
    {
        public Cell(int row, int column)
        {
            if (row < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(row), row, 
                    "Row must be integer greater equal 1.");
            }
            if (column < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(column), column, 
                    "Column must be integer greater equal 1.");
            }

            Row = row;
            Column = column;
        }

        public int Row { get; }

        public int Column { get; }
    }
}