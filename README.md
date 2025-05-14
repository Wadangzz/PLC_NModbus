# 🚀 PLC_NModbus

## 📘 프로젝트 개요

**PLC_NModbus**는 C# 기반의 Modbus TCP 통신 시스템으로, Mitsubishi PLC와   
외부 장치(Unity, Python, HMI 등)와 실시간 데이터 연동을 목적으로 제작된 프로젝트입니다.  
자동화 라인 구축 프로젝트에서 실제 사용된 코드이며, PLC 제어, 로봇 제어, 데이터 수집, MySQL/SQLite 연동 기능을 포함합니다.

---

## ❓ Why NModbus?

처음에는 TCP 통신의 바이트 패킷을 직접 구성하여 구현하려 했으나,  
Modbus 프로토콜에 맞춰 통신을 설계하는 데에는 시간과 리스크가 컸습니다.  
그러던 중 구조화가 잘 되어 있는 **NModbus 오픈소스 패키지**를 발견하게 되었고,  
GitHub의 Source Code를 직접 분석하여 **Slave와 Master가 어떻게 동작하는지** 구조를 파악하고 프로젝트에 적용하였습니다.

---

## 🔧 주요 기능

- **Modbus TCP Slave 구현**: PLC에서 수집한 데이터를 Modbus 방식으로 외부 시스템에 제공.
- **PLC 실시간 데이터 수신**: Mitsubishi PLC의 디바이스/코일 값을 읽어 내부 데이터스토어에 저장.
- **로봇 축 데이터 수신**: Python 제어기로부터 전송된 로봇 축 데이터를 수신하여 DataStore에 반영.
- **SQLite/DB 기록**: 수집된 데이터를 로컬 DB(SQLite)에 기록.
- **MySQL 연동**: 제품 생산 이벤트 발생 시, 자동으로 ProductCode를 생성하고 MySQL DB에 저장.

---

## 🗂️ 프로젝트 구조

| 경로              | 설명 |
|------------------|------|
| `ModbusMaster/`   | 같은 망에서 Slave에게 읽기 쓰기 요청을 할 수 있는 Modbus Master WinForm |
| `PlcModbus/`      | PLC와 Python 로봇 데이터를 수신하고, Modbus TCP Slave 역할을 수행하는 서버. |
| `plc_data.db`     | SQLite 로컬 DB 파일. |

---

## 🔄 주요 동작 흐름 요약

| 기능 | 설명 |
|------|------|
| PLC 연결 | `ActUtlType64` 라이브러리로 Mitsubishi PLC 접속. |
| Modbus 서버 실행 | `modbusServer.StartModbusServer()` 호출로 Modbus TCP Slave 시작. |
| 데이터 저장 | `modbusServer.ModbusUpdate()`에서 PLC와 Python의 데이터를 Slave DataStore에 저장. |
| 로봇 제어 | `modbusServer.RobotListen()`으로 Python 측 로봇 축 정보 수신 후 `RobotAxis()` 처리. |
| SQLite 기록 | `plc_data.db`에 DigitalTags 등 데이터를 기록 (e.g., 태그값, 타임스탬프). |

---

## 🧩 MySQL 연동 로직 요약

| 조건 | 동작 |
|------|------|
| `M150` 비트가 ON될 때 | 제품 생산 이벤트 발생으로 간주. |
| ProductCode 생성 방식 | `yyyyMMdd + 제품종류 + 당일생산번호` 형식으로 생성. |
| MySQL 저장 | `productcode`, `model`, `dust`, `scratch`, `inspection` 컬럼을 MySQL DB에 기록. |

---

## 🐞 Trouble Shooting

> **문제**: 서보 모터 위치값(int), 로봇의 관절 각도(float)는 Modbus Register의 자료형(2byte ushort)과 호환되지 않음  
> **해결**: Int/Float 값을 `ushort` 2개로 인코딩하여 DataStore에 저장하고,  Master 측에서 다시 디코딩하는 방식을 사용  
>  
> 🔄 그 결과: Unity 기반 Digital Twin에서 **서보 모터 위치 및 로봇 관절 각도를 정확히 재현**할 수 있었음

---

## 💡 To Me

**PLC_NModbus 프로젝트를 통해 하위 디바이스 데이터를 수집하여 상위 시스템과 연동하는 Server–Client 아키텍처를 직접 구현**해보며,  
OPC-UA, SECS/GEM 등 산업용 통신 프로토콜이 지향하는 **시스템 구조와 데이터 흐름**을 체감적으로 이해할 수 있었습니다.  

또한 다양한 자료형의 데이터를 통신 포맷에 맞춰 변환하고 재조립하는 과정을 통해  
**산업현장에서 필요한 통신 설계 능력과 문제 해결력**을 길렀습니다.  
이 경험은 향후, 스마트팩토리 시스템 구축 시 **PLC–로봇–상위 시스템 간 데이터 흐름을 전체적으로 설계하고 통합할 수 있는 실전 역량**으로 이어질 수 있을 것이라 확신합니다.
