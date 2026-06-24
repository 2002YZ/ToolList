using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Helper
{
    public class CrcHelper
    {
        /// <summary>
        /// 计算校验码的函数，使用CRC16算法
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static ushort Crc16(byte[] data, int length)
        {
            ushort crc = 0xFFFF;
            for (int i = 0; i < length; i++)
            {
                crc ^= data[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x0001) == 0x0001)
                    {
                        crc >>= 1;
                        crc ^= 0xA001;
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
            }
            return crc;
        }

        /// <summary>
        /// 验证接收数据的函数和长度是否匹配，输入参数为接收的数据字节数组和数据长度
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static bool VerifyCrc(byte[] data, int length)
        {
            byte byteCount = (byte)(length - 2); // 数据部分的长度
            // 数据长度必须至少包含CRC
            if (length < 2)
            {
                MessageBox.Show("数据长度不足，无法进行CRC校验");
                return false;
            }
            else if (byteCount < 0)
            {
                MessageBox.Show("数据长度不正确，无法进行CRC校验");
                return false;
            }


            // 比较接收的CRC和计算的CRC
            ushort receivedCrc = (ushort)((data[length - 1] << 8) | data[length - 2]); // 提取接收的CRC
            ushort calculatedCrc = Crc16(data, length - 2); // 计算数据的CRC
            if (receivedCrc == calculatedCrc)
            {
                MessageBox.Show("CRC校验成功，数据完整");
                return true;
            }
            else
            {
                MessageBox.Show($"CRC校验失败，数据可能被篡改或损坏。接收的CRC: {receivedCrc:X4}, 计算的CRC: {calculatedCrc:X4}");
                return false;
            }


            
        }




        /// <summary>
        /// 生成读取报文的函数，输入参数包括从站地址、功能码、寄存器地址和寄存器个数，输出生成的报文字节数组
        /// </summary>
        /// <param name="slaveAddress"></param>
        /// <param name="functionCode"></param>
        /// <param name="registerAddress"></param>
        /// <param name="registerCount"></param>
        /// <returns></returns>
        public static byte[] GenerateMessage(byte slaveAddress, byte functionCode, ushort registerAddress, ushort registerCount)
        {
            byte[] message = new byte[8];
            message[0] = slaveAddress;//从站地址
            message[1] = functionCode;//功能码
            message[2] = (byte)(registerAddress >> 8);//寄存器地址高位
            message[3] = (byte)(registerAddress & 0xFF);//寄存器地址低位
            message[4] = (byte)(registerCount >> 8);//寄存器个数高位
            message[5] = (byte)(registerCount & 0xFF);//寄存器个数低位 
            ushort crc = Crc16(message, 6);
            message[6] = (byte)(crc & 0xFF);//校验码低位
            message[7] = (byte)(crc >> 8);//校验码高位
            return message;
        }



        /// <summary>
        /// 06,生成写入单个寄存器报文的函数，输入参数包括从站地址、功能码、寄存器地址，输出生成的报文字节数组
        /// </summary>
        /// <param name="slaveAddress"></param>
        /// <param name="functionCode"></param>
        /// <param name="registerAddress"></param>
        /// <param name="registerValues"></param>
        /// <returns></returns>
        public static byte[] GenerateWrite(byte slaveAddress, byte functionCode, ushort registerAddress, ushort registerValues)
        {
            byte[] message = new byte[8];
            message[0] = slaveAddress;                     //从站地址
            message[1] = functionCode;                     //功能码
            message[2] = (byte)(registerAddress >> 8);    //寄存器地址高位
            message[3] = (byte)(registerAddress & 0xFF);  //寄存器地址低位
            message[4] = (byte)(registerValues >> 8);     //寄存器值高位
            message[5] = (byte)(registerValues & 0xFF);   //寄存器值低位
            ushort crc = Crc16(message, 6);
            message[6]= (byte)(crc & 0xFF);                //校验码低位
            message[7]= (byte)(crc >> 8);                   //校验码高位
            return message;

        }

        /// <summary>
        /// 生成写入寄存器报文的函数（功能码0x10：写多个寄存器）
        /// </summary>
        /// <param name="slaveAddress">从站地址</param>
        /// <param name="functionCode">功能码</param>
        /// <param name="registerAddress">寄存器地址</param>
        /// <param name="registerValues">寄存器值数组</param>
        /// <returns>生成的报文字节数组</returns>
        public static byte[] GenerateWriteMessage(byte slaveAddress, byte functionCode, ushort registerAddress, ushort[] registerValues)
        {
            byte registerCount = (byte)registerValues.Length;  // 寄存器个数
            byte byteCount = (byte)(registerCount * 2);  // 每个寄存器占2个字节

            // 报文结构: 地址1 + 功能码1 + 寄存器地址2 + 数量2 + 字节数1 + 数据 + CRC2
            byte[] message = new byte[9 + byteCount];
            //byte[] message = new byte[8];
            message[0] = slaveAddress;                      // 从站地址
            message[1] = functionCode;                              // 功能码（写多个寄存器）
            message[2] = (byte)(registerAddress >> 8);      // 寄存器地址高位
            message[3] = (byte)(registerAddress & 0xFF);    // 寄存器地址低位
            message[4] = (byte)(registerCount >> 8);        // 寄存器个数高位
            message[5] = (byte)(registerCount & 0xFF);      // 寄存器个数低位
            message[6] = byteCount;                         // 字节数

            // 填充数据（大端格式）
            for (int i = 0; i < registerValues.Length; i++)
            {
                message[7 + i * 2] = (byte)(registerValues[i] >> 8);        // 高位
                message[7 + i * 2 + 1] = (byte)(registerValues[i] & 0xFF);  // 低位
            }

            // 计算并填充CRC
            ushort crc = Crc16(message, message.Length - 2);
            message[message.Length - 2] = (byte)(crc & 0xFF);       // CRC低位
            message[message.Length - 1] = (byte)(crc >> 8);         // CRC高位

            return message;
        }



        /// <summary>
        /// 生成写入线圈报文的函数（功能码0x05：写单个线圈）
        /// </summary>
        /// <param name="slaveAddress">从站地址</param>
        /// <param name="functionCode">功能码</param>
        /// <param name="coilAddress">线圈地址</param>
        /// <param name="coilValue">线圈值</param>
        /// <returns></returns>
        public static byte[] GenerateWriteCoilMessage(byte slaveAddress, byte functionCode, ushort coilAddress, bool coilValue)
        {
            byte[] message = new byte[8];
            message[0] = slaveAddress; // 从站地址
            message[1] = functionCode; // 功能码（写单个线圈）
            message[2] = (byte)(coilAddress >> 8); // 线圈地址高位
            message[3] = (byte)(coilAddress & 0xFF); // 线圈地址低位
            message[4] = coilValue ? (byte)0xFF : (byte)0x00; // 线圈值（0xFF表示ON，0x00表示OFF）
            message[5] = 0x00; // 保留字节，通常为0
            ushort crc = Crc16(message, 6);
            message[6] = (byte)(crc & 0xFF); // CRC低位
            message[7] = (byte)(crc >> 8); // CRC高位
            return message;
        }

        /// <summary>
        /// 生成写入多个线圈报文的函数（功能码0x0F：写多个线圈）
        /// </summary>
        /// <param name="slaveAddress">从站地址</param>
        /// <param name="functionCode">功能码</param>
        /// <param name="coilAddress">线圈地址</param>
        /// <param name="coilValues">线圈值数组</param>
        /// <returns></returns>
        public static byte[] GenerateWriteMultipleCoilsMessage(byte slaveAddress, byte functionCode, ushort coilAddress, bool[] coilValues)
        {
            byte coilCount = (byte)coilValues.Length; // 线圈个数
            byte byteCount = (byte)((coilCount + 7) / 8); // 字节数（每8个线圈占1字节）
            // 报文结构: 地址1 + 功能码1 + 线圈地址2 + 数量2 + 字节数1 + 数据 + CRC2
            byte[] message = new byte[9 + byteCount];
            message[0] = slaveAddress; // 从站地址
            message[1] = functionCode; // 功能码（写多个线圈）
            message[2] = (byte)(coilAddress >> 8); // 线圈地址高位
            message[3] = (byte)(coilAddress & 0xFF); // 线圈地址低位
            message[4] = (byte)(coilCount >> 8); // 线圈个数高位
            message[5] = (byte)(coilCount & 0xFF); // 线圈个数低位
            message[6] = byteCount; // 字节数
            // 填充数据（每8个线圈打包成1字节）
            for (int i = 0; i < coilValues.Length; i++)
            {
                if (coilValues[i])
                {
                    message[7 + i / 8] |= (byte)(1 << (i % 8)); // 设置对应位为1
                }
            }
            // 计算并填充CRC
            ushort crc = Crc16(message, message.Length - 2);
            message[message.Length - 2] = (byte)(crc & 0xFF); // CRC低位
            message[message.Length - 1] = (byte)(crc >> 8); // CRC高位
            return message;
        }


    }
}
