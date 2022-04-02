using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace _3DXO
{
//TODO: Recieve after Send automaticaly
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public Color color1 = Color.Red;
        public Color color2 = Color.Blue;

        public string symbol;
        public Button[,,] btns = new Button[3, 3, 3];
        public string[,,] chars = new string[3, 3, 3];

        static string remoteAddress; // хост для отправки данных
        static int remotePort; // порт для отправки данных
        static int localPort; // локальный порт для прослушивания входящих подключений
        private void Form1_Load(object sender, EventArgs e)
        {
            bindButtons();
            clear();
        }
        private void button_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Text == " ")
            {
                btn.Text = symbol;
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                        for (int k = 0; k < 3; k++)
                            chars[i, j, k] = btns[i, j, k].Text;
            }
            else
                MessageBox.Show("Занято");
            SendMessage();
        }
        private void SendMessage()
        {
            UdpClient sender = new UdpClient(); // создаем UdpClient для отправки сообщений
            try
            {
                string message = btnsToString(); // сообщение для отправки
                byte[] data = Encoding.Unicode.GetBytes(message);
                sender.Send(data, data.Length, remoteAddress, remotePort); // отправка
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sender.Close();
            }
        }

        private void ReceiveMessage()
        {
            UdpClient receiver = new UdpClient(localPort); // UdpClient для получения данных
            IPEndPoint remoteIp = null; // адрес входящего подключения
            try
            {
                byte[] data = receiver.Receive(ref remoteIp); // получаем данные
                string message = Encoding.Unicode.GetString(data);
                stringToBtns(message);
                MessageBox.Show("Собеседник: {0}", message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                receiver.Close();
                checkWinner();
            }
        }

        private string btnsToString()
        {
            string str = "";
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    for (int k = 0; k < 3; k++)
                        str += btns[i, j, k].Text + ", ";
            return str;
        }
        private void stringToBtns(string str)
        {
            int index = 0;
            string[] strs = str.Split(", ");
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    for (int k = 0; k < 3; k++)
                    {
                        btns[i, j, k].Text = strs[index];
                        chars[i, j, k] = strs[index];
                        index++;
                    }
        }

        private void bindButtons()
        {
            btns[0, 0, 0] = button1;
            btns[0, 0, 1] = button2;
            btns[0, 0, 2] = button3;
            btns[0, 1, 0] = button4;
            btns[0, 1, 1] = button5;
            btns[0, 1, 2] = button6;
            btns[0, 2, 0] = button7;
            btns[0, 2, 1] = button8;
            btns[0, 2, 2] = button9;

            btns[1, 0, 0] = button10;
            btns[1, 0, 1] = button11;
            btns[1, 0, 2] = button12;
            btns[1, 1, 0] = button13;
            btns[1, 1, 1] = button14;
            btns[1, 1, 2] = button15;
            btns[1, 2, 0] = button16;
            btns[1, 2, 1] = button17;
            btns[1, 2, 2] = button18;

            btns[2, 0, 0] = button19;
            btns[2, 0, 1] = button20;
            btns[2, 0, 2] = button21;
            btns[2, 1, 0] = button22;
            btns[2, 1, 1] = button23;
            btns[2, 1, 2] = button24;
            btns[2, 2, 0] = button25;
            btns[2, 2, 1] = button26;
            btns[2, 2, 2] = button27;
        }
        private void checkWinner()
        {
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        isEqueal(btns[0, i, j], btns[1, i, j], btns[2, i, j]);
                        isEqueal(btns[0, j, i], btns[1, j, i], btns[2, j, i]);
                        isEqueal(btns[i, 0, j], btns[i, 1, j], btns[i, 2, j]);
                        isEqueal(btns[j, 0, i], btns[j, 1, i], btns[j, 2, i]);
                        isEqueal(btns[i, j, 0], btns[i, j, 1], btns[i, j, 2]);
                        isEqueal(btns[j, i, 0], btns[j, i, 1], btns[j, i, 2]);
                    }
                }
                for (int i = 0; i < 3; i++)
                {
                    isEqueal(btns[i, 0, 0], btns[i, 1, 1], btns[i, 2, 2]);
                    isEqueal(btns[i, 0, 2], btns[i, 1, 1], btns[i, 2, 0]);
                    isEqueal(btns[0, 0, i], btns[1, 1, i], btns[2, 2, i]);
                    isEqueal(btns[0, 2, i], btns[1, 1, i], btns[2, 0, i]);
                    isEqueal(btns[0, i, 0], btns[1, i, 1], btns[2, i, 2]);
                    isEqueal(btns[0, i, 2], btns[1, i, 1], btns[2, i, 0]);
                }
                isEqueal(btns[0, 0, 0], btns[1, 1, 1], btns[2, 2, 2]);
                isEqueal(btns[0, 0, 2], btns[1, 1, 1], btns[2, 0, 2]);
                isEqueal(btns[0, 2, 0], btns[1, 1, 1], btns[2, 0, 2]);
                isEqueal(btns[0, 2, 2], btns[1, 1, 1], btns[2, 0, 0]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void clear()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    for (int k = 0; k < 3; k++)
                    {
                        btns[i, j, k].Text = " ";
                        btns[i, j, k].BackColor = SystemColors.Control;
                        chars[i, j, k] = " ";
                    }
        }
        private void isEqueal(Button b1, Button b2, Button b3)
        {
            if (b1.Text == b2.Text && b2.Text == b3.Text && b2.Text != " ")
            {
                if (b2.Text == symbol)
                    b1.BackColor = b2.BackColor = b3.BackColor = color1;
                else
                    b1.BackColor = b2.BackColor = b3.BackColor = color2;
                throw new Exception("Игрок " + b2.Text + " победил!");
            }
        }
        private void restartBtn_Click(object sender, EventArgs e)
        {
            clear();
        }

        private void button28_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        private void button29_Click(object sender, EventArgs e)
        {
            ReceiveMessage();
        }

        private void button30_Click(object sender, EventArgs e)
        {
            symbol = symbolTb.Text;
            remoteAddress = serverTb.Text;
        }

        private void button31_Click(object sender, EventArgs e)
        {
            remotePort = 50504;
            localPort = 50505;
        }

        private void button32_Click(object sender, EventArgs e)
        {
            remotePort = 50505;
            localPort = 50504;
        }
    }
}
