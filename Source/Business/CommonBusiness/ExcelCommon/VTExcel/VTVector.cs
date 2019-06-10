using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VTUtils.Excel.Export
{
    public class VTVector
    {
        public int X { get; set; }
        public int Y { get; set; }

        public static Dictionary<char, int> dic = new Dictionary<char, int> {
            {'A', 1},
            {'B', 2},
            {'C', 3},
            {'D', 4},
            {'E', 5},
            {'F', 6},
            {'G', 7},
            {'H', 8},
            {'I', 9},
            {'J', 10},
            {'K', 11},
            {'L', 12},
            {'M', 13},
            {'N', 14},
            {'O', 15},
            {'P', 16},
            {'Q', 17},
            {'R', 18},
            {'S', 19},
            {'T', 20},
            {'U', 21},
            {'V', 22},
            {'W', 23},
            {'X', 24},
            {'Y', 25},
            {'Z', 26},
            {'a', 1},
            {'b', 2},
            {'c', 3},
            {'d', 4},
            {'e', 5},
            {'f', 6},
            {'g', 7},
            {'h', 8},
            {'i', 9},
            {'j', 10},
            {'k', 11},
            {'l', 12},
            {'m', 13},
            {'n', 14},
            {'o', 15},
            {'p', 16},
            {'q', 17},
            {'r', 18},
            {'s', 19},
            {'t', 20},
            {'u', 21},
            {'v', 22},
            {'w', 23},
            {'x', 24},
            {'y', 25},
            {'z', 26}
        };

        public static Dictionary<int, char> dicResverse = new Dictionary<int, char> {  
            {1, 'A'},
            {2, 'B'},
            {3, 'C'},
            {4, 'D'},
            {5, 'E'},
            {6, 'F'},
            {7, 'G'},
            {8, 'H'},
            {9, 'I'},
            {10, 'J'},
            {11, 'K'},
            {12, 'L'},
            {13, 'M'},
            {14, 'N'},
            {15, 'O'},
            {16, 'P'},
            {17, 'Q'},
            {18, 'R'},
            {19, 'S'},
            {20, 'T'},
            {21, 'U'},
            {22, 'V'},
            {23, 'W'},
            {24, 'X'},
            {25, 'Y'},
            {26, 'Z'}
        };

        public VTVector(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public VTVector(string cell)
        {
            string row = "";
            int col = 0;
            foreach (char c in cell)
            {
                if ('0' <= c && c <= '9')
                {
                    row =  row + c.ToString();
                }
                else
                {
                    col = 26*col + dic[c];
                }
            }
            this.X = Int32.Parse(row);
            this.Y = col;
        }

        public static VTVector operator +(VTVector v1, VTVector v2)
        {
            return new VTVector(v1.X + v2.X, v1.Y + v2.Y);
        }

        public override string ToString()
        {
            return ColumnIntToString(Y) + X;
        }

        public static string ColumnIntToString(int value)
        {
            string col = "";
            int val = value;
            while (val > 0)
            {
                if (val % 26 != 0)
                {
                    col += dicResverse[(val % 26)] + col;
                    val = val / 26;
                }
                else
                {
                    col += dicResverse[26] + col;
                    val = val / 26 - 1;
                }
            }
            return col;
        }
    }
}
