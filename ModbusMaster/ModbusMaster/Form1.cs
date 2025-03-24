using NModbus;
using NModbus.Extensions.Enron;
using System.Diagnostics;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Threading;
using NModbus.Device;
using System.Text;
using System.Net;

namespace ModbusMaster
{
    public partial class Form1 : Form
    {

        private TcpClient? client;
        private IModbusMaster? modbusMaster;
        ushort[] registers = new ushort[123];
        bool[] coils = new bool[1024];
        bool isConnected = false;

        private CancellationTokenSource? tokenSource;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ModbusConnect(ipAddress.Text, 502);//로컬호스트 접속
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
                    MessageBox.Show("서버에 접속했습니다.", "Success", MessageBoxButtons.OK);
                    ModbusFactory factory = new ModbusFactory();
                    modbusMaster = factory.CreateMaster(client);

                    tokenSource = new CancellationTokenSource();
                    await StartPolling(tokenSource.Token);
                }
                else
                {
                    MessageBox.Show("서버 접속 실패.", "Fail", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("서버 접속 에러.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("서버 접속을 종료했습니다.", "Success", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show("서버 접속이 되어있지 않습니다.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public async void SendCommand(string dataType)
        {
            try
            {
                using TcpClient writeClient = new TcpClient();
                await writeClient.ConnectAsync(ipAddress.Text, 6001);

                using NetworkStream stream = writeClient.GetStream();
                string message = dataType; // 문자열 "1" 전송
                byte[] data = Encoding.ASCII.GetBytes(message);
                await stream.WriteAsync(data, 0, data.Length);
                Debug.WriteLine("String dataType sent to server.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void ReadInputRegisters()
        {
            try
            {
                if (modbusMaster != null)
                {
                    ushort startAddress = 0;
                    ushort numRegisters = 123;//최대 123워드 주고 받기
                    registers = modbusMaster.ReadInputRegisters(1, startAddress, numRegisters);

                    //Debug.WriteLine($"Registers: {string.Join(", ", registers)}");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        private void ReadInputs()
        {
            try
            {
                if (modbusMaster != null)
                {
                    ushort startAddress = 0;
                    ushort numCoils = 1024;//코일 갯수
                    coils = modbusMaster.ReadInputs(1, startAddress, numCoils);
                    //Debug.WriteLine($"Coils: {string.Join(", ", coils)}");

                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
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
                        //modbusMaster.WriteSingleCoil(1, address, false);
                        coils[address] = false;
                    }
                    else
                    {
                        //modbusMaster.WriteSingleCoil(1, address, true);
                        coils[address] = true;
                    }
                    modbusMaster.WriteMultipleCoils(1, 0, coils);
                    bool[] coil = modbusMaster.ReadCoils(1, 0, 123);
                    Debug.WriteLine($"Coils: {string.Join(", ", coil)}");
                }
                else
                {
                    MessageBox.Show("입력 주소를 확인하세요.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void WriteRegisters(string _address, string _value)
        {
            try
            {
                if (modbusMaster != null)
                {
                    if ((ushort.TryParse(_address, out ushort address)) & (ushort.TryParse(_value, out ushort value)))
                    {
                        registers[address] = value;
                        modbusMaster.WriteMultipleRegisters(1, 0, registers);
                        //modbusMaster.WriteSingleRegister(1, address, value);
                        ushort[] register = modbusMaster.ReadHoldingRegisters(1, 0, 123);
                        Debug.WriteLine($"Registers: {string.Join(", ", register)}");
                    }
                    else
                    {
                        MessageBox.Show("입력 주소와 데이터를 확인하세요.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            string address = WriteAddress.Text;
            int idx = int.Parse(address);
            if (modbusMaster != null)
            {
                ushort startAddress = 0;
                ushort numCoils = 1024;//코일 갯수
                bool[] writeValue = modbusMaster.ReadCoils(1, startAddress, numCoils);

                WriteCoils(address, writeValue);
                SendCommand("M");
                lbx_Log.Items.Add($"{DateTime.Now.ToString()}   Write   Address : M{idx}      Data : {coils[idx]}");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ModbusDisconnect();
        }

        private void btnWriteDevice_Click(object sender, EventArgs e)
        {
            string address = whiteDevice.Text;
            string value = data.Text;
            int idx = int.Parse(address);
            if (modbusMaster != null)
            {
                WriteRegisters(address, value);
                SendCommand("D");
                lbx_Log.Items.Add($"{DateTime.Now.ToString()}   Write   Address : D{idx}      Data : {registers[idx]}");
            }
        }

        private async Task StartPolling(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (isConnected)
                {
                    await Task.Run(() => ReadInputRegisters());
                    await Task.Run(() => ReadInputs());
                }
                await Task.Delay(1, token); // 1ms 인터벌
            }
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            int address = int.Parse(readAddress.Text);
            lbx_Log.Items.Add($"{DateTime.Now.ToString()}   Read   Address : M{address}      Data : {coils[address]}");
        }

        private void btnReadDevice_Click(object sender, EventArgs e)
        {
            int address = int.Parse(readDevice.Text);
            lbx_Log.Items.Add($"{DateTime.Now.ToString()}   Read   Address : D{address}      Data : {registers[address]}");
        }
    }
}
