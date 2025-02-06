using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using NModbus.Data;
using NModbus;



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
        
        private IPointSource<bool> _0x01;
        public IPointSource<bool> _0x02;
        private IPointSource<ushort> _0x03;
        public IPointSource<ushort> _0x04;


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
            if (this.isConnected)
            {

                _0x01 = dataStore.CoilInputs;
                //_0x02 = dataStore.CoilDiscretes;
                _0x03 = dataStore.HoldingRegisters;
                //_0x04 = dataStore.InputRegisters;
                
                
                ushort j = 0;
                ushort k = 0;
                
                // HoldingRegisters 업데이트
                
                while (plcData.FromPlcRegister.Count > 0)
                {
                    _0x03.WritePoints(j, new ushort[] { (ushort)plcData.FromPlcRegister.Dequeue() });
                    j++;
                }

                //CoilInputs 업데이트
                while (plcData.FromPlcCoil.Count > 0)
                {

                    _0x01.WritePoints(k, new bool[] { plcData.FromPlcCoil.Dequeue() });
                    k++;
                }

                //Debug.WriteLine(plcData.FromPlcCoil.Count);
                //for (ushort i = 100; i < 110; i++)
                //{   
                //    _0x02.WritePoints(i, new bool[] { plcData.FromPlcCoil.Dequeue() });
                //}


                ////D0~9(HoldingRegisters) 값 출력
                //Debug.WriteLine("Holding Registers (D0~D9):");
                //for (ushort i = 0; i < 10; i++)
                //{
                //    Debug.Write($"D{i}: {_0x03.ReadPoints(i, 1)[0]} ");
                //}
                //Debug.WriteLine("");

                ////M0~9(CoilInputs) 값 출력
                //Debug.WriteLine("Coil Output (M0~M9):");
                //for (ushort i = 0; i < 10; i++)
                //{
                //    Debug.Write($"M{i}: {_0x01.ReadPoints(i, 1)[0]} ");
                //}
                //Debug.WriteLine("");

                //Debug.WriteLine("Coil Input (M100~M109):");
                //for (ushort i = 80; i < 90; i++)
                //{
                //    Debug.Write($"M{i}: {_0x01.ReadPoints(i, 1)[0]} ");
                //}
                //Debug.WriteLine("");

                //Debug.WriteLine("마스터 요청(M100~M104):");
                //for (ushort i = 80; i < 85; i++)
                //{
                //    Debug.Write($"M{i}: {_0x02.ReadPoints(i, 1)[0]} ");
                //}
                //Debug.WriteLine("");

                //for (ushort i = 80; i < 85; i++)
                //{
                //    plcData.ToPlcCoil.Enqueue(_0x02.ReadPoints(i, 1)[0]);
                //}
            }
        }
    }
}
