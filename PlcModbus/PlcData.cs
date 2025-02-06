using System;
using System.Windows.Form;
using ActUtlType64Lib;
using PlcModbus;

namespace PlcClass
{
    public class PlcData
    {

        ActUtlType64 plc_data = new ActUtlType64();

        //Form1에서 호출한 actUtlType 인스턴스 정보(ex: PLC 접속 정보)를 그대로 이용하기 위함
        public PlcData(ActUtlType actUtlType)
        {
            plc_data = actUtlType;
        }
        //변수를 호출할 때는 get으로 return값을 불러오고
        //변수에 값을 넣을 땐 set으로 값을 밀어 넣음
        public Queue<int> toPlc = new Queue<int>();
        public Queue<int> fromPlc = new Queue<int>();

        public Queue<int> ToPlc

        {
            get { return toPlc; }
            set { toPlc = value; }
        }

        public Queue<int> FromPlc
        {
            get { return fromPlc; }
            set { fromPlc = value; }
        }

        public void ReadWord()
        {

            int[] deviceValues = new int[200];
            int result = plc_data.ReadDeviceBlock("D0", 200, out deviceValues[0]);
            if (result == 0)
            {
                for (int i = 0; i < 200; i++)
                {
                    this.FromPlc.Enqueue(deviceValues[i]);
                }
            }
            
        }


        private void WriteWord()
        {

            int[] deviceValues = new int[100];
            for (int i = 0; i < 100; i++)
            {
                if (ToPlc.Count > 0)
                {
                    deviceValues[i] = ToPlc.Dequeue();
                }
                else
                {
                    deviceValues[i] = 0;
                }
            }
            plc_data.WriteDeviceBlock("D200", 100, ref deviceValues[0]);
        }
    }
}

