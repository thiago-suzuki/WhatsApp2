using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Chat01
{
    public partial class Form1 : Form
    {
        Thread thread;
        Socket socketreceber = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP);
        EndPoint endereco2 = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4060);
        byte[] data = new byte[1024];
        int qtdbytes;
        public Form1()
        {
            InitializeComponent();
            socketreceber.Bind(endereco2);
        }
        private void ProcessoEnviaMensagem()
        {
            while (true)
            {
                qtdbytes = socketreceber.ReceiveFrom(data, ref endereco2);
                listBox1.Invoke((Action)delegate
                {
                    listBox1.Items.Add($"Amigo: {Encoding.UTF8.GetString(data, 0, qtdbytes)}");
                });
                Thread.Sleep(1);
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = DateTime.Now.ToString("dd:MM:yyyy HH:mm:ss");
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            thread = new Thread(() => ProcessoEnviaMensagem());
            thread.Start();
        }
        private void btnEnviar_Click(object sender, EventArgs e)
        {
            Socket socketenviar = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP);
            IPEndPoint endereco = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3060);
            socketenviar.SendTo(Encoding.UTF8.GetBytes(textBox1.Text), endereco);
            listBox1.Items.Add($"Você: {textBox1.Text}");
            socketenviar.Close();
            textBox1.Clear();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            var resultado = MessageBox.Show("Você Deseja Sair da Aplicação?", "Sair da Aplicação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resultado == DialogResult.Yes)
            {
                if (e.Cancel == false)
                    e.Cancel = true;
                thread.Abort();
                socketreceber.Close();
                e.Cancel = false;
            }
            else if (resultado == DialogResult.No)
            {
                if (e.Cancel == false)
                    e.Cancel = true;
            }
        }
    }
}
