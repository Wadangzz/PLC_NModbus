using System.Net.Sockets;
using System.Net;
using System.Data.SQLite;
using NModbus.Data;
using NModbus;
using System.Diagnostics;
using System.Text;
using NModbus.Device;
using ActUtlType64Lib;

namespace PlcModbus
{
    public class ModbusServer
    {
        ActUtlType64 plc = new ActUtlType64();
        private TcpListener tcpListener;
        private TcpListener writeListener;
        private ModbusFactory modbusFactory;
        private IModbusSlaveNetwork slaveNetwork;
        private IModbusSlave slave;
        private SlaveDataStore dataStore;
        private PlcData plcData;
        private Form1 form;
        public bool isConnected = false;
        public string writeCommand;

        // 각 Function Code별 저장 공간을 변수로 선언
        private IPointSource<bool> _0x01; // Coil Inputs
        public IPointSource<bool> _0x02; // Coil Discretes
        private IPointSource<ushort> _0x03; // Input Registers
        public IPointSource<ushort> _0x04; // Holding Registers

        public ModbusServer(PlcData _plcData, ActUtlType64 _plc)
        {
            modbusFactory = new ModbusFactory();
            tcpListener = new TcpListener(IPAddress.Any, 502); // Modbus TCP 기본 포트
            slaveNetwork = modbusFactory.CreateSlaveNetwork(tcpListener);
            dataStore = new SlaveDataStore(); // 슬레이브 데이터 저장 공간
            this.plcData = _plcData; // ReadData()로 불러온 PLC 데이터가 저장된 인스턴스
            this.plc = _plc; // MX Component 인스턴스
        }

        public void StartModbusServer()
        {
            tcpListener.Start(); // TCP 소켓 OPEN

            slave = modbusFactory.CreateSlave(1, dataStore);
            slaveNetwork.AddSlave(slave);
            slaveNetwork.ListenAsync(); // 비동기로 서버 실행

            this.isConnected = true;
            MessageBox.Show("Modbus TCP 서버 시작", "정보", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void EndModbusServer()
        {
            tcpListener.Stop(); // TCP 소켓 Close
            writeListener.Stop();
            slaveNetwork.RemoveSlave(1);
            this.isConnected = false;
            MessageBox.Show("Modbus TCP 서버 종료", "정보", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public void writeConnect()
        {
            writeListener = new TcpListener(IPAddress.Any, 6001);
            writeListener.Start();
        }
        public async void WriteCommand()
        {
            try
            {
                using TcpClient writeClient = await writeListener.AcceptTcpClientAsync();
                using NetworkStream stream = writeClient.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                writeCommand = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Debug.WriteLine($"WriteCommand: {writeCommand}");
            }
            catch (SocketException ex)
            {
                Debug.WriteLine($"SocketException: {ex.Message}");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
        public void ModbusUpdate()
        {
            string strConn = @"Data Source = C:\\Users\\user\\Documents\\GitHub\\wadangzz\\PlcModbus\\plc_data.db";

            if (this.isConnected)
            {
                // 데이터 저장소에서 각 Function Code별 데이터 읽기
                _0x01 = dataStore.CoilInputs;
                _0x02 = dataStore.CoilDiscretes;
                _0x03 = dataStore.InputRegisters;
                _0x04 = dataStore.HoldingRegisters;

                ushort regAddress = 0;
                ushort coilAddress = 0;

                // InputRegisters 업데이트
                while (plcData.FromPlcRegister.Count > 0)
                {
                    ushort reg = (ushort)plcData.FromPlcRegister.Dequeue();
                    _0x03.WritePoints(regAddress, new ushort[] { reg });
                    regAddress++;
                }

                // CoilInputs 업데이트
                while (plcData.FromPlcCoil.Count > 0)
                {
                    bool coil = plcData.FromPlcCoil.Dequeue();
                    _0x01.WritePoints(coilAddress, new bool[] { coil });
                    coilAddress++;
                }
                // 마스터로부터 쓰기 요청이 있을 때
                if (writeCommand == "M")
                {
                    // CoilDiscretes 읽기
                    for (ushort i = 0; i < 1024; i++)
                    {
                        plcData.ToPlcCoil.Enqueue(_0x02.ReadPoints(i, 1)[0]);
                    }
                   
                    int[] writeValues = new int[1024];
                    int[] mValue = new int[64];


                    for (int i = 0; i < 1024; i++)
                    {
                        if (plcData.ToPlcCoil.TryDequeue(out bool coil))
                        {
                            writeValues[i] = Convert.ToInt32(coil);
                        }
                    }
                    for (int i = 0; i < 64; i++)
                    {
                        for (int j = 0; j < 16; j++)
                        {
                            mValue[i] += (int)Math.Pow(2, j) * writeValues[j + (i * 16)];
                        }
                    }
                    int result = plc.WriteDeviceBlock("M0", 64, ref mValue[0]);
                    Debug.WriteLine("M 메모리 쓰기 완료");
                    writeCommand = "마스터 쓰기 요청 대기";
                }
                else if (writeCommand == "D")
                { 
                    // HoldingRegisters 읽기
                    for (ushort i = 0; i < 123; i++)
                    {
                        plcData.ToPlcRegister.Enqueue(_0x04.ReadPoints(i, 1)[0]);
                    }

                    int[] deviceValue = new int[123];

                    for (int i = 0; i < 123; i++)
                    {
                        if (plcData.ToPlcRegister.TryDequeue(out int register))
                        {
                            deviceValue[i] = register;
                            //deviceValue[i] = this.ToPlcRegister.Dequeue();
                        }
                    }
                    int result2 = plc.WriteDeviceBlock("D0", 123, ref deviceValue[0]);
                    Debug.WriteLine("D 데이터 쓰기 완료");
                    writeCommand = "마스터 쓰기 요청 대기";
                }

                using SQLiteConnection conn = new SQLiteConnection(strConn);
                {
                    conn.Open();
                    using (SQLiteTransaction transaction = conn.BeginTransaction())
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        // 디지털 태그 업데이트
                        for (ushort i = 0; i < 1024; i++)
                        {
                            bool value = _0x01.ReadPoints(i, 1)[0];
                            cmd.CommandText =
                            "UPDATE DigitalTags SET value = @val, timestamp = datetime('now', '+9 hours') WHERE address = @addr";
                            cmd.Parameters.AddWithValue("@addr", $"M{i}");
                            cmd.Parameters.AddWithValue("@val", value ? 1 : 0);
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }

                        // 아날로그 태그 저장
                        for (ushort i = 0; i < 123; i++)
                        {
                            ushort reg = _0x03.ReadPoints(i, 1)[0];
                            cmd.CommandText =
                            "UPDATE AnalogTags SET value = @val, timestamp = datetime('now', '+9 hours') WHERE address = @addr";
                            cmd.Parameters.AddWithValue("@addr", $"D{i}");
                            cmd.Parameters.AddWithValue("@val", reg);
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                        transaction.Commit();
                    }
                }
            }
        }
    }
}
