using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rullo
{
    public class Cell
    {
        int x;
        int y;
        int v;

        TextBox txt;
        Button btn;

        bool marked;
        bool locked;
        bool internal_marked;
        bool internal_locked;

        public Cell(int _x, int _y, TextBox t, Button l)
        {
            x = _x;
            y = _y;
            try
            {
                V = Convert.ToInt32(l.Text);
            }
            catch (Exception e)
            {
                V = -1;
            }
            Marked = true;
            Locked = false;
            Internal_marked = true;
            Internal_locked = false;
            Btn = l;
            Txt = t;
        }

        public Button Btn { get => btn; set => btn = value; }
        public TextBox Txt { get => txt; set => txt = value; }
        public int V { get => v; set => v = value; }
        public bool Marked { get => marked; set => marked = value; }
        public bool Locked { get => locked; set => locked = value; }
        public bool Internal_marked { get => internal_marked; set => internal_marked = value; }
        public bool Internal_locked { get => internal_locked; set => internal_locked = value; }

        public System.Drawing.Point Get_pos()
        {
            return new System.Drawing.Point(x, y);
        }

        public override string ToString()
        {
            return "[" + x + "," + y + "]:" + v;
        }
        public string ToX()
        {
            return v + "[" + x + "]";
        }
        public string ToY()
        {
            return v + "[" + y + "]";
        }
    }
}
