using ActUtlType64Lib;
using System.Data.SQLite;



namespace PlcModbus
{
    public partial class Form1 : Form
    {
        public int writeCommand = 0;
        bool isConnected = false;
        static ActUtlType64 plc = new ActUtlType64();//���⼭ ������ �ν��Ͻ��� �ٸ� Ŭ������ ���� �����Ϸ��ٺ��� �������� ����
        static PlcData _data = new PlcData(plc);
        static ModbusServer modbusServer = new ModbusServer(_data, plc);

        private CancellationTokenSource? tokenSource;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            plc.ActLogicalStationNumber = 0;
            int result = plc.Open();
            if (result == 0)
            {
                MessageBox.Show("PLC ���� ����", "", MessageBoxButtons.OK);
                button1.BackColor = Color.AliceBlue;

                modbusServer.StartModbusServer();
                modbusServer.writeConnect();
                modbusServer.RobotListen();

                isConnected = true;
            }
            else
            {
                MessageBox.Show($"PLC ���� ����\n �����ڵ� {result}", "", MessageBoxButtons.OK);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int result = plc.Close();
            if (result == 0)
            {
                MessageBox.Show("PLC ���� ����", "", MessageBoxButtons.OK);
                isConnected = false;
                button1.BackColor = Color.White;

                modbusServer.EndModbusServer();
            }
            else
            {
                MessageBox.Show($"PLC ���� ���� ����\n �����ڵ� {result}", "", MessageBoxButtons.OK);
            }
        }

        private void CurrentTime()
        {
            toolStripStatusLabel1.Text = DateTime.Now.ToString();
        }

        private async Task StartPolling(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Run(() => CurrentTime());
                if (isConnected)
                {
                    await Task.Run(() => _data.ReadData());
                    await Task.Run(() => modbusServer.ModbusUpdate());
                    await Task.Run(() => modbusServer.WriteCommand());
                    await Task.Run(() => modbusServer.RobotAxis());
                }
                await Task.Delay(1, token);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            string strConn = @"Data Source = C:\\Users\\user\\Documents\\GitHub\\wadangzz\\PlcModbus\\plc_data.db";
            using (SQLiteConnection conn = new SQLiteConnection(strConn))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS DigitalTags (
                    id INTEGER PRIMARY KEY,
                    address TEXT NOT NULL,
                    value INTEGER NOT NULL,
                    timestamp DATETIME DEFAULT (datetime('now', '+9 hours'))
                );
                CREATE TABLE IF NOT EXISTS AnalogTags (
                    id INTEGER PRIMARY KEY,
                    address TEXT NOT NULL,
                    value INTEGER NOT NULL,
                    timestamp DATETIME DEFAULT (datetime('now', '+9 hours'))
                );";
                    cmd.ExecuteNonQuery();

                    for (int i = 0; i < 1024; i++)
                    {
                        cmd.CommandText = "INSERT OR IGNORE INTO DigitalTags (id, address, value) VALUES (@id, @addr, 0)";
                        cmd.Parameters.AddWithValue("@id", i);
                        cmd.Parameters.AddWithValue("@addr", $"M{i}");
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }

                    for (int i = 0; i < 125; i++)
                    {
                        cmd.CommandText = "INSERT OR IGNORE INTO AnalogTags (id, address, value) VALUES (@id, @addr, 0)";
                        cmd.Parameters.AddWithValue("@id", i);
                        cmd.Parameters.AddWithValue("@addr", $"D{i}");
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string strConn = @"Data Source = C:\\Users\\wadangzz\\Desktop\\Wadangzz\\wadangzz\\PlcModbus\\plc_data.db";
            using (SQLiteConnection conn = new SQLiteConnection(strConn))
            {
                
                conn.Open();
                using (SQLiteTransaction transaction = conn.BeginTransaction())
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = "UPDATE DigitalTags SET value = 0;";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE AnalogTags SET value = 0;";
                    cmd.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            tokenSource = new CancellationTokenSource();
            await StartPolling(tokenSource.Token);
        }
    }
}
