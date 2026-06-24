using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Helper
{
    public class HexByteConvert
    {
        /// <summary>
        /// 将十六进制字符串转换为字节数组
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] HexStringToByteArry(string hex)
        {
            string[] hexValuesSplit = hex.Split(' ');
            byte[] bytes = new byte[hexValuesSplit.Length];
            for (int i = 0; i < hexValuesSplit.Length; i++)
            {
                if (hexValuesSplit[i].Length == 2)
                {
                    bytes[i] = Convert.ToByte(hexValuesSplit[i], 16);
                }
                else if (hexValuesSplit[i].Length == 4 && hexValuesSplit[i].StartsWith("0x"))
                {
                    bytes[i] = Convert.ToByte(hexValuesSplit[i].Substring(2), 16);
                }
                else
                {
                    MessageBox.Show($"输入的十六进制字符串 '{hex}' 不合法。每个字节必须是两位十六进制数，或者以 '0x' 开头且长度为4。");
                }
            }
            return bytes;
        }

        /// <summary>
        /// 将字节数组转换为十六进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ByteArrayToHexString(byte[] bytes)
        {
            string res = "";
            foreach (byte b in bytes)
            {
                res += "0x" + b.ToString("X2") + " ";
            }
            return res;
        }


        /// <summary>
        /// 将字节数组转换为ushort数组，每两个字节转换为一个ushort值
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static ushort[] ByteToUshort(byte[] bytes)
        {
            ushort[] result = new ushort[bytes.Length / 2];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = BitConverter.ToUInt16(bytes, i * 2);
            }
            return result;
        }

        /// <summary>
        /// 将ushort数组转换为字节数组，每个ushort值转换为两个字节
        /// </summary>
        /// <param name="ushorts"></param>
        /// <returns></returns>
        public static byte[] ByteToUshort(ushort[] ushorts)
        {
            byte[] result = new byte[ushorts.Length * 2];
            for (int i = 0; i < ushorts.Length; i++)
            {
                BitConverter.GetBytes(ushorts[i]).CopyTo(result, i * 2);
            }
            return result;
        }


        /// <summary>
        /// 将字符串转换为ushort数组，字符串中的每个数字用空格分隔
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static ushort[] StringToUshort(string str)
        {
            ushort[] result=str
            //.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
            //.Select(x => ushort.Parse(x))
            //.ToArray();
            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => Convert.ToUInt16(x.Replace("0x", ""), 16))
            .ToArray();
            return result;
        }
    }
}
