using ActUtlType64Lib;
using NModbus;
using NModbus.Data;
using NModbus.Device;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Xml.Linq;

namespace PlcModbus
{
    public partial class Form1 : Form
    {


        bool isConnected = false;
        static ActUtlType64 plc = new ActUtlType64();
        static PlcData _data = new PlcData(plc);
        static ModbusServer modbusServer = new ModbusServer(_data);



        public Form1()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {

            int result = plc.Open();
            if (result == 0)
            {
                MessageBox.Show("PLC 연결 성공", "", MessageBoxButtons.OK);
                isConnected = true;
                button1.BackColor = Color.AliceBlue;
                

                modbusServer.StartModbusServer();
            }
            else
            {
                MessageBox.Show($"PLC 연결 실패\n 에러코드 {result}", "", MessageBoxButtons.OK);
            }

        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = DateTime.Now.ToString();

            if (isConnected)
            {

                _data.ReadData();


                modbusServer.ModbusUpdate();

                //_data.WriteData();

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int result = plc.Close();
            if (result == 0)
            {
                MessageBox.Show("PLC 연결 해제", "", MessageBoxButtons.OK);
                isConnected = false;
                button1.BackColor = Color.White;


                modbusServer.EndModbusServer();
            }
            else
            {
                MessageBox.Show($"PLC 연결 해제 실패\n 에러코드 {result}", "", MessageBoxButtons.OK);
            }
        }
    }
}
