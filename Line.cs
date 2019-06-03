using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rullo
{
    public class Line
    {
        int count = 0;
        Cell[] cells;
        int require_sum;
        bool sum_locked;
        bool horiz;

        TextBox tsum1;
        TextBox tsum2;
        Button bsum1;
        Button bsum2;

        public Cell[] Cells { get => cells; set => cells = value; }
        public Button Bsum1 { get => bsum1; set => bsum1 = value; }
        public Button Bsum2 { get => bsum2; set => bsum2 = value; }
        public TextBox Tsum2 { get => tsum2; set => tsum2 = value; }
        public TextBox Tsum1 { get => tsum1; set => tsum1 = value; }
        public int Require_sum { get => require_sum; set => require_sum = value; }
        public bool Sum_locked { get => sum_locked; set => sum_locked = value; }
        public bool Horiz { get => horiz; set => horiz = value; }

        public int CalcSum(bool silent=false)
        {
            int ret = 0;
            foreach (Cell item in Cells)
            {
                if (item.Marked && !silent || silent && item.Internal_marked)
                {
                    ret += item.V;
                }
            }
            return ret;
        }

        public Line(int size)
        {
            cells = new Cell[size];
            require_sum = 0;
        }

        public void Add(Cell c, int pos)
        {
            cells[pos] = c;
        }
        public Line(int s, Cell[] c, Button l_sum1, Button l_sum2, TextBox t_sum1, TextBox t_sum2, bool d)
        {
            count = c.Length;
            Require_sum = s;
            Cells = c;
            Tsum1 = t_sum1;
            Tsum2 = t_sum2;
            Bsum1 = l_sum1;
            Bsum2 = l_sum2;
            Horiz = d;
        }
        public Cell Get_cell(int x, int y)
        {
            foreach (Cell iter in Cells)
            {
                if (iter.Get_pos() == new System.Drawing.Point(x, y))
                {
                    return iter;
                }
            }
            throw new System.ArgumentException("Ячейка не найдена", "original");
        }
        public override string ToString()
        {
            string s = Require_sum + " |";
            foreach (Cell item in Cells)
            {
                s += " " + item.V;
            }
            return s;
        }
    }
}
