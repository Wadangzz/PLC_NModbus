using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.Security.Claims;
using System.Windows.Forms;
using ActUtlType64Lib;


namespace PlcModbus
{
    public class PlcData
    {

        ActUtlType64 plc_data = new ActUtlType64();

        //Form1에서 호출한 actUtlType 인스턴스 정보(ex: PLC 접속 정보)를 그대로 이용하므로
        //actUtlType을 static으로 선언 후 생성자 함수의 매개변수로 연결
        public PlcData(ActUtlType64 actUtlType)
        {
            plc_data = actUtlType;
        }
        //변수를 호출할 때는 get으로 return값을 불러오고
        //변수에 값을 넣을 땐 set으로 값을 밀어 넣음
        public Queue<int> ToPlcRegister = new Queue<int>();
        public Queue<int> FromPlcRegister = new Queue<int>();
        public Queue<bool> FromPlcCoil = new Queue<bool>();
        public Queue<bool> ToPlcCoil = new Queue<bool>();

        public int[] deviceValues = new int[125];
        public int[] mValues = new int[64];
        public bool[] coilValues = new bool[1024];

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

        public void WriteData()
        {

            int[] writeValues = new int[1024];
            int[] mValue = new int[64];
            int[] deviceValue = new int[125];

                //for (int i = 0;i < 5; i++)
                //{
                //    writeValues[i] = Convert.ToInt32(this.ToPlcCoil.Dequeue());
                //    plc_data.SetDevice($"M{i + 100}", writeValues[i]);
                //}

                for (int i = 0; i < 1024; i++)
            {
                if (ToPlcCoil.TryDequeue(out bool coil))
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
            int result = plc_data.WriteDeviceBlock("M0", 64, ref mValue[0]);


            for (int i = 0; i < 125; i++)
            {
                if (ToPlcRegister.TryDequeue(out int register))
                {
                    deviceValue[i] = register;
                    //deviceValue[i] = this.ToPlcRegister.Dequeue();
                }
            }
            int result2 = plc_data.WriteDeviceBlock("D0", 125, ref deviceValue[0]);
        }
    }
}


