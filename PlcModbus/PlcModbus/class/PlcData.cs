using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security.Claims;
using System.Text;
using System.Windows.Forms;
using ActUtlType64Lib;


namespace PlcModbus
{
    public class PlcData
    {

        ActUtlType64 plc_data = new ActUtlType64();
        TcpListener yoloListener;


        //Form1에서 호출한 actUtlType 인스턴스 정보(ex: PLC 접속 정보)를 그대로 이용하므로
        //actUtlType을 static으로 선언 후 생성자 함수의 매개변수로 연결
        public PlcData(ActUtlType64 actUtlType)
        {
            plc_data = actUtlType;
        }
     
        public Queue<int> ToPlcRegister = new Queue<int>();
        public Queue<int> FromPlcRegister = new Queue<int>();
        public Queue<bool> FromPlcCoil = new Queue<bool>();
        public Queue<bool> ToPlcCoil = new Queue<bool>();

        public int[] deviceValues = new int[125];
        public int[] mValues = new int[64];
        public bool[] coilValues = new bool[1024];

        public void YoloConnect()
        {
            yoloListener = new TcpListener(IPAddress.Any, 6000);
            yoloListener.Start();
            MessageBox.Show("Yolo Detect 시작", "information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void YoloDisconnect()
        {
            yoloListener.Stop(); // TCP 소켓 Close
            MessageBox.Show("Yolo Detect 종료", "information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public async void YoloDetect()
        {
            try
            {
                using TcpClient yoloClient = await yoloListener.AcceptTcpClientAsync();
                using NetworkStream stream = yoloClient.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                string[]? parts = data.Split(',');
                int address = int.Parse(parts[0]);
                int cls = int.Parse(parts[1]);
                plc_data.SetDevice($"D{address}", cls);
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

        public void ReadData()
        {

            //PLC에서 3번 Function Code 최대 개수 만큼 D0에서 불러옮
            int result = plc_data.ReadDeviceBlock("D0", 125, out deviceValues[0]);
            if (result == 0)
            {
                //Debug.WriteLine($"deviceValues[{0}] = {deviceValues[0]}");
                // deviceValues 배열의 값을 FromPlcRegister Queue에 추가.
                for (int i = 0; i < 125 ; i++)
                {
                    this.FromPlcRegister.Enqueue(deviceValues[i]);
                }
            }

            result = plc_data.ReadDeviceBlock("M0", 64, out mValues[0]);
            if (result == 0)
            {
                // mValues를 2진수 문자열로 변환하고, 왼쪽에 '0'을 채워 16비트로 변환.
                //Convert.ToString(mValues, 2).PadLeft(16, '0');

                // 각 비트를 bool 형식으로 변환하여 coilValues 배열에 저장.
                for (ushort i = 0; i < mValues.Length * 16; i++)
                {
                    coilValues[i] = (mValues[i/16] & (1 << i%16)) != 0;
                }
                // coilValues 배열의 값을 FromPlcCoil Queue에 추가.
                for (int i = 0; i < mValues.Length * 16; i++)
                {
                    this.FromPlcCoil.Enqueue(coilValues[i]);
                }
            }
        }


        //마스터에서 쓰기요청이 올 때만 쓰려고 ModbusServer 클래스로 이전

        //public void WriteData()
        //{

        //    int[] writeValues = new int[1024];
        //    int[] mValue = new int[64];
        //    int[] deviceValue = new int[123];


        //    for (int i = 0; i < 1024; i++)
        //    {
        //        if (ToPlcCoil.TryDequeue(out bool coil))
        //        {
        //            writeValues[i] = Convert.ToInt32(coil);
        //        }
        //    }


        // WriteDeviceBlock은 Word단위로 쓰기 때문에
        // 16비트씩 64개의 값을 10진수로 변환하여 mValue 배열에 저장.
        //    for (int i = 0; i < 64; i++)
        //    {
        //        for (int j = 0; j < 16; j++)
        //        {
        //            mValue[i] += (int)Math.Pow(2, j) * writeValues[j + (i * 16)];
        //        }
        //    }
        //    int result = plc_data.WriteDeviceBlock("M0", 64, ref mValue[0]);


        //    for (int i = 0; i < 123; i++)
        //    {
        //        if (ToPlcRegister.TryDequeue(out int register))
        //        {
        //            deviceValue[i] = register;
        //            //deviceValue[i] = this.ToPlcRegister.Dequeue();
        //        }
        //    }
        //    Debug.WriteLine(deviceValue[0]);
        //    int result2 = plc_data.WriteDeviceBlock("D0", 123, ref deviceValue[0]);
        //    Debug.WriteLine("쓰기 완료");
        //}
    }
}


