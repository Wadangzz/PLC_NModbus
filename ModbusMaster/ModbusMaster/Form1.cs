using NModbus;
using NModbus.Extensions.Enron;
using System.Diagnostics;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Threading;
using NModbus.Device;

namespace ModbusMaster
{
    public partial class Form1 : Form
    {

        private static TcpClient? client;
        private static IModbusMaster? modbusMaster;
        bool isConnected = false;

        private CancellationTokenSource? tokenSource;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ModbusConnect(ipAddress.Text, 502);//����ȣ��Ʈ ����

        }

        private async void ModbusConnect(string _ipAddress, int _port)
        {
            try
            {
                client = new TcpClient();
                await client.ConnectAsync(_ipAddress, _port);
                isConnected = client.Connected;
                if (isConnected)
                {
                    MessageBox.Show("������ �����߽��ϴ�.", "Success", MessageBoxButtons.OK);
                    var factory = new ModbusFactory();
                    modbusMaster = factory.CreateMaster(client);

                    tokenSource = new CancellationTokenSource();
                    await StartPolling(tokenSource.Token);
                }
                else
                {
                    MessageBox.Show("���� ���� ����.", "Fail", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("���� ���� ����.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine(e);
            }
        }

        private void ModbusDisconnect()
        {
            if (modbusMaster != null)
            {
                modbusMaster.Dispose();
                modbusMaster = null;
                client?.Close();
                isConnected = false;
                MessageBox.Show("���� ������ �����߽��ϴ�.", "Success", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show("���� ������ �Ǿ����� �ʽ��ϴ�.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ReadInputRegisters()
        {
            if (modbusMaster != null)
            {
                ushort startAddress = 0;
                ushort numRegisters = 125;//�ִ� 125���� �ְ� �ޱ�
                ushort[] registers = modbusMaster.ReadInputRegisters(1, startAddress, numRegisters);

                //Debug.Log($"Coils: {string.Join(", ", registers)}");
            }
        }

        private void ReadInputs()
        {
            if (modbusMaster != null)
            {
                bool[] coils = new bool[1024];
                ushort startAddress = 0;
                ushort numCoils = 1024;//���� ����
                coils = modbusMaster.ReadInputs(1, startAddress, numCoils);

                //for (int i = 0; i < 5; i++)
                //{
                //    Debug.Write($"M{i} : {coils[i]} ");
                //}
                //Debug.WriteLine("");
            }
        }

        private void WriteCoils(string _address, bool[] _writeValue)
        {
            if (modbusMaster != null)
            {
                if (ushort.TryParse(_address, out ushort address))
                {
                    if (_writeValue[address])
                    {
                        modbusMaster.WriteSingleCoil(1, address, false);
                    }
                    else
                    {
                        modbusMaster.WriteSingleCoil(1, address, true);
                    }

                }
                else
                {
                    MessageBox.Show("�Է� �ּҸ� Ȯ���ϼ���.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void WriteRegisters(string _address, string _value)
        {
            if (modbusMaster != null)
            {
                if ((ushort.TryParse(_address, out ushort address)) & (ushort.TryParse(_value, out ushort value)))
                {
                    modbusMaster.WriteSingleRegister(1, address, value);
                }
                else
                {
                    MessageBox.Show("�Է� �ּҿ� �����͸� Ȯ���ϼ���.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void btnWrite_Click(object sender, EventArgs e)
        {
            string address = WriteAddress.Text;
            if (modbusMaster != null)
            {
                ushort startAddress = 0;
                ushort numCoils = 1024;//���� ����
                bool[] writeValue = modbusMaster.ReadCoils(1, startAddress, numCoils);

                WriteCoils(address, writeValue);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ModbusDisconnect();
        }

        private void btnWriteDevice_Click(object sender, EventArgs e)
        {
            WriteRegisters(whiteDevice.Text, data.Text);
        }

        //private void timer1_Tick(object sender, EventArgs e)
        //{
        //    if (isConnected)
        //    {
        //        ReadInputRegisters();
        //        ReadInputs();
        //    }
        //}
        private async Task StartPolling(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (isConnected)
                {
                    await Task.Run(() => ReadInputRegisters());
                    await Task.Run(() => ReadInputs());
                }
                await Task.Delay(1, token); // 10ms ���͹�
            }
        }

        private void btnRead_Click(object sender, EventArgs e)
        {

        }
    }
}
