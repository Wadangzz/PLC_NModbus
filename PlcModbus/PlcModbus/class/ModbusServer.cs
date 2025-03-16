using System.Net.Sockets;
using System.Net;
using System.Data.SQLite;
using NModbus.Data;
using NModbus;
using System.Drawing.Text;
using System.Transactions;


namespace PlcModbus
{
    public class ModbusServer
    {
        private TcpListener tcpListener;
        private ModbusFactory modbusFactory;
        private IModbusSlaveNetwork slaveNetwork;
        private IModbusSlave slave;
        private SlaveDataStore dataStore;
        private PlcData plcData;
        public bool isConnected = false;

        // 각 Function Code별 저장 공간을 변수로 선언
        private IPointSource<bool> _0x01; // Coil Inputs
        public IPointSource<bool> _0x02; // Coil Discretes
        private IPointSource<ushort> _0x03; // Input Registers
        public IPointSource<ushort> _0x04; // Holding Registers

        public ModbusServer(PlcData _plcData)
        {
            modbusFactory = new ModbusFactory();
            tcpListener = new TcpListener(IPAddress.Any, 502); // Modbus TCP 기본 포트
            slaveNetwork = modbusFactory.CreateSlaveNetwork(tcpListener);
            dataStore = new SlaveDataStore(); // 슬레이브 데이터 저장 공간
            this.plcData = _plcData; // ReadData()로 불러온 PLC 데이터가 저장된 인스턴스
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
            slaveNetwork.RemoveSlave(1);
            this.isConnected = false;
            MessageBox.Show("Modbus TCP 서버 종료", "정보", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ModbusUpdate()
        {
            string strConn = @"Data Source = C:\\Users\\wadangzz\\Desktop\\Wadangzz\\wadangzz\\PlcModbus\\plc_data.db";

            if (this.isConnected)
            {
                // 데이터 저장소에서 각 Function Code별 데이터 읽기
                _0x01 = dataStore.CoilInputs;
                _0x02 = dataStore.CoilDiscretes;
                _0x03 = dataStore.InputRegisters;
                _0x04 = dataStore.HoldingRegisters;

                ushort j = 0;
                ushort k = 0;

                // HoldingRegisters 업데이트
                while (plcData.FromPlcRegister.Count > 0)
                {
                    _0x03.WritePoints(j, new ushort[] { (ushort)plcData.FromPlcRegister.Dequeue() });
                    j++;
                }

                // CoilInputs 업데이트
                while (plcData.FromPlcCoil.Count > 0)
                {
                    _0x01.WritePoints(k, new bool[] { plcData.FromPlcCoil.Dequeue() });
                    k++;
                }

                // CoilDiscretes 읽기
                for (ushort i = 0; i < 1024; i++)
                {
                    plcData.ToPlcCoil.Enqueue(_0x02.ReadPoints(i, 1)[0]);
                }

                // InputRegisters 읽기
                for (ushort i = 0; i < 125; i++)
                {
                    plcData.ToPlcRegister.Enqueue(_0x04.ReadPoints(i, 1)[0]);
                }
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
                    for (ushort i = 0; i < 125; i++)
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
