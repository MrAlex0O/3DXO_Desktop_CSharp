using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace _3DXO_Console
{
    class Program
    {
        static string symbol = "O";
        public static string[,,] chars = new string[3, 3, 3];

        static string remoteAddress = "127.0.0.1"; // хост для отправки данных
        static int remotePort; // порт для отправки данных
        static int localPort; // локальный порт для прослушивания входящих подключений
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Крестики-нолики 3D");
                Console.WriteLine("Выберите символ: ");
                symbol = Console.ReadLine();
                Console.WriteLine("Адрес оппронента: ");
                remoteAddress = Console.ReadLine();
                clear();

                Console.WriteLine("Первый ход? (+/-)");
                string key = Console.ReadLine();
                if (key == "-")
                {
                    remotePort = 50505;
                    localPort = 50504;    
                    ReceiveMessage();
                }
                else
                {
                    remotePort = 50504;
                    localPort = 50505;
                }
                while (true)
                {
                    showField();
                    Console.WriteLine("Выберите точку{лист, строка, столбец (1\\n2\\n3)}: ");
                    int i = Convert.ToInt32(Console.ReadLine()) - 1;
                    int j = Convert.ToInt32(Console.ReadLine()) - 1;
                    int k = Convert.ToInt32(Console.ReadLine()) - 1;
                    showField();
                    takePoint(i, j, k);
                    SendMessage();
                    ReceiveMessage();
                }
            }
            catch(Exception ex)
            { Console.WriteLine(ex.Message); }
        }
        private static void showField()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        Console.Write(chars[i, j, k] + ", ");

                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
        }
        private static void takePoint(int i, int j, int k)
        {
            if (chars[i, j, k] == " ")
                chars[i, j, k] = symbol;
            else
                Console.WriteLine("Занято");
        }
        private static void SendMessage()
        {
            UdpClient sender = new UdpClient(); // создаем UdpClient для отправки сообщений
            try
            {
                string message = charsToString(); // сообщение для отправки
                byte[] data = Encoding.Unicode.GetBytes(message);
                sender.Send(data, data.Length, remoteAddress, remotePort); // отправка
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                sender.Close();
            }
        }

        private static void ReceiveMessage()
        {
            UdpClient receiver = new UdpClient(localPort); // UdpClient для получения данных
            IPEndPoint remoteIp = null; // адрес входящего подключения
            try
            {
                byte[] data = receiver.Receive(ref remoteIp); // получаем данные
                string message = Encoding.Unicode.GetString(data);
                stringToBtns(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                receiver.Close();
                checkWinner();
            }
        }

        private static string charsToString()
        {
            string str = "";
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    for (int k = 0; k < 3; k++)
                        str += chars[i, j, k] + ", ";
            return str;
        }
        private static void stringToBtns(string str)
        {
            int index = 0;
            string[] strs = str.Split(", ");
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    for (int k = 0; k < 3; k++)
                    {
                        chars[i, j, k] = strs[index];
                        index++;
                    }
        }

        private static void checkWinner()
        {
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        isEqueal(chars[0, i, j], chars[1, i, j], chars[2, i, j]);
                        isEqueal(chars[0, j, i], chars[1, j, i], chars[2, j, i]);
                        isEqueal(chars[i, 0, j], chars[i, 1, j], chars[i, 2, j]);
                        isEqueal(chars[j, 0, i], chars[j, 1, i], chars[j, 2, i]);
                        isEqueal(chars[i, j, 0], chars[i, j, 1], chars[i, j, 2]);
                        isEqueal(chars[j, i, 0], chars[j, i, 1], chars[j, i, 2]);
                    }
                }
                for (int i = 0; i < 3; i++)
                {
                    isEqueal(chars[i, 0, 0], chars[i, 1, 1], chars[i, 2, 2]);
                    isEqueal(chars[i, 0, 2], chars[i, 1, 1], chars[i, 2, 0]);
                    isEqueal(chars[0, 0, i], chars[1, 1, i], chars[2, 2, i]);
                    isEqueal(chars[0, 2, i], chars[1, 1, i], chars[2, 0, i]);
                    isEqueal(chars[0, i, 0], chars[1, i, 1], chars[2, i, 2]);
                    isEqueal(chars[0, i, 2], chars[1, i, 1], chars[2, i, 0]);
                }
                isEqueal(chars[0, 0, 0], chars[1, 1, 1], chars[2, 2, 2]);
                isEqueal(chars[0, 0, 2], chars[1, 1, 1], chars[2, 0, 2]);
                isEqueal(chars[0, 2, 0], chars[1, 1, 1], chars[2, 0, 2]);
                isEqueal(chars[0, 2, 2], chars[1, 1, 1], chars[2, 0, 0]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private static void clear()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    for (int k = 0; k < 3; k++)
                    {
                        chars[i, j, k] = " ";
                    }
        }
        private static void isEqueal(string b1, string b2, string b3)
        {
            if (b1 == b2 && b2 == b3 && b2 != " ")
            {
                throw new Exception("Игрок " + b2 + " победил!");
            }
        }
    }
}
