using System.Net.Sockets;
using System.Net;
using System.Data.SQLite;
using NModbus.Data;
using NModbus;
using System.Diagnostics;
using System.Text;
using ActUtlType64Lib;


namespace PlcModbus
{
    public class ModbusServer
    {
        ActUtlType64 plc = new ActUtlType64();
        private TcpListener tcpListener;
        private TcpListener writeListener;
        private TcpListener robotListener;
        private ModbusFactory modbusFactory;
        private IModbusSlaveNetwork slaveNetwork;
        private IModbusSlave slave;
        private SlaveDataStore dataStore;
        private PlcData plcData;
        private Form1 form;
        public bool isConnected = false;
        public string writeCommand;

        public ushort[] robotAxis = new ushort[24]; // 로봇 축 데이터 저장 배열

        // 각 Function Code별 저장 공간을 변수로 선언
        private IPointSource<bool> _0x01; // Coil Inputs
        public IPointSource<bool> _0x02; // Coil Discretes
        private IPointSource<ushort> _0x03; // Input Registers
        public IPointSource<ushort> _0x04; // Holding Registers

        public ModbusServer(PlcData _plcData, ActUtlType64 _plc)
        {
            modbusFactory = new ModbusFactory();
            tcpListener = new TcpListener(IPAddress.Any, 1502); // Modbus TCP 기본 포트
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

        //ModbusMaster에서 쓰기 명령 수신 대기하는 소켓 생성(왠지 모르겠는데 Modbus Socket으로 GerStream이 안돼서 PORT 따로 뺌)
        public void writeConnect() 
        {
            writeListener = new TcpListener(IPAddress.Any, 6001);
            writeListener.Start();
        }
        // Dobot 축 각도 수신
        public void RobotListen()
        {
            robotListener = new TcpListener(IPAddress.Any, 8000);
            robotListener.Start();
            MessageBox.Show("RobotAxis 수신", "정보", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public async void WriteCommand() // 6001번 포트로 쓰기 요청 수신
        {
            try
            {
                await Task.Run(async () =>
                {
                    using TcpClient writeClient = await writeListener.AcceptTcpClientAsync();
                    using NetworkStream stream = writeClient.GetStream();
                    byte[] buffer = new byte[1024];
                    while (true)
                    {
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                        {
                            break;
                        }
                        writeCommand = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Debug.WriteLine($"WriteCommand: {writeCommand}");
                    }
                });
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
        public ushort[] FloatToRegisters(float value) // 4byte float를 2byte unsigned short로 변환
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return new ushort[] {
                BitConverter.ToUInt16(bytes, 0),
                BitConverter.ToUInt16(bytes, 2)
            };
        }
        public async void RobotAxis() // 로봇 축 데이터 수신 후 해당 id에 맞는 축에 저장
        {
            try
            {
                await Task.Run(async () =>
                {
                    using TcpClient robotClient = await robotListener.AcceptTcpClientAsync();
                    using NetworkStream stream = robotClient.GetStream();
                    byte[] buffer = new byte[17];
                    while (true)
                    {
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                        {
                            break;
                        }
                        if (bytesRead == 17)
                        {
                            byte id = buffer[0];
                            float j1 = BitConverter.ToSingle(buffer, 1);
                            float j2 = BitConverter.ToSingle(buffer, 5);
                            float j3 = BitConverter.ToSingle(buffer, 9);
                            float j4 = BitConverter.ToSingle(buffer, 13);

                            ushort[] reg_j1 = FloatToRegisters(j1);
                            ushort[] reg_j2 = FloatToRegisters(j2);
                            ushort[] reg_j3 = FloatToRegisters(j3);
                            ushort[] reg_j4 = FloatToRegisters(j4);

                            if (id == 1)
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    robotAxis[i] = reg_j1[i];
                                    robotAxis[i + 2] = reg_j2[i];
                                    robotAxis[i + 4] = reg_j3[i];
                                    robotAxis[i + 6] = reg_j4[i];
                                }
                            }
                            else if (id == 2)
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    robotAxis[i + 8] = reg_j1[i];
                                    robotAxis[i + 10] = reg_j2[i];
                                    robotAxis[i + 12] = reg_j3[i];
                                    robotAxis[i + 14] = reg_j4[i];
                                }
                            }
                            else if (id == 3)
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    robotAxis[i + 16] = reg_j1[i];
                                    robotAxis[i + 18] = reg_j2[i];
                                    robotAxis[i + 20] = reg_j3[i];
                                    robotAxis[i + 22] = reg_j4[i];
                                }
                            }
                            Debug.WriteLine($"[ID {id}] J1={j1}, J2={j2}, J3={j3}, J4={j4}");
                        }
                    }
                });
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

                // plc값 InputRegisters 업데이트
                while (plcData.FromPlcRegister.Count > 0)
                {
                    ushort reg = (ushort)plcData.FromPlcRegister.Dequeue();
                    _0x03.WritePoints(regAddress, new ushort[] { reg });
                    regAddress++;
                }
                
                //로봇 각도 InputRegisters 업데이터(100~123)
                for (int i = 0; i < 24; i++)
                {
                    _0x03.WritePoints((ushort)(i+100), new ushort[] { robotAxis[i] });
                }

                // plc값 CoilInputs 업데이트
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
                    for (ushort i = 0; i < 100; i++)
                    {
                        plcData.ToPlcRegister.Enqueue(_0x04.ReadPoints(i, 1)[0]);
                    }

                    int[] deviceValue = new int[100];

                    for (int i = 0; i < 100; i++)
                    {
                        if (plcData.ToPlcRegister.TryDequeue(out int register))
                        {
                            deviceValue[i] = register;
                            //deviceValue[i] = this.ToPlcRegister.Dequeue();
                        }
                    }
                    int result2 = plc.WriteDeviceBlock("D0", 100, ref deviceValue[0]);
                    Debug.WriteLine("D 데이터 쓰기 완료");
                    writeCommand = "마스터 쓰기 요청 대기";
                }

                //using SQLiteConnection conn = new SQLiteConnection(strConn);
                //{
                //    conn.Open();
                //    using (SQLiteTransaction transaction = conn.BeginTransaction())
                //    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                //    {
                //        // 디지털 태그 업데이트
                //        for (ushort i = 0; i < 1024; i++)
                //        {
                //            bool value = _0x01.ReadPoints(i, 1)[0];
                //            cmd.CommandText =
                //            "UPDATE DigitalTags SET value = @val, timestamp = datetime('now', '+9 hours') WHERE address = @addr";
                //            cmd.Parameters.AddWithValue("@addr", $"M{i}");
                //            cmd.Parameters.AddWithValue("@val", value ? 1 : 0);
                //            cmd.ExecuteNonQuery();
                //            cmd.Parameters.Clear();
                //        }

                //        // 아날로그 태그 저장
                //        for (ushort i = 0; i < 125; i++)
                //        {
                //            ushort reg = _0x03.ReadPoints(i, 1)[0];
                //            cmd.CommandText =
                //            "UPDATE AnalogTags SET value = @val, timestamp = datetime('now', '+9 hours') WHERE address = @addr";
                //            cmd.Parameters.AddWithValue("@addr", $"D{i}");
                //            cmd.Parameters.AddWithValue("@val", reg);
                //            cmd.ExecuteNonQuery();
                //            cmd.Parameters.Clear();
                //        }
                //        transaction.Commit();
                //    }
                //}
            }
        }
    }
}
