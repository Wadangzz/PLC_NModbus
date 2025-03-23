using ActUtlType64Lib;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Xml.Linq;
using System.Threading;
using System.Data.SQLite;
using System.Text;


namespace PlcModbus
{
    public partial class Form1 : Form
    {
        //delegate void TimerEventFiredDelegate();

        public int writeCommand = 0;
        bool isConnected = false;
        static ActUtlType64 plc = new ActUtlType64();
        static PlcData _data = new PlcData(plc);
        static ModbusServer modbusServer = new ModbusServer(_data, plc);


        private CancellationTokenSource? tokenSource;


        //private System.Threading.Timer readDataTimer = null!;
        //private System.Threading.Timer modbusUpdateTimer = null!;
        //private System.Threading.Timer writeDataTimer = null!;


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
                button1.BackColor = Color.AliceBlue;

                modbusServer.StartModbusServer();
                _data.YoloConnect();
                modbusServer.writeConnect();

                isConnected = true;

                // 멀티쓰레드로 타이머 실행
                //readDataTimer = new System.Threading.Timer(ReadDataTimer_Tick, null, 0,1);
                //modbusUpdateTimer = new System.Threading.Timer(ModbusUpdateTimer_Tick, null, 0,1);
                //writeDataTimer = new System.Threading.Timer(WriteDataTimer_Tick, null, 0,1);
            }
            else
            {
                MessageBox.Show($"PLC 연결 실패\n 에러코드 {result}", "", MessageBoxButtons.OK);
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
                _data.YoloDisconnect();

                // 타이머 중지
                //readDataTimer?.Change(Timeout.Infinite, Timeout.Infinite);
                //modbusUpdateTimer?.Change(Timeout.Infinite, Timeout.Infinite);
                //writeDataTimer?.Change(Timeout.Infinite, Timeout.Infinite);
            }
            else
            {
                MessageBox.Show($"PLC 연결 해제 실패\n 에러코드 {result}", "", MessageBoxButtons.OK);
            }
        }

        //private void timer1_Tick(object sender, EventArgs e)
        //{
        //    if (isConnected)
        //    {
        //        _data.ReadData();
        //        modbusServer.ModbusUpdate();
        //        _data.WriteData();
        //    }
        //}

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
                    await Task.Run(() => _data.YoloDetect());
                    await Task.Run(() => modbusServer.ModbusUpdate());
                    await Task.Run(() => modbusServer.WriteCommand());
                }
                await Task.Delay(1, token);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            string strConn = @"Data Source = C:\\Users\\wadangzz\\Desktop\\Wadangzz\\wadangzz\\PlcModbus\\plc_data.db";
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

    //private void ReadDataTimer_Tick(object? garbage)
    //{
    //    if (isConnected)
    //    {
    //        this.BeginInvoke(new TimerEventFiredDelegate(_data.ReadData));
    //    }
    //}


    //private void ModbusUpdateTimer_Tick(object? garbage)
    //{
    //    if (isConnected)
    //    {
    //        this.BeginInvoke(new TimerEventFiredDelegate(modbusServer.ModbusUpdate));
    //    }
    //}

    //private void WriteDataTimer_Tick(object? garbage)
    //{
    //    if (isConnected)
    //    {
    //        this.BeginInvoke(new TimerEventFiredDelegate(_data.WriteData));
    //    }
    //}


