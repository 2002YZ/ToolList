using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using NModbus.Device;

namespace ModBusTCP.Command
{
    public class TCPCommand
    {

        /// <summary>
        /// 构建一个读取通用的MBAP头部
        /// </summary>
        /// <param name="transId">事务标识</param>
        /// <param name="unitId">从站地址</param>
        /// <param name="pduLength">PDU长度</param>
        /// <returns></returns>
        public static byte[] ReadBuildMbapHeader(ushort transId, ushort unitId)
        {
            List<byte> bytes = new List<byte>();

            // MBAP头部
            bytes.Add((byte)(transId >> 8));       // 事务标识高字节
            bytes.Add((byte)(transId & 0xFF));     // 事务标识低字节
            bytes.Add(0x00);                       // 协议标识高字节
            bytes.Add(0x00);                       // 协议标识低字节
            bytes.Add(0x00);                       // 数据长度高字节
            bytes.Add(0x06);                       // 数据长度低字节
            bytes.Add((byte)unitId);                // 从站地址
            return bytes.ToArray();
        }

        /// <summary>
        /// 构建一个读取通用的Modbus TCP协议的请求报文。功能码01H-04H，读取线圈、离散输入、保持寄存器、输入寄存器
        /// </summary>
        /// <param name="transId">事务标识</param>
        /// <param name="unitId">从站地址</param>
        /// <param name="startAddress">起始地址</param>
        /// <param name="quantity">读取数量</param>
        /// <param name="functionCode">功能码</param>
        /// <returns></returns>
        public static byte[] BuildMbap01H(ushort transId, ushort unitId, byte functionCode, byte startAddress, ushort quantity)
        {
            List<byte> bytes = new List<byte>();
            // MBAP头部
            bytes.AddRange(ReadBuildMbapHeader(transId, unitId));

            // PDU部分
            bytes.Add(functionCode);                 // 功能码
            bytes.Add((byte)(startAddress >> 8));   // 起始地址高字节
            bytes.Add((byte)(startAddress & 0xFF));  // 起始地址低字节
            bytes.Add((byte)(quantity >> 8));       // 读取数量高字节
            bytes.Add((byte)(quantity & 0xFF));     // 读取数量低字节
            return bytes.ToArray();
        }



        /// <summary>
        /// 构建写入单个寄存器的请求报文。功能码06H，写入单个保持寄存器
        /// </summary>
        /// <param name="transId"></param>
        /// <param name="unitId"></param>
        /// <param name="registerAddress"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] BuildMbap06H(ushort transId, ushort unitId, byte registerAddress, ushort value)
        {

            List<byte> bytes = new List<byte>();
            // MBAP头部
            bytes.AddRange(ReadBuildMbapHeader(transId, unitId));

            // PDU部分
            bytes.Add(0x06);                       // 功能码
            bytes.Add((byte)(registerAddress >> 8));   // 寄存器地址高字节
            bytes.Add((byte)(registerAddress & 0xFF));  // 寄存器地址低字节
            bytes.Add((byte)(value >> 8));          // 写入值高字节
            bytes.Add((byte)(value & 0xFF));        // 写入值低字节
            return bytes.ToArray();

        }

        /// <summary>
        /// 构建写入单个线圈的请求报文。功能码05H，写入单个线圈
        /// </summary>
        /// <param name="transId"></param>
        /// <param name="unitId"></param>
        /// <param name="coilAddress"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] BuildMbap05H(ushort transId, ushort unitId, byte coilAddress, bool value)
        {
            List<byte> bytes = new List<byte>();
            // MBAP头部
            bytes.AddRange(ReadBuildMbapHeader(transId, unitId));

            // PDU部分
            bytes.Add(0x05);                       // 功能码
            bytes.Add((byte)(coilAddress >> 8));   // 线圈地址高字节
            bytes.Add((byte)(coilAddress & 0xFF)); // 线圈地址低字节
            bytes.Add((byte)(value ? 0xFF : 0x00)); // 写入值高字节
            bytes.Add(0x00);                         // 写入值低字节
            return bytes.ToArray();

        }


        /// <summary>
        /// 构建写入多个寄存器的请求报文。功能码16H，写入多个保持寄存器
        /// </summary>
        /// <param name="transId"></param>
        /// <param name="unitId"></param>
        /// <param name="startAddress"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static byte[] BuildMbap16H(ushort transId, ushort unitId, byte startAddress, ushort[] values)
        {
            List<byte> bytes = new List<byte>();
            ushort pdulen = 6;
            // MBAP头部
            bytes.AddRange(ReadBuildMbapHeader(transId, unitId));

            // PDU部分
            bytes.Add(0x10);                       // 功能码
            bytes.Add((byte)(startAddress >> 8));   // 起始地址高字节
            bytes.Add((byte)(startAddress & 0xFF));  // 起始地址低字节
            bytes.Add((byte)(values.Length >> 8));  // 写入数量高字节
            bytes.Add((byte)(values.Length & 0xFF)); // 写入数量低字节
            bytes.Add((byte)(values.Length * 2));   // 字节数
            foreach (var value in values)
            {
                bytes.Add((byte)(value >> 8));      // 写入值高字节
                bytes.Add((byte)(value & 0xFF));    // 写入值低字节
            }


            // 更新MBAP头部中的数据长度
            pdulen = (ushort)(bytes.Count - 7 + 1); // 数据长度 = 总长度 - MBAP头部长度(7字节)+1从站地址
            bytes[4] = (byte)(pdulen >> 8);     // 数据长度高字节
            bytes[5] = (byte)(pdulen & 0xFF);   // 数据长度低字节
            return bytes.ToArray();
        }


        /// <summary>
        /// 构建写入多个线圈的请求报文。功能码15H，写入多个线圈
        /// </summary>
        /// <param name="transId"></param>
        /// <param name="unitId"></param>
        /// <param name="startAddress"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static byte[] BuildMbap15H(ushort transId, ushort unitId, byte startAddress, bool[] values)
        {
            List<byte> bytes = new List<byte>();
            ushort pdulen = 6;
            // MBAP头部
            bytes.AddRange(ReadBuildMbapHeader(transId, unitId));
            // PDU部分
            bytes.Add(0x0F);                       // 功能码
            bytes.Add((byte)(startAddress >> 8));   // 起始地址高字节
            bytes.Add((byte)(startAddress & 0xFF));  // 起始地址低字节
            bytes.Add((byte)(values.Length >> 8));  // 写入数量高字节
            bytes.Add((byte)(values.Length & 0xFF)); // 写入数量低字节
            byte byteCount = (byte)((values.Length + 7) / 8); // 字节数，向上取整
            bytes.Add(byteCount);

            int dataStartIndex = bytes.Count; // 当前末尾索引（即字节数之后的索引）
            // 预留数据空间，根据线圈数量计算需要的字节数，并初始化为0，防止后续设置位时出现问题
            for (int i = 0; i < byteCount; i++)
            {
                bytes.Add(0x00); // 初始化数据字节为0
            }
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i])
                {
                    bytes[dataStartIndex + (i / 8)] |= (byte)(1 << (i % 8)); // 设置对应位
                }
            }
            // 更新MBAP头部中的数据长度
            pdulen = (ushort)(bytes.Count - 7 + 1); // 数据长度 = 总长度 - MBAP头部长度(7字节)+1从站地址
            bytes[4] = (byte)(pdulen >> 8);     // 数据长度高字节
            bytes[5] = (byte)(pdulen & 0xFF);   // 数据长度低字节
            return bytes.ToArray();
        }
}}
