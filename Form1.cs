using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rullo
{
    public partial class Form1 : Form
    {
        public TextBox[,] textbox_sum_mas;
        public TextBox[,] textbox_table_mas;
        public Button[,] button_sum_mas;
        public Button[,] button_table_mas;
        public Field field;
        public Form1()
        {
            InitializeComponent();
            Program.CallBackMy.callbackEventHandler = new Program.CallBackMy.callbackEvent(this.Log_updating);

            Generate(1, 9);
        }
        private void Create()
        {
            //сначала - вычисление размеров и рамок
            int delta = 8;
            int _w = 35;
            int _h = _w;
            
            //общий регион
            System.Drawing.Drawing2D.GraphicsPath myPath = new System.Drawing.Drawing2D.GraphicsPath();
            myPath.AddEllipse(0, 0, _w-1, _h-1);
            Region myRegion = new Region(myPath);
            
            //конструирование
            for (int i = 0; i < numericUpDown1.Value; i++)
            {
                //суммы
                for (int j = 1; j < 5; j++)
                {
                    button_sum_mas[j - 1, i] = new Button()
                    {
                        ForeColor = Color.White,
                        Size = new Size(_w, _h),
                        FlatStyle = FlatStyle.Flat,
                        BackColor = Color.FromArgb(255, 23, 26, 41),
                        Tag = "sum",
                        TabStop = false,
                        TextAlign = ContentAlignment.MiddleCenter
                    };
                    textbox_sum_mas[j - 1, i] = new TextBox()
                    {
                        Visible = false,
                        Size = new Size(_w, _h),
                        Multiline = true,
                        MaxLength = 2,
                        Tag = "sum",
                        BorderStyle = BorderStyle.FixedSingle,
                        TextAlign = HorizontalAlignment.Center
                    };
                    switch (j)
                    {
                        case 1:
                            textbox_sum_mas[j - 1, i].Text = "w " + i + " " + (j - 1);
                            textbox_sum_mas[j - 1, i].Location = new Point(((i + 1) * _w) + ((i + 2) * delta), delta);
                            break;
                        case 2:
                            textbox_sum_mas[j - 1, i].Text = "w " + i + " " + (j - 1);
                            textbox_sum_mas[j - 1, i].Location = new Point(((i + 1) * _w) + ((i + 2) * delta), (((int)numericUpDown1.Value + 1) * _h) + (((int)numericUpDown1.Value + 2) * delta));
                            break;
                        case 3:
                            textbox_sum_mas[j - 1, i].Text = "h " + i + " " + (j - 1);
                            textbox_sum_mas[j - 1, i].Location = new Point(delta, ((i + 1) * _h) + ((i + 2) * delta));
                            break;
                        case 4:
                            textbox_sum_mas[j - 1, i].Text = "h " + i + " " + (j - 1);
                            textbox_sum_mas[j - 1, i].Location = new Point((((int)numericUpDown1.Value + 1) * _w) + (((int)numericUpDown1.Value + 2) * delta), ((i + 1) * _h) + ((i + 2) * delta));
                            break;
                    }
                    textbox_sum_mas[j - 1, i].Parent = panel1;
                    textbox_sum_mas[j - 1, i].MouseLeave += new EventHandler(TextBox_MouseLeave);
                    textbox_sum_mas[j - 1, i].TextChanged += new EventHandler(TextBox_TextChanged);

                    button_sum_mas[j - 1, i].Text = textbox_sum_mas[j - 1, i].Text;
                    button_sum_mas[j - 1, i].Location = textbox_sum_mas[j - 1, i].Location;
                    button_sum_mas[j - 1, i].FlatAppearance.BorderSize = 1;
                    button_sum_mas[j - 1, i].FlatAppearance.BorderColor = Color.FromArgb(255, 69, 72, 84);
                    button_sum_mas[j - 1, i].Parent = panel1;
                    sum_tooltip.SetToolTip(button_sum_mas[j - 1, i], "");
                    button_sum_mas[j - 1, i].MouseDown += new MouseEventHandler(Button_MouseDown);
                    button_sum_mas[j - 1, i].MouseEnter += new EventHandler(Button_MouseEnter);
                    button_sum_mas[j - 1, i].MouseLeave += new EventHandler(Button_MouseLeave);
                }

                //середина
                for (int j = 0; j < numericUpDown1.Value; j++)
                {
                    button_table_mas[i, j] = new Button()
                    {
                        ForeColor = Color.White,
                        Text = "t " + i + " " + j,
                        Size = new Size(_w, _h),
                        Location = new Point(((i + 1) * _w) + ((i + 2) * delta), ((j + 1) * _h) + ((j + 2) * delta)),
                        FlatStyle = FlatStyle.Flat,
                        Region = myRegion,
                        BackgroundImage = global::Rullo.Properties.Resources.locked3,
                        BackgroundImageLayout = ImageLayout.Stretch,
                        Tag = "cell",
                        TabStop = false,
                        TextAlign = ContentAlignment.MiddleCenter
                    };
                    textbox_table_mas[i, j] = new TextBox()
                    {
                        Visible = false,
                        Size = new Size(_w, _h),
                        Multiline = true,
                        MaxLength = 2,
                        Location = new Point(((i + 1) * _w) + ((i + 2) * delta), ((j + 1) * _h) + ((j + 2) * delta)),
                        BorderStyle = BorderStyle.FixedSingle,
                        TextAlign = HorizontalAlignment.Center
                    };
                    button_table_mas[i, j].Parent = panel1;
                    button_table_mas[i, j].FlatAppearance.BorderSize = 0;
                    button_table_mas[i, j].MouseDown += new MouseEventHandler(Button_MouseDown);
                    
                    textbox_table_mas[i, j].Text = button_table_mas[i, j].Text;
                    textbox_table_mas[i, j].Parent = panel1;
                    textbox_table_mas[i, j].MouseLeave += new EventHandler(TextBox_MouseLeave);
                    textbox_table_mas[i, j].TextChanged += new EventHandler(TextBox_TextChanged);
                }
            }

            //заполнение фактического массива
            field = new Field((int)numericUpDown1.Value, button_sum_mas, button_table_mas, textbox_sum_mas, textbox_table_mas);
        }
        private void CreateVoid_ButtonClick(object sender, EventArgs e)
        {
            if (field == null || (int)numericUpDown1.Value != field.Count)
            {
                panel1.Controls.Clear();
                textbox_table_mas = new TextBox[(int)numericUpDown1.Value, (int)numericUpDown1.Value];
                textbox_sum_mas = new TextBox[4, (int)numericUpDown1.Value];
                button_table_mas = new Button[(int)numericUpDown1.Value, (int)numericUpDown1.Value];
                button_sum_mas = new Button[4, (int)numericUpDown1.Value];
                Create();
                Redraw_state_from_table();
            }
            else
            {
                field.Clear();
                Redraw_state_from_table();
            }
        }
        private void Button_MouseDown(object sender, MouseEventArgs e)
        {
            if (checkBox1.Checked)
            {
                //edit mode
                if (e.Button == MouseButtons.Left)
                {
                    (sender as Button).Visible = false;
                    try
                    {
                        field.GetTextBox(sender as Button).Visible = true;
                    }
                    catch (Exception ee)
                    {
                        MessageBox.Show("это как раз тот случай, который никогда не должен сработать" + ee.Data);
                    }
                }
            }
            else
            {
                if ((sender as Button).Tag.ToString() == "cell" && e.Button == MouseButtons.Left)
                {
                    field.SetMarked((sender as Button), !field.GetMarked(sender as Button));
                    Redraw_state_from_table();
                }
                if ((sender as Button).Tag.ToString() == "cell" && e.Button == MouseButtons.Right)
                {
                    field.SetLocked((sender as Button), !field.GetLocked(sender as Button));
                    Redraw_state_from_table();
                }
                if ((sender as Button).Tag.ToString() == "sum" && e.Button == MouseButtons.Left)
                {
                    if (field.GetLocked(sender as Button))
                    {
                        //проверка на повторное нажатие
                        Line temp = field.GetLine(sender as Button);
                        bool flag1 = false;
                        foreach (Cell item in temp.Cells)
                        {
                            if (!item.Locked) flag1 = true;
                        }
                        if (!flag1)
                        {
                            //второй прогон - все делаем НЕ locked
                            foreach (Cell item in temp.Cells)
                            {
                                item.Locked = false;
                            }
                        }
                        else
                        {
                            //первый прогон - все делаем locked
                            foreach (Cell item in temp.Cells)
                            {
                                item.Locked = true;
                            }
                        }
                        Redraw_state_from_table();
                    }
                }
            }
        }
        private void Button_MouseEnter(object sender, EventArgs e)
        {
            if ((sender as Button).Tag.ToString() == "sum")
            {
                (sender as Button).BackColor = Color.Blue;
                sum_tooltip.SetToolTip((sender as Button), "Текущая сумма = " + field.CalcCurrentSum(sender as Button));
            }
            Status_update();
        }
        private void Button_MouseLeave(object sender, EventArgs e)
        {
            if ((sender as Button).Tag.ToString() == "sum")
            {
                Check_all_sum_eq_and_wrong();
                Status_update();
            }
        }
        private void TextBox_MouseLeave(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                (sender as TextBox).Visible = false;
                field.GetButton(sender as TextBox).Visible = true;
                Redraw_state_from_table();
            }
        }
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                field.FillValue(sender as TextBox, (sender as TextBox).Text);
                field.Was_solved = false;
                field.Clear(false, true, false);
                field.CanSolveAllInternal();
            }
        }
        public void Redraw_state_from_table()
        {
            foreach (Line iter in field.H_lines)
            {
                //ячейки
                foreach (Cell iter2 in iter.Cells)
                {
                    iter2.Btn.Text = iter2.V.ToString();
                    iter2.Txt.Text = iter2.V.ToString();

                    if (iter2.Locked)
                    {
                        iter2.Btn.BackgroundImage = global::Rullo.Properties.Resources.locked3;
                    }
                    else
                    {
                        iter2.Btn.BackgroundImage = global::Rullo.Properties.Resources.normal3;
                    }

                    if (iter2.Marked)
                    {
                        iter2.Btn.BackColor = Color.FromArgb(255, 213, 20, 76);
                    }
                    else
                    {
                        iter2.Btn.BackColor = Color.FromArgb(255, 23, 26, 41);
                    }

                    if (iter2.Internal_marked && !iter2.Marked && checkBox4.Checked || !iter2.Internal_marked && iter2.Marked && iter2.Locked && checkBox5.Checked)
                    {
                        iter2.Btn.ForeColor = Color.DarkRed;
                        iter2.Btn.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
                    }
                    else
                    {
                        iter2.Btn.ForeColor = Color.White;
                        iter2.Btn.Font = new Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                    }
                }
            }
            //для сумм
            foreach (Line iter in field.Lines)
            {
                iter.Bsum1.Text = iter.Require_sum.ToString();
                iter.Bsum2.Text = iter.Require_sum.ToString();
                iter.Tsum1.Text = iter.Require_sum.ToString();
                iter.Tsum2.Text = iter.Require_sum.ToString();
            }
            Check_all_sum_eq_and_wrong();
            Status_update();
        }
        public void Status_update()
        {
            string s = "";
            if (field.CheckWrong())
            {
                s += "wrong values on field`s cells, you should supplement it";
            }
            else if (!field.Was_solved)
            {
                s += "field cant be solved, pls edit";
            }
            else if (field.FullField(false))
            {
                s += "Yo! field fullfilled";
            }
            else if (checkBox3.Checked)
            {
                if (field.Check())
                {
                    s += "allright, field can be solved, and still no mistakes";
                }
                else
                {
                    s += "mistake";
                }
            }
            else
            {
                s += "field can be solved, not full filled noww";
            }
            label1.Text = s;
        }
        void Check_all_sum_eq_and_wrong()
        {
            foreach (Line iter in field.Lines)
            {
                iter.Bsum1.BackColor = Color.FromArgb(255, 23, 26, 41);
                iter.Bsum2.BackColor = Color.FromArgb(255, 23, 26, 41);
                iter.Bsum1.ForeColor = Color.White;
                iter.Bsum2.ForeColor = Color.White;

                int temp_sum = field.CalcCurrentSum(iter.Bsum1);

                if ((temp_sum < iter.Require_sum) && checkBox2.Checked)
                {
                    iter.Sum_locked = false;
                    iter.Bsum1.ForeColor = Color.Red;
                    iter.Bsum1.BackColor = Color.Purple;
                    iter.Bsum2.ForeColor = Color.Red;
                    iter.Bsum2.BackColor = Color.Purple;
                }
                else if (temp_sum == iter.Require_sum && checkBox6.Checked)
                {
                    iter.Sum_locked = true;
                    iter.Bsum1.ForeColor = Color.Gold;
                    iter.Bsum2.ForeColor = Color.Gold;
                }
                else if (temp_sum > iter.Require_sum || ((temp_sum < iter.Require_sum) && !checkBox2.Checked))
                {
                    iter.Sum_locked = false;
                }
            }
        }
        private void SolveButton_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += DateTime.Now.ToString() + "\n";
            if (field.Check())
            {
                if (field.FullField(false))
                {
                    richTextBox1.Text += "Поле заполнено, нефиг жать кнопки просто так\n";
                }
                else
                {
                    field.Solve();
                }
            }
            else
            {
                richTextBox1.Text += "Сначала стоит исправить текущие ошибки\n";
            }
            richTextBox1.Text += "------------------------------------------\n";
            richTextBox1.SelectionStart = richTextBox1.TextLength;
            richTextBox1.ScrollToCaret();
            Redraw_state_from_table();
        }
        private void Example1Button_Click(object sender, EventArgs e)
        {
            field.Clear();
            numericUpDown1.Value = 5;
            CreateVoid_ButtonClick(this, new EventArgs());

            int[,] mas = {
                { 3, 5, 1, 3, 7 },
                { 7, 1, 3, 2, 4 },
                { 9, 7, 4, 3, 6 },
                { 1, 1, 8, 8, 9 },
                { 9, 8, 4, 2, 8 }
            };
            field.FillField(5, mas, new int[] { 28, 13, 19, 8, 18 }, new int[] { 8, 14, 26, 17, 21 });
            field.CanSolveAllInternal();
            Redraw_state_from_table();
        }
        private void Example2Button_Click(object sender, EventArgs e)
        {
            field.Clear();
            numericUpDown1.Value = 8;
            CreateVoid_ButtonClick(this, new EventArgs());

            int[,] mas = {
                { 3, 8, 5, 9, 4, 4, 7, 2 },
                { 8, 1, 2, 1, 5, 4, 9, 9 },
                { 5, 8, 6, 6, 8, 3, 5, 4 },
                { 9, 1, 7, 4, 5, 4, 1, 1 },
                { 5, 2, 6, 3, 5, 3, 9, 3 },
                { 7, 2, 6, 2, 3, 1, 9, 9 },
                { 1, 5, 7, 7, 4, 6, 3, 3 },
                { 5, 1, 5, 8, 6, 3, 6, 2 }
            };
            field.FillField(8, mas, new int[] { 24, 19, 39, 27, 30, 21, 34, 31 }, new int[] { 33, 30, 39, 18, 24, 37, 24, 20 });
            field.CanSolveAllInternal();
            Redraw_state_from_table();
        }
        private void Log_updating(string param)
        {
            richTextBox1.Text += param;
        }
        void Generate(int a, int b)
        {
            if (field != null) field.Clear();
            int[,] mas;
            bool[,] matrix;
            int[] vsum;
            int[] hsum;
            bool flag_zero = false;
            bool flag_retry = false;
            do
            {
                do
                {
                    //рандомные ячейки
                    Random r = new Random();
                    mas = new int[(int)numericUpDown1.Value, (int)numericUpDown1.Value];
                    for (int i = 0; i < numericUpDown1.Value; i++)
                    {
                        for (int j = 0; j < numericUpDown1.Value; j++)
                        {
                            mas[i, j] = Convert.ToInt32(Math.Round(r.NextDouble() * (b - a) + a));
                        }
                    }

                    //заполнение матрицы заполнения
                    matrix = new bool[(int)numericUpDown1.Value, (int)numericUpDown1.Value];
                    for (int i = 0; i < numericUpDown1.Value; i++)
                    {
                        for (int j = 0; j < numericUpDown1.Value; j++)
                        {
                            matrix[i, j] = r.NextDouble() > 0.4 ? true : false;
                        }
                    }

                    //подсчет сумм по матрице
                    vsum = new int[(int)numericUpDown1.Value];
                    hsum = new int[(int)numericUpDown1.Value];
                    for (int i = 0; i < numericUpDown1.Value; i++)
                    {
                        for (int j = 0; j < numericUpDown1.Value; j++)
                        {
                            if (matrix[i, j])
                            {
                                vsum[j] += mas[i, j];
                                hsum[i] += mas[i, j];
                            }
                        }
                    }

                    //просчет флага - для того, чтобы в суммах не было нуля
                    flag_zero = false;
                    foreach (int i in hsum)
                    {
                        if (i == 0)
                        {
                            flag_zero = true;
                            break;
                        }
                    }
                    if (!flag_zero)
                    {
                        foreach (int i in vsum)
                        {
                            if (i == 0)
                            {
                                flag_zero = true;
                                break;
                            }
                        }
                    }
                } while (flag_zero);

                CreateVoid_ButtonClick(this, new EventArgs());
                field.FillField((int)numericUpDown1.Value, mas, vsum, hsum);
                Redraw_state_from_table();

                flag_retry = false;
                if (!field.CanSolveAllInternal())
                {
                    string s = "Сгенеренная таблица не может быть решена обычными методами, так что это либо таблица, имеющая два решения (что ОЧЕНЬ вероятно, особенно в режиме 2-4), либо методы решения не справились, о чем стоит меня уведомить, и ОБЯЗАТЕЛЬНО заскринить эту таблицу.\nОставить эту таблицу?";
                    DialogResult result = MessageBox.Show(s, "Ахтунг!", MessageBoxButtons.YesNo);
                    if (result == DialogResult.No) flag_retry = true;
                }
            } while (flag_retry);
        }
        private void GenerateButton_Click(object sender, EventArgs e)
        {
            if ((groupBox1.Controls.Find("radioButton1", false).FirstOrDefault() as RadioButton).Checked)
            {
                Generate(1, 9);
            }
            if ((groupBox1.Controls.Find("radioButton2", false).FirstOrDefault() as RadioButton).Checked)
            {
                Generate(1, 19);
            }
            if ((groupBox1.Controls.Find("radioButton3", false).FirstOrDefault() as RadioButton).Checked)
            {
                Generate(2, 4);
            }
        }
        private void ClearButton_Click(object sender, EventArgs e)
        {
            field.Clear(false, false);
            Redraw_state_from_table();
        }
        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            Check_all_sum_eq_and_wrong();
        }
        private void CheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            Status_update();
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            string ss = "Итак, что тут есть и зачем:\n";
            ss += "Это реализация игры Rullo с возможностью автоматического решения\n";
            ss += "Правила:\n";
            ss += "Круги - ячейки, квадраты - суммы вертикальных/горизонтальных линий\n";
            ss += "Необходимо оставить круги заполненными так, чтобы реальные и прописанные в квадратах суммы совпадали везде\n";
            ss += "Управление:\nЛКМ на круге: выбор (отметка) ячейки, повторный клик инвертирует состояние клетки\n";
            ss += "ЛКМ на квадрате: фиксирование линии (когда сумма совпадает), отмена этого действия по повторному нажатию\n";
            ss += "ПКМ: блокировка ячейки";
            MessageBox.Show(ss, "WTF");
        }
        private void CheckBox4_CheckedChanged(object sender, EventArgs e)
        {
            Redraw_state_from_table();
        }
    }
}
