using NModbus;
using System.Diagnostics;
using System.Net.Sockets;
using System.Windows.Forms;

namespace ModbusMaster
{
    public partial class Form1 : Form
    {
        private static TcpClient client;
        private static IModbusMaster modbusMaster;
        bool isConnected = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ModbusConnect("127.0.0.1", 502);//����ȣ��Ʈ ����

        }

        private void ModbusConnect(string _ipAddress, int _port)
        {
            try
            {
                client = new TcpClient(_ipAddress, _port);
                isConnected = client.Connected;
                if (isConnected)
                {
                    MessageBox.Show("������ �����߽��ϴ�.", "Success", MessageBoxButtons.OK);
                    var factory = new ModbusFactory();
                    modbusMaster = factory.CreateMaster(client);
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

        private void ReadHoldingRegisters()
        {
            if (modbusMaster != null)
            {
                ushort startAddress = 0;
                ushort numRegisters = 125;//�ִ� 125���� �ְ� �ޱ�
                ushort[] registers = modbusMaster.ReadHoldingRegisters(1, startAddress, numRegisters);

                //Debug.Log($"Coils: {string.Join(", ", registers)}");
            }
            else
            {
                Console.WriteLine("ModbusMaster is not initialized.");//����ó�� �����ؾ� ��
            }
        }

        private void ReadInputs()
        {
            if (modbusMaster != null)
            {
                ushort startAddress = 0;
                ushort numCoils = 1024;//���� ����
                bool[] coils = modbusMaster.ReadInputs(1, startAddress, numCoils);

                for (int i = 0; i < 5; i++)
                {
                    Debug.Write($"M{i} : {coils[i]} ");
                }
                Debug.WriteLine("");
            }
            else
            {
                Debug.WriteLine("ModbusMaster is not initialized.");//����ó�� �����ؾ� ��
            }
        }

        private void WriteCoils(string _address, bool[] _writeValue)
        {
            if (modbusMaster != null)
            {
                if (ushort.TryParse(_address, out ushort address))
                {
                    if (_writeValue[address-80])
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
            else
            {
                Debug.WriteLine("ModbusMaster is not initialized.");//����ó�� �����ؾ� ��
            }
        }


        private void timer1_Tick(object sender, EventArgs e)
        {

            if (isConnected)
            {
                ReadHoldingRegisters();
                ReadInputs();
            }
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            string address = WriteAddress.Text;
            
            if (modbusMaster != null)
            {
                bool[] writeValue = modbusMaster.ReadCoils(1, 80, 5);
                for (int i = 0; i < 5; i++)
                {
                    Debug.Write($"M{i+80} : {writeValue[i]} ");
                }
                Debug.WriteLine("");
                WriteCoils(address, writeValue);
            }
            else
            {
                MessageBox.Show("ModbusMaster is not initialized.","Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);//����ó�� �����ؾ� ��
            }
        }
    }
}
