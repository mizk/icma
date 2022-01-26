using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
namespace libicma.utils
{
    /// <summary>
    /// 辅助函数
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// 文件扩展名
        /// </summary>
        public const string Extension = ".jqc";
      


        /// <summary>
        /// 随机文件名
        /// </summary>
        /// <param name="hasExtension">是否添加扩展名</param>
        /// <returns></returns>
        public static string RandomFileName(bool hasExtension)
        {
            var name = Guid.NewGuid().ToString().Replace("-", "").ToLower();
            return hasExtension ? $"{name}{Extension}" : name;
        }
        /// <summary>
        /// 随机字符串
        /// </summary>
        /// <param name="size">字符串个数</param>
        /// <returns></returns>
        public static string RandomString(int size)
        {
            Random r = new Random();
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                int pos = r.Next(3);
                int isize;
                char begin;
                if (pos == 0)
                {
                    begin = '0';
                    isize = 10;
                }
                else
                {
                    begin = (pos == 1) ? 'a' : 'A';
                    isize = 26;
                }


                char value = (char)(begin + r.Next(isize));
                builder.Append(value);
            }
            return builder.ToString();

        }
        /// <summary>
        /// 输出目录
        /// </summary>
        /// <param name="output"></param>
        /// <param name="fileName"></param>
        /// <param name="randomFileName"></param>
        /// <returns></returns>
        public static string GetOutputString(string output,string fileName, bool randomFileName)
        {
            var name = fileName;
            if (randomFileName)
            {
                name = RandomFileName(true);
            }
            return Path.Combine(output, name);
        }
        
        /// <summary>
        /// 计算hash特征码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Hash(string str)
        {
            var hasher = MD5.Create();
            var hex = hasher.ComputeHash(Encoding.UTF8.GetBytes(str));
            var builder = new StringBuilder();
            for (var index = 0; index < hex.Length; index++)
            {
                builder.Append(hex[index].ToString("x2"));
            }
            return builder.ToString().ToLower();
        }
    }
}
