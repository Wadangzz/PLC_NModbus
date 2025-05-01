using ActUtlType64Lib;
using MySql.Data.MySqlClient;

namespace PlcModbus
{
    public class PlcData
    {

        ActUtlType64 plc_data = new ActUtlType64();

        //Form1에서 호출한 actUtlType 인스턴스 정보(ex: PLC 접속 정보)를 그대로 이용하므로
        //actUtlType을 static으로 선언 후 생성자 함수의 매개변수로 연결
        //이제와서 느낀건데 PLC StationNum을 한개만 사용할라고 하니까
        //너무 비직관적인 코드가 된거 같음
        //어차피 같은 PLC에 StationNum 다르게 접근해도 다 똑같은 값을 불러올 수 있는데
        //Station 갯수가 모자라거나 리소스 관리가 필요할 때는 이게 맞을지도......
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

        private Dictionary<int, int> productMap = new Dictionary<int, int> {
                                        { 1, 0 }, // A
                                        { 2, 1 }, // B
                                        { 4, 2 }  // C
                                    };
        private Dictionary<int, char> productModel = new Dictionary<int, char> {
                                        { 1, 'A' }, // A
                                        { 2, 'B' }, // B
                                        { 4, 'C' }  // C
                                    };

        private string lastDate = "";   // 날짜 비교용
        private int[] productCount = new int[3];   // 당일 생산 갯수 확인용 제품이 추가되면 배열 갯수도 추가
        private bool prevM150 = false; // M100의 이전 값 저장용

        public void ReadData()
        {
            //PLC에서 D0~D99 불러옮
            int result = plc_data.ReadDeviceBlock("D0", 100, out deviceValues[0]);
            if (result == 0)
            {
                //Debug.WriteLine($"deviceValues[{0}] = {deviceValues[0]}");
                // deviceValues 배열의 값을 FromPlcRegister Queue에 추가.
                for (int i = 0; i < 100; i++)
                {
                    this.FromPlcRegister.Enqueue(deviceValues[i]);
                }
            }
            // M0~M1023 불러옮
            result = plc_data.ReadDeviceBlock("M0", 64, out mValues[0]);
            if (result == 0)
            {
                // mValues를 2진수 문자열로 변환하고, 왼쪽에 '0'을 채워 16비트로 변환.
                //Convert.ToString(mValues, 2).PadLeft(16, '0');
                // 각 비트를 bool 형식으로 변환하여 coilValues 배열에 저장.
                for (ushort i = 0; i < mValues.Length * 16; i++)
                {
                    coilValues[i] = (mValues[i / 16] & (1 << i % 16)) != 0;
                }
                // coilValues 배열의 값을 FromPlcCoil Queue에 추가.
                for (int i = 0; i < mValues.Length * 16; i++)
                {
                    this.FromPlcCoil.Enqueue(coilValues[i]);
                }
            }

            string strConn = @"server=127.0.0.1;
                            port=3306;database=product_db;
                            uid=product;password=**********;";
            if (!prevM150 && coilValues[150])
                using (MySqlConnection conn = new MySqlConnection(strConn))
                {
                    conn.Open();
                    {
                        int product = deviceValues[5];
                        string today = DateTime.Now.ToString("yyyyMMdd");

                        if (productMap.ContainsKey(product))
                        {
                            int index = productMap[product];      // 0, 1, 2
                            char model = productModel[product];  // A, B, C
                            // 날짜가 바뀌었으면
                            if (today != lastDate)
                            {
                                lastDate = today;
                                productCount[index] = 1; // 당일 생산 갯수 초기화
                            }
                            else
                            {
                                productCount[index]++; // 당일 생산 갯수 증가
                            }

                            string productCode = $"{today}-{model}{productCount[index]}";

                            using (MySqlCommand cmd = new MySqlCommand(
                                @"INSERT IGNORE INTO productnum 
                                (Product, Model, Dust, Scratch, inspection)
                                VALUES (@ProductCode, @Model, @Dust, @Scratch, @inspection)", conn))
                            {
                                cmd.Parameters.AddWithValue("@ProductCode", productCode);
                                cmd.Parameters.AddWithValue("@Model", model);
                                cmd.Parameters.AddWithValue("@Dust", 0);
                                cmd.Parameters.AddWithValue("@Scratch", 0);
                                cmd.Parameters.AddWithValue("@inspection", "Not Yet");
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            prevM150 = coilValues[150]; // 다음 비교를 위해 상태 저장
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



