using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rullo
{
    public class Field
    {
        int count;
        Line[] h_lines;
        Line[] v_lines;
        Line[] lines;
        bool was_solved;

        public Line[] H_lines { get => h_lines; set => h_lines = value; }
        public Line[] V_lines { get => v_lines; set => v_lines = value; }
        public int Count { get => count; set => count = value; }
        public Line[] Lines { get => lines; set => lines = value; }
        public bool Was_solved { get => was_solved; set => was_solved = value; }

        public Field(int c, Button[,] l_sum, Button[,] l_table, TextBox[,] t_sum, TextBox[,] t_table)
        {
            Count = c;
            H_lines = new Line[c];
            V_lines = new Line[c];
            lines = new Line[2*c];
            //создание экземпляров
            for (int i = 0; i < c; i++)
            {
                Cell[] temp_cells_mas = new Cell[c];
                for (int j = 0; j < c; j++)
                {
                    temp_cells_mas[j] = new Cell(i, j, t_table[j, i], l_table[j, i]);
                }
                H_lines[i] = new Line(c, temp_cells_mas, l_sum[2, i], l_sum[3, i], t_sum[2, i], t_sum[3, i], true);
                lines[i] = H_lines[i];
            }
            for (int i = 0; i < c; i++)
            {
                Cell[] temp_cells_mas = new Cell[c];
                for (int j = 0; j < c; j++)
                {
                    temp_cells_mas[j] = H_lines[j].Get_cell(j, i);
                }
                V_lines[i] = new Line(c, temp_cells_mas, l_sum[0, i], l_sum[1, i], t_sum[0, i], t_sum[1, i], false);
                lines[c+i] = V_lines[i];
            }
        }

        public bool FullField(bool flag_internal=true)
        {
            //проверка поля на заполненность
            foreach (Line item in Lines)
            {
                if ((item.CalcSum(true) != item.Require_sum) && flag_internal || (item.CalcSum() != item.Require_sum) && !flag_internal)
                {
                    return false;
                }
            }
            return true;
        }

        //работа с просчетом ходов
        bool Find_cell_in_hashset(Cell item, HashSet<Cell> u)
        {
            foreach (Cell item2 in u)
            {
                if (item2 == item)
                {
                    return true;
                }
            }
            return false;
        }
        static IEnumerable<int[]> Combinations(int m, int n)
        {
            int[] result = new int[m];
            Stack<int> stack = new Stack<int>();
            stack.Push(0);

            while (stack.Count > 0)
            {
                int index = stack.Count - 1;
                int value = stack.Pop();

                while (value < n)
                {
                    result[index++] = ++value;
                    stack.Push(value);

                    if (index == m)
                    {
                        yield return result;
                        break;
                    }
                }
            }
        }
        List<HashSet<Cell>> Find_decay(Line r)
        {
            List<HashSet<Cell>> ret = new List<HashSet<Cell>>();
            for (int i = 1; i <= r.Cells.Count(); i++)
            {
                foreach (int[] c in Combinations(i, r.Cells.Count()))
                {
                    int s = 0;
                    foreach (int item in c)
                    {
                        s += r.Cells[item - 1].V;
                    }
                    if (s == r.Require_sum)
                    {
                        HashSet<Cell> temp = new HashSet<Cell>();
                        foreach (int item in c)
                        {
                            temp.Add(r.Cells[item - 1]);
                        }
                        ret.Add(temp);
                    }
                }
            }
            return ret;
        }
        int Solve_for_one_Line(Line r, bool flag_for_internal=false)
        {
            //проверка на ПОЛНОСТЬЮ заполненную линию
            bool flag_full_locked_line = true;
            foreach (Cell c in r.Cells)
            {
                if (!c.Locked && !flag_for_internal || !c.Internal_locked && flag_for_internal)
                {
                    flag_full_locked_line = false;
                    break;
                }
            }
            if (flag_full_locked_line) return 0;

            int count = 0;
            //проверка на не до конца заполненную линию - нулевую по разнице сумм - бывший метод 2
            int sum = r.Require_sum;
            foreach (Cell c in r.Cells)
            {
                if ((c.Marked && c.Locked && !flag_for_internal) || (c.Internal_locked && c.Internal_marked && flag_for_internal))
                {
                    sum -= c.V;
                }
            }
            if (sum == 0)
            {
                foreach (Cell c in r.Cells)
                {
                    if ((!c.Locked || !c.Marked) && !flag_for_internal)
                    {
                        c.Locked = true;
                        c.Marked = false;
                        count++;
                    }
                    if ((!c.Internal_locked || !c.Internal_marked) && flag_for_internal)
                    {
                        c.Internal_locked = true;
                        c.Internal_marked = false;
                        count++;
                    }
                }
                if (!flag_for_internal) Program.CallBackMy.callbackEventHandler("Дозаполняем полностью заполненную строку: " + r.ToString() + "\n");
                return count;
            }

            //выдираем нужные ячейки для составления "строки"
            int _sum = r.Require_sum;
            List<Cell> lst = new List<Cell>();
            foreach (Cell c in r.Cells)
            {
                if ((c.Locked && !flag_for_internal) || (c.Internal_locked && flag_for_internal))
                {
                    if ((c.Marked && !flag_for_internal) || (c.Internal_marked && flag_for_internal))
                    {
                        _sum -= c.V;
                    }
                }
                else
                {
                    lst.Add(c);
                }
            }

            //заполняем временную "строку"
            Line r_temp = new Line(lst.Count)
            {
                Require_sum = _sum
            };
            for (int i = 0; i < lst.Count; i++)
            {
                r_temp.Add(lst[i], i);
            }

            //анализируем составные множества
            List<HashSet<Cell>> t = Find_decay(r_temp);

            //единичный вариант дробления
            if (t.Count == 1)
            {
                //лочим единственный вариант, убираем пометки с других
                foreach (Cell item in r.Cells)
                {
                    if (Find_cell_in_hashset(item, t[0]))
                    {
                        if (!flag_for_internal)
                        {
                            item.Locked = true;
                            item.Marked = true;
                        }
                        else
                        {
                            item.Internal_locked = true;
                            item.Internal_marked = true;
                        }
                        count++;
                    }
                    else
                    {
                        if ((!item.Locked || !item.Marked) && !flag_for_internal)
                        {
                            item.Locked = true;
                            item.Marked = false;
                            count++;
                        }
                        if ((!item.Internal_locked || !item.Internal_marked) && flag_for_internal)
                        {
                            item.Internal_locked = true;
                            item.Internal_marked = false;
                            count++;
                        }
                    }
                }
                if (!flag_for_internal) Program.CallBackMy.callbackEventHandler("Единственный вариант дробления строки (" + r.ToString() + ")\n");
                return count;
            }

            if (t.Count > 1)
            {
                //объединение
                HashSet<Cell> union_temp = new HashSet<Cell>();
                foreach (HashSet<Cell> item in t)
                {
                    union_temp.UnionWith(item);
                }

                //убираем отметки и лочим все, чего нет в объединении
                bool flag_once = false;
                string s = "";
                string spart = "";
                bool not_first_iter2 = false;
                foreach (HashSet<Cell> item in t)
                {
                    if (not_first_iter2)
                    {
                        spart += "; ";
                    }
                    not_first_iter2 = true;
                    bool not_first_iter = false;
                    foreach (Cell item2 in item)
                    {
                        if (not_first_iter)
                        {
                            spart += ",";
                        }
                        not_first_iter = true;
                        if (r.Horiz)
                        {
                            spart += item2.ToY();
                        }
                        else
                        {
                            spart += item2.ToX();
                        }
                    }
                }

                foreach (Cell item in r_temp.Cells)
                {
                    if (!Find_cell_in_hashset(item, union_temp))
                    {
                        if (!flag_for_internal)
                        {
                            item.Locked = true;
                            item.Marked = false;
                        }
                        else
                        {
                            item.Internal_locked = true;
                            item.Internal_marked = false;
                        }
                        count++;
                        flag_once = true;
                        s += " " + item.ToString();
                    }
                }
                if (flag_once && !flag_for_internal) Program.CallBackMy.callbackEventHandler("Объединение множества всех возможных [" + t.Count + "] разбиений (" + spart + ") в строке: " + r.ToString() + " дает возможность исключить все, чего нет в объединении:" + s + "\n");

                //пересечение
                HashSet<Cell> cross_temp = new HashSet<Cell>();
                bool flag_first = true;
                foreach (HashSet<Cell> item in t)
                {
                    if (flag_first)
                    {
                        //Фактически, cross_temp = item; 
                        cross_temp.UnionWith(item);
                        flag_first = false;
                    }
                    else
                    {
                        cross_temp.IntersectWith(item);
                    }
                }

                //лочим в отмеченном состоянии все, что есть в исключении
                flag_once = false;
                s = "";
                spart = "";
                not_first_iter2 = false;
                foreach (HashSet<Cell> item in t)
                {
                    if (not_first_iter2)
                    {
                        spart += "; ";
                    }
                    not_first_iter2 = true;
                    bool not_first_iter = false;
                    foreach (Cell item2 in item)
                    {
                        if (not_first_iter)
                        {
                            spart += ",";
                        }
                        not_first_iter = true;
                        if (r.Horiz)
                        {
                            spart += item2.ToY();
                        }
                        else
                        {
                            spart += item2.ToX();
                        }
                    }
                }
                foreach (Cell item in r_temp.Cells)
                {
                    if (Find_cell_in_hashset(item, cross_temp))
                    {
                        if (!item.Marked && !flag_for_internal || !item.Internal_marked && flag_for_internal)
                        {
                            throw new Exception("ничесе, я такого не ожидал");
                        }
                        if (!flag_for_internal)
                        {
                            item.Locked = true;
                            item.Marked = true;
                        }
                        else
                        {
                            item.Internal_locked = true;
                            item.Internal_marked = true;
                        }
                        count++;
                        flag_once = true;
                        s += " " + item.ToString();
                    }
                }
                if (flag_once && !flag_for_internal) Program.CallBackMy.callbackEventHandler("Пересечение множества всех возможных [" + t.Count + "] разбиений (" + spart + ") в строке: " + r.ToString() + " дает возможность отметить все, что есть в пересечении:" + s + "\n");

            }
            return count;
        }

        //оболочки этих просчетов
        public void Solve()
        {
            foreach (Line r in lines)
            {
                if (Solve_for_one_Line(r) > 0) return;
            }
        }
        public bool CanSolveAllInternal(bool silent=true)
        {
            for (int i = 0; i < 20; i++)
            {
                int temp = 0;
                foreach (Line r in lines)
                {
                    temp += Solve_for_one_Line(r, silent);
                }
                if (temp < 1) break;
            }
            bool tempsolved = FullField(silent);
            Was_solved = tempsolved;
            return tempsolved;
        }

        //оболочки для интерфейса
        public TextBox GetTextBox(Button what)
        {
            foreach (Line iter in H_lines)
            {
                //для горизонтальных сумм
                if (iter.Bsum1 == what)
                {
                    return iter.Tsum1;
                }
                if (iter.Bsum2 == what)
                {
                    return iter.Tsum2;
                }
                //для ячеек
                foreach (Cell iter2 in iter.Cells)
                {
                    if (iter2.Btn == what)
                    {
                        return iter2.Txt;
                    }
                }
            }
            foreach (Line iter in V_lines)
            {
                //для вертикальных сумм
                if (iter.Bsum1 == what)
                {
                    return iter.Tsum1;
                }
                if (iter.Bsum2 == what)
                {
                    return iter.Tsum2;
                }
            }
            throw new System.ArgumentException("TextBox не найден", "original");
        }
        public Button GetButton(TextBox what)
        {
            foreach (Line iter in H_lines)
            {
                //для горизонтальных сумм
                if (iter.Tsum1 == what)
                {
                    return iter.Bsum1;
                }
                if (iter.Tsum2 == what)
                {
                    return iter.Bsum2;
                }
                //для ячеек
                foreach (Cell iter2 in iter.Cells)
                {
                    if (iter2.Txt == what)
                    {
                        return iter2.Btn;
                    }
                }
            }
            foreach (Line iter in V_lines)
            {
                //для вертикальных сумм
                if (iter.Tsum1 == what)
                {
                    return iter.Bsum1;
                }
                if (iter.Tsum2 == what)
                {
                    return iter.Bsum2;
                }
            }
            throw new System.ArgumentException("Button не найден, GetButton", "original");
        }
        public void FillValue(TextBox what, string v)
        {
            foreach (Line iter in H_lines)
            {
                //для горизонтальных сумм
                if (iter.Tsum1 == what || iter.Tsum2 == what)
                {
                    iter.Bsum1.Text = v;
                    iter.Bsum2.Text = v;
                    try
                    {
                        iter.Require_sum = Convert.ToInt32(v);
                    }
                    catch (Exception eee) { }
                }
                //для ячеек
                foreach (Cell iter2 in iter.Cells)
                {
                    if (iter2.Txt == what)
                    {
                        iter2.Btn.Text = v;
                        try
                        {
                            iter2.V = Convert.ToInt32(v);
                        }
                        catch (Exception eee) { }
                    }
                }
            }
            foreach (Line iter in V_lines)
            {
                //для вертикальных сумм
                if (iter.Tsum1 == what || iter.Tsum2 == what)
                {
                    iter.Bsum1.Text = v;
                    iter.Bsum2.Text = v;
                    try
                    {
                        iter.Require_sum = Convert.ToInt32(v);
                    }
                    catch (Exception eee) { }
                }
            }
        }
        public void SetMarked(Button what_cell, bool what_value)
        {
            foreach (Line iter in H_lines)
            {
                //для ячеек
                foreach (Cell iter2 in iter.Cells)
                {
                    if (iter2.Btn == what_cell)
                    {
                        iter2.Marked = what_value;
                    }
                }
            }
        }
        public bool GetMarked(Button what_cell)
        {
            foreach (Line iter in H_lines)
            {
                //для ячеек
                foreach (Cell iter2 in iter.Cells)
                {
                    if (iter2.Btn == what_cell)
                    {
                        return iter2.Marked;
                    }
                }
            }
            throw new System.ArgumentException("Button не найден, GetMarked", "original");
        }
        public void SetLocked(Button what_cell, bool what_value)
        {
            foreach (Line iter in H_lines)
            {
                //для ячеек
                foreach (Cell iter2 in iter.Cells)
                {
                    if (iter2.Btn == what_cell)
                    {
                        iter2.Locked = what_value;
                    }
                }
            }
        }
        public bool GetLocked(Button what_cell)
        {
            foreach (Line iter in H_lines)
            {
                //для горизонтальных сумм
                if (iter.Bsum1 == what_cell || iter.Bsum2 == what_cell)
                {
                    return iter.Sum_locked;
                }
                //для ячеек
                foreach (Cell iter2 in iter.Cells)
                {
                    if (iter2.Btn == what_cell)
                    {
                        return iter2.Locked;
                    }
                }
            }
            foreach (Line iter in V_lines)
            {
                //для вертикальных сумм
                if (iter.Bsum1 == what_cell || iter.Bsum2 == what_cell)
                {
                    return iter.Sum_locked;
                }
            }
            throw new System.ArgumentException("Button не найден, GetLocked", "original");
        }
        public Line GetLine(Button what_sum)
        {
            foreach (Line iter in H_lines)
            {
                //для горизонтальных сумм
                if (iter.Bsum1 == what_sum || iter.Bsum2 == what_sum)
                {
                    return iter;
                }
            }

            foreach (Line iter in V_lines)
            {
                //для вертикальных сумм
                if (iter.Bsum1 == what_sum || iter.Bsum2 == what_sum)
                {
                    return iter;
                }
            }

            throw new System.ArgumentException("Не может такого быть, Line по button не найден, GetLine", "original");
        }
        public int CalcCurrentSum(Button what_cell)
        {
            return GetLine(what_cell).CalcSum();
        }
        public void FillField(int _size, int[,] _cell, int[] _sum_v, int[] _sum_h)
        {
            for (int i = 0; i < _size; i++)
            {
                //для горизонтальных сумм
                H_lines[i].Require_sum = _sum_h[i];
                //для ячеек
                for (int j = 0; j < _size; j++)
                {
                    H_lines[i].Cells[j].V = _cell[i, j];
                    H_lines[i].Cells[j].Locked = false;
                    H_lines[i].Cells[j].Marked = true;
                }
                //для вертикальных сумм
                V_lines[i].Require_sum = _sum_v[i];
            }
        }
        public void Clear(bool values=true, bool internal_marks=true, bool marks=true)
        {
            foreach (Line item in Lines)
            {
                foreach (Cell item2 in item.Cells)
                {
                    if (internal_marks) item2.Internal_locked = false;
                    if (internal_marks) item2.Internal_marked = true;
                    if (marks) item2.Locked = false;
                    if (marks) item2.Marked = true;
                    if (values) item2.V = -1;
                }
                if (values) item.Require_sum = 5;
            }
        }
        public bool Check()
        {
            foreach (Line item in Lines)
            {
                foreach (Cell item2 in item.Cells)
                {
                    if (item2.Internal_marked && !item2.Marked || !item2.Internal_marked && item2.Marked && item2.Locked)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public bool CheckWrong()
        {
            foreach (Line item in Lines)
            {
                foreach (Cell item2 in item.Cells)
                {
                    if (item2.V < 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
