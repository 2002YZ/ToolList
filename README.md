# Helper 工具库文档

本项目是一套适用于工业自动化上位机开发的 C# 通用帮助工具库，包含：

- Modbus RTU/TCP 通信
- SQL Server 数据库访问
- JSON 序列化
- CSV 文件导入导出
- INI 配置文件管理
- 文件与目录操作
- 十六进制数据转换
- 日志记录功能

可直接用于 WinForm、WPF、MES、SCADA、PLC 通信等项目开发。

---

## 📦 项目依赖

* **.NET Framework 4.8** 或 **.NET Core 3.1+**
* **NuGet 包**：

  * `Newtonsoft.Json`（用于 JSON 序列化）
  * `NModbus`（用于 Modbus TCP 主站）
  * `System.Data.SqlClient`（用于 SQL Server 访问）

---

## 🧩 模块说明

### 1. `CrcHelper` 类 (`ModBus类库封装`)

提供 Modbus RTU 协议的 CRC-16 校验计算、验证，以及各种 Modbus 报文的生成（RTU 格式）。

| 方法                                       | 说明                    |
| ---------------------------------------- | --------------------- |
| `Crc16(byte[] data, int length)`         | 计算 CRC-16（多项式 0xA001） |
| `VerifyCrc(byte[] data, int length)`     | 验证接收数据的 CRC 是否匹配      |
| `GenerateMessage(...)`                   | 生成读取报文（功能码 01~04）     |
| `GenerateWrite(...)`                     | 生成写入单个寄存器报文（功能码 06）   |
| `GenerateWriteMessage(...)`              | 生成写入多个寄存器报文（功能码 10）   |
| `GenerateWriteCoilMessage(...)`          | 生成写入单个线圈报文（功能码 05）    |
| `GenerateWriteMultipleCoilsMessage(...)` | 生成写入多个线圈报文（功能码 0F）    |

**示例：**

```csharp
byte[] readMsg = CrcHelper.GenerateMessage(1, 0x03, 0x0000, 10);
```

---

### 2. `DBHelper` 类 (`Common`)

封装 SQL Server 数据库的通用操作，支持普通 SQL 和存储过程，并提供分页辅助属性。

#### 属性

| 属性          | 说明      |
| ----------- | ------- |
| `Count`     | 总记录数    |
| `PageSize`  | 每页大小    |
| `PageBegin` | 当前页起始索引 |
| `PageCount` | 总页数     |

#### 方法

| 方法                  | 说明             |
| ------------------- | -------------- |
| `InitData()`        | 初始化分页数据        |
| `GetBookList()`     | 获取书籍集合         |
| `CommandSQL()`      | 执行增删改          |
| `ExecuteNonQuery()` | 执行非查询命令        |
| `GetDataTablList()` | 返回 BindingList |
| `GetDataTable()`    | 返回 DataTable   |
| `ExecuteScalar()`   | 返回单一值          |

**示例：**

```csharp
string sql = "SELECT * FROM Book WHERE Price > @price";

SqlParameter[] p =
{
    new SqlParameter("@price", 50)
};

DataTable dt =
    DBHelper.GetDataTable(sql, p, false);
```

---

### 3. `HexByteConvert` 类 (`ModBus_TCP.Helper`)

提供十六进制字符串与字节数组之间的转换，以及 ushort 与字节数组转换。

| 方法                       | 说明              |
| ------------------------ | --------------- |
| `HexStringToByteArry()`  | 十六进制字符串转字节数组    |
| `ByteArrayToHexString()` | 字节数组转十六进制字符串    |
| `ByteToUshort(byte[])`   | 字节数组转 ushort 数组 |
| `ByteToUshort(ushort[])` | ushort 数组转字节数组  |
| `StringToUshort()`       | 字符串转 ushort 数组  |

**示例：**

```csharp
byte[] raw = { 0x01, 0x03, 0x00 };

string hex =
    HexByteConvert.ByteArrayToHexString(raw);
```

---

### 4. `JsonHelper` 类 (`Common`)

基于 Newtonsoft.Json 实现对象的 JSON 序列化与反序列化。

| 方法                 | 说明            |
| ------------------ | ------------- |
| `SaveToJson<T>()`  | 保存对象到 JSON    |
| `Deserialize<T>()` | 从 JSON 文件读取对象 |

**示例：**

```csharp
JsonHelper.SaveToJson(books, "books.json");

var loaded =
    JsonHelper.Deserialize<List<BookInfo>>
    (
        "books.json"
    );
```

---

### 5. `CsvHelper` 类 (`ToolsLib`)

提供 CSV 文件与 DataTable、List<T> 之间的相互转换功能。

#### CSV读取

| 方法                 | 说明            |
| ------------------ | ------------- |
| `CsvToDataTable()` | CSV转DataTable |
| `CsvToList<T>()`   | CSV转List      |

#### CSV导出

| 方法                 | 说明             |
| ------------------ | -------------- |
| `DataTableToCsv()` | DataTable导出CSV |
| `ListToCsv<T>()`   | List导出CSV      |
| `WriteToCsv()`     | 追加写入CSV        |

#### 数据转换

| 方法                     | 说明             |
| ---------------------- | -------------- |
| `DataTableToList<T>()` | DataTable转List |
| `ListToDataTable<T>()` | List转DataTable |

**示例：**

```csharp
DataTable dt =
    CsvHelper.CsvToDataTable
    (
        "student.csv",
        true
    );

List<Student> list =
    CsvHelper.CsvToList<Student>
    (
        "student.csv",
        true
    );

CsvHelper.ListToCsv
(
    list,
    "result.csv",
    true
);
```

---

### 6. `FileHelper` 类 (`ToolsLib`)

提供文本文件、对象序列化以及文件夹管理等功能。

#### 文本文件操作

| 方法              | 说明   |
| --------------- | ---- |
| `WriteToTxt()`  | 写入文本 |
| `ReadFromTxt()` | 读取文本 |

#### 对象序列化

| 方法                              | 说明        |
| ------------------------------- | --------- |
| `SerializeObject()`             | 序列化对象到文件  |
| `DeSerializeObject<T>()`        | 文件反序列化    |
| `SerializeObjToString()`        | 序列化对象为字符串 |
| `DeSerializeObjFromString<T>()` | 字符串反序列化   |

#### 文件操作

| 方法             | 说明   |
| -------------- | ---- |
| `CopyFile()`   | 文件复制 |
| `MoveFile()`   | 文件移动 |
| `DeleteFile()` | 文件删除 |

#### 目录操作

| 方法                  | 说明    |
| ------------------- | ----- |
| `GetFiles()`        | 获取文件  |
| `GetDirectories()`  | 获取子目录 |
| `CreateDirectory()` | 创建目录  |
| `DeleteFiles()`     | 删除目录  |

**示例：**

```csharp
FileHelper.WriteToTxt
(
    "log.txt",
    "Program Start",
    true
);

string content =
    FileHelper.ReadFromTxt("log.txt");

FileHelper.CopyFile
(
    "source.txt",
    "target.txt"
);
```

> ⚠️ 当前对象序列化功能使用 BinaryFormatter，该技术已被微软标记为过时，仅建议兼容历史项目。

---

### 7. `IniConfigHelper` 类 (`ToolsLib`)

用于读取和写入 Windows INI 配置文件。

#### 属性

| 属性        | 说明       |
| --------- | -------- |
| `IniPath` | 默认配置文件路径 |

#### 配置读写

| 方法               | 说明   |
| ---------------- | ---- |
| `ReadIniData()`  | 读取配置 |
| `WriteIniData()` | 写入配置 |

#### 节点管理

| 方法               | 说明              |
| ---------------- | --------------- |
| `ReadSections()` | 获取所有Section     |
| `ReadKeys()`     | 获取Section下所有Key |

**示例：**

```csharp
IniConfigHelper.IniPath =
    "config.ini";

IniConfigHelper.WriteIniData
(
    "Database",
    "Server",
    "127.0.0.1"
);

string server =
    IniConfigHelper.ReadIniData
    (
        "Database",
        "Server",
        ""
    );
```

---

### 8. `TCPCommand` 类 (`ModBusTCP.Command`)

构建 Modbus TCP 协议请求报文（MBAP + PDU）。

| 方法                      | 说明      |
| ----------------------- | ------- |
| `ReadBuildMbapHeader()` | 构建MBAP头 |
| `BuildMbap01H()`        | 读取寄存器   |
| `BuildMbap05H()`        | 写单个线圈   |
| `BuildMbap06H()`        | 写单个寄存器  |
| `BuildMbap0FH()`        | 写多个线圈   |
| `BuildMbap10H()`        | 写多个寄存器  |

**示例：**

```csharp
bool[] coils =
{
    true,
    false,
    true
};

byte[] frame =
    TCPCommand.BuildMbap15H
    (
        1,
        1,
        0,
        coils
    );
```

### 9. `LogHelper` 类 (`Helper`) （注意：使用这个类时要与 FileHelper` 类配合。LogHelper` 类调用了 FileHelper中的方法）

提供简单的日志记录功能，采用单例模式实现，按小时自动创建日志文件，适用于设备通信、MES系统、PLC监控等场景。

#### 特性

* 单例模式，全局唯一实例
* 自动创建日志文件
* 按小时分割日志
* 支持成功日志和错误日志
* 自动追加写入

#### 日志文件格式

```text
log/
├── 2026-06-24 08.txt
├── 2026-06-24 09.txt
└── 2026-06-24 10.txt
```

#### 方法

| 方法                                 | 说明     |
| ---------------------------------- | ------ |
| `CreateLogFile()`                  | 创建日志文件 |
| `WriteLog(string log, int status)` | 写入日志   |

#### 参数说明

| 参数         | 说明   |
| ---------- | ---- |
| `log`      | 日志内容 |
| `status=1` | 成功日志 |
| `status=0` | 错误日志 |

#### 日志格式

成功日志：

```text
2026-06-24 10:30:01 Success PLC连接成功
```

错误日志：

```text
2026-06-24 10:31:15 Error PLC连接失败
```

#### 示例

```csharp
// 获取日志实例
LogHelper logger = LogHelper.Instance;

// 写入成功日志
logger.WriteLog(
    "PLC连接成功",
    1
);

// 写入错误日志
logger.WriteLog(
    "PLC连接失败",
    0
);
```

#### MES项目示例

```csharp
try
{
    bool result = plc.Connect();

    if (result)
    {
        LogHelper.Instance.WriteLog(
            "PLC连接成功",
            1
        );
    }
}
catch (Exception ex)
{
    LogHelper.Instance.WriteLog(
        ex.Message,
        0
    );
}
```



---

## 🚀 快速开始

### 1. Modbus RTU读取寄存器

```csharp
byte slave = 1;

ushort start = 0;

ushort count = 10;

byte[] request =
    CrcHelper.GenerateMessage
    (
        slave,
        0x03,
        start,
        count
    );
```

### 2. Modbus TCP读取寄存器

```csharp
TcpClient client =
    new TcpClient
    (
        "192.168.1.100",
        502
    );

IModbusMaster master =
    ModbusTcpMaster.CreateIp(client);

ushort[] registers =
    master.ReadHoldingRegisters
    (
        0,
        10
    );

client.Close();
```

### 3. CSV导出

```csharp
List<Student> students =
    new List<Student>();

CsvHelper.ListToCsv
(
    students,
    "students.csv",
    true
);
```

### 4. INI配置读取

```csharp
string ip =
    IniConfigHelper.ReadIniData
    (
        "PLC",
        "IP",
        "127.0.0.1"
    );
```

---

## 📌 注意事项

* CRC 校验中使用 MessageBox，在服务程序中可能不适用。
* HexByteConvert 默认采用小端字节序。
* DBHelper 建议从配置文件读取数据库连接字符串。
* BinaryFormatter 已被微软废弃，不建议用于新项目。
* IniConfigHelper 基于 Windows API，仅适用于 Windows 平台。

---

## 📄 许可证

本项目仅供学习与内部使用，请勿用于商业用途。

---

## 👨‍💻 作者

**YangZhan** (2002YZ)

- GitHub: [@2002YZ](https://github.com/2002YZ)
- Email: 2532384161@qq.com

---

## 🎉 致谢

感谢使用本项目集合！如果有帮助，请给个 ⭐ Star！

---

**最后更新**: 2026-06-24  
**版本**: 3.0
