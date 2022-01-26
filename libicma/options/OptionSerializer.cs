using libicma.codecs;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace libicma.options
{
    public static class OptionSerializer
    {
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static Option DeSerialize(string file)
        {
            Option option = null;
            if (File.Exists(file))
            {
                try
                {
                    var bytes = File.ReadAllBytes(file);
                    byte[] pBytes = Encoding.UTF8.GetBytes(CodecTookit.Tag);
                    var raw=ProtectedData.Unprotect(bytes, pBytes, DataProtectionScope.CurrentUser);
                    var text=Encoding.UTF8.GetString(raw);
                    option = JsonConvert.DeserializeObject<Option>(text);
                }
                catch (Exception )
                {

                }

            }
            return option;
        }
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="option"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool Serialize(Option option, string file)
        {
            try
            {
                var content = JsonConvert.SerializeObject(option);
                var bytes = Encoding.UTF8.GetBytes(content);
                byte[] pBytes = Encoding.UTF8.GetBytes(CodecTookit.Tag);
                var d = ProtectedData.Protect(bytes, pBytes, DataProtectionScope.CurrentUser);
                File.WriteAllBytes(file, d);
                return true;
            }
            catch (Exception )
            {

            }
            return false;
        }
    }
}
