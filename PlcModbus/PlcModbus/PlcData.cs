using ActUtlType64Lib;
using MySql.Data.MySqlClient;
using System.Linq.Expressions;

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

        private Dictionary<int, char> productData = 
            new Dictionary<int, char> {
                                        { 1, 'A' }, // A
                                        { 2, 'B' }, // B
                                        { 4, 'C' }  // C
                                    };

        private string find_Maxnum = @"SELECT MAX(num) FROM(
                                    SELECT CAST(SUBSTRING(ProductCode, LENGTH(@prefix) + 1) AS UNSIGNED) AS num
                                    FROM productnum
                                    WHERE ProductCode LIKE @prefixLike

                                    UNION ALL

                                    SELECT CAST(SUBSTRING(ProductCode, LENGTH(@prefix) + 1) AS UNSIGNED) AS num
                                    FROM ok
                                    WHERE ProductCode LIKE @prefixLike

                                    UNION ALL

                                    SELECT CAST(SUBSTRING(ProductCode, LENGTH(@prefix) + 1) AS UNSIGNED) AS num
                                    FROM ng
                                    WHERE ProductCode LIKE @prefixLike
                                ) AS all_codes";
        
        private string insert_productnum = @"INSERT IGNORE INTO productnum 
                                    (ProductCode, Model, Dust, Scratch, inspection)
                                    VALUES (@ProductCode, @Model, @Dust, @Scratch, @inspection)";

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
                            uid=product;password=*****;";

            // MySQL 연결 문자열을 사용하여 MySQL 데이터베이스에 연결.
            if (!prevM150 && coilValues[150])
            {
                using (MySqlConnection conn = new MySqlConnection(strConn))
                {
                    conn.Open();
                    using (MySqlTransaction mySqlTransaction = conn.BeginTransaction())
                    {   
                        try
                        { 
                            int product = deviceValues[5]; // 1, 2, 4
                            string today = DateTime.Now.ToString("yyyyMMdd");

                            // 제품 모델에 대한 정보를 저장하는 딕셔너리에서 제품 번호를 찾음
                            if (!productData.ContainsKey(product))
                                return;
                            
                            char model = productData[product];  // A, B, C
                            string prefix = $"{today}-{model}";
                            int nextNumber = 1;

                            using (MySqlCommand cmd = new MySqlCommand(find_Maxnum, conn, mySqlTransaction))
                            {
                                cmd.Parameters.AddWithValue("@prefix", prefix);
                                cmd.Parameters.AddWithValue("@prefixLike", prefix + "%");

                                object find_Result = cmd.ExecuteScalar();
                                if (find_Result != DBNull.Value && find_Result != null)
                                {
                                    nextNumber = Convert.ToInt32(find_Result) + 1; // 다음 번호
                                }
                            }

                            string productCode = $"{prefix}{nextNumber}";

                            using (MySqlCommand cmd = new MySqlCommand(insert_productnum, conn, mySqlTransaction))
                            {
                                cmd.Parameters.AddWithValue("@ProductCode", productCode);
                                cmd.Parameters.AddWithValue("@Model", model);
                                cmd.Parameters.AddWithValue("@Dust", 0);
                                cmd.Parameters.AddWithValue("@Scratch", 0);
                                cmd.Parameters.AddWithValue("@inspection", "Not Yet");
                                cmd.ExecuteNonQuery();
                            }
                            mySqlTransaction.Commit();
                        }
                        catch (MySqlException ex)
                        {
                            // 예외 처리
                            Console.WriteLine($"MySQL 오류: {ex.Message}");
                            mySqlTransaction.Rollback();
                        }
                    }
                }
            }
            prevM150 = coilValues[150]; // 다음 비교를 위해 상태 저장
        }
    }
}