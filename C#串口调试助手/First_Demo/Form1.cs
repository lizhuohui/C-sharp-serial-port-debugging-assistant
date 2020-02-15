using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace First_Demo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
        }
        /// <summary>
        /// 窗口初始化函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox2.Text = "9600";
            //清空接收区、发送、清空、发送框、接收框不可用
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            Search_Port(serialPort1, comboBox1);
            comboBox1.Text = "COM1";
            //手动添加的事件处理函数
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
        }
        /// <summary>
        /// 打开串口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            string buffer = comboBox1.Text;//获取控件信息
            if (buffer.Equals(""))//如果buffer数值为空
            {
                MessageBox.Show("请选择串口", "提示");
                return;
            }
            if (serialPort1.IsOpen)//如果串口已经打开
            {
                try
                {
                    button2.Text = "打开串口";
                    serialPort1.Close();
                    //清空接收区、发送、清空、发送框、接收框不可用
                    button3.Enabled = false;
                    button4.Enabled = false;
                    textBox1.Enabled = false;
                    //扫描可用
                    button1.Enabled = true;
                    //端口号可用
                    comboBox1.Enabled = true;
                    //波特率可用
                    comboBox2.Enabled = true;
                }
                catch { }
                return;
            }
            try
            {
                serialPort1.PortName = comboBox1.Text;//获取端口号
                serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text, 10);//获取波特率
                serialPort1.Open();
                button2.Text = "关闭串口";
                //扫描按键不可用
                button1.Enabled = false;
                //端口号不可用
                comboBox1.Enabled = false;
                //波特率不可用
                comboBox2.Enabled = false;
                //清空接收区、发送、清空、发送框、接收框可用
                button3.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = true;
                textBox1.Enabled = true;
                textBox2.Enabled = true;
            }
            catch
            {
                serialPort1.Close();
                button2.Text = "打开串口";
                //清空接收区、发送、清空、发送框、接收框不可用
                button3.Enabled = false;
                button4.Enabled = false;
                button5.Enabled = false;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                //扫描可用
                button1.Enabled = true;
                //端口号可用
                comboBox1.Enabled = true;
                //波特率可用
                comboBox2.Enabled = true;
                MessageBox.Show("串口出现未知错误", "错误");
            }
        }
        /// <summary>
        /// 串口数据接收
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (radioButton1.Checked)//如果接收模式为字符模式
                {
                    int ilen = serialPort1.BytesToRead;
                    byte[] bytes = new byte[ilen];
                    serialPort1.Read(bytes, 0, ilen);
                    string xx = System.Text.Encoding.Default.GetString(bytes);
                    textBox2.AppendText(xx);//添加内容
                }
                else
                {
                    //如果接收模式为数值接收
                    byte data;
                    data = (byte)serialPort1.ReadByte();//此处需要强制类型转换，将(int)类型数据转换为(byte类型数据，不必考虑是否会丢失数据
                    string str = Convert.ToString(data, 16).ToUpper();//转换为大写十六进制字符串
                    textBox2.AppendText("0x" + (str.Length == 1 ? "0" + str : str) + " ");//空位补“0”   
                }
            }
            catch
            {
                MessageBox.Show("接收数据错误", "警告");
            }

        }
        /// <summary>
        /// 寻找可用串口
        /// </summary>
        private void Search_Port(SerialPort Myport, ComboBox MyBox)
        {
            comboBox1.Items.Clear();
            string Buffer;
            for (int i = 1; i < 20; i++)
            {
                try
                {
                    Buffer = "COM" + i.ToString();
                    serialPort1.PortName = Buffer;
                    serialPort1.Open();
                    comboBox1.Items.Add(Buffer);
                    serialPort1.Close();
                }
                catch { }
            }
        }
        /// <summary>
        /// 扫描串口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            Search_Port(serialPort1, comboBox1);
        }
        /// <summary>
        /// 发送数据按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            byte[] Data = new byte[1];
            if (textBox1.Text != "")
            {
                if (radioButton3.Checked)//如果发送模式是字符模式
                {
                    try
                    {
                        Encoding gb = System.Text.Encoding.GetEncoding("gb2312");
                        byte[] bytes = gb.GetBytes(textBox1.Text);
                        serialPort1.Write(bytes, 0, bytes.Length);                     
                    }
                    catch
                    {
                        MessageBox.Show("串口数据写入错误", "错误");//出错提示
                    }
                }
                else   //发送的是十六进制数据
                {
                    for (int i = 0; i < (textBox1.Text.Length - 1) / 2; i++)//对两个字符进行处理，剩下的一个单独处理
                    {
                        //Substring(a,b); 
                        //a:开始字符
                        //b:需要截取的字符串的长度
                        Data[0] = Convert.ToByte(textBox1.Text.Substring(i * 2, 2), 16);
                        serialPort1.Write(Data, 0, 1);//循环发送（如果输入字符为0A0BB,则只发送0A,0B）
                    }
                    if (textBox1.Text.Length % 2 != 0)//剩下一位单独处理
                    {
                        Data[0] = Convert.ToByte(textBox1.Text.Substring(textBox1.Text.Length - 1, 1), 16);//单独发送B（0B）
                        serialPort1.Write(Data, 0, 1);//发送
                    }
                }
            }
            else
            {
                MessageBox.Show("请不要发送空白数据", "提醒");
            }
        }
        /// <summary>
        /// 清空接收区
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "")//已经没有数据
            {
                MessageBox.Show("请不要重复清空", "提示");
            }
            else
            {
                textBox2.Text = "";
            }
        }
        /// <summary>
        /// 清空发送区
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")//已经没有数据
            {
                MessageBox.Show("请不要重复清空", "提示");
            }
            else
            {
                textBox1.Text = "";
            }
        }
    }
}
