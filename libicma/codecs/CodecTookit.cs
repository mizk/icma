
using Google.Protobuf;
using libicma.events;
using libicma.options;
using libicma.utils;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace libicma.codecs
{
    /// <summary>
    /// 编码和解码相关方法
    /// </summary>
    public class CodecTookit
    {
        private const int BufferSize = 1408576;//缓冲区大小:1mb
        public const string Tag = "mizk.chen";//协议标识
        public const string Version = "1.0";//协议版本
        private static readonly object mutex = new object();

        private static CodecTookit instance;
        /// <summary>
        /// 创建解析器对象
        /// </summary>
        /// <returns></returns>
        public static CodecTookit GetInstance()
        {
            if (instance == null)
            {
                lock (mutex)
                {
                    if (instance == null)
                    {
                        instance = new CodecTookit();
                    }
                }

            }
            return instance;
        }
        /// <summary>
        /// 创建帧数据
        /// </summary>
        /// <param name="password">密码</param>
        /// <param name="data">数据</param>
        /// <param name="order">编号</param>
        /// <returns></returns>
        public Frame CreateFrame(string password, byte[] data, int order)
        {
            try
            {
                var key = GenerateKey(password);
                var ivBytes = Encoding.UTF8.GetBytes(Utils.RandomString(16));
                var iv = ByteString.CopyFrom(ivBytes);
                var encryptedBytes = AES.EncryptByteArray(key, ivBytes, data);
                var content = ByteString.CopyFrom(encryptedBytes);
                var frame = new Frame { Data = content, Order = order, Secret = iv };
                return frame;
            }
            catch (Exception)
            {

            }
            return null;

        }
        /// <summary>
        /// 解密帧数据
        /// </summary>
        /// <param name="password">密码</param>
        /// <param name="frame">帧数据</param>
        /// <returns></returns>
        public byte[] DecodeFrame(string password, Frame frame)
        {
            try
            {

                var k = GenerateKey(password);
                var i = frame.Secret.ToByteArray();
                var d = frame.Data.ToByteArray();
                var encryptedBytes = AES.DecryptByteArray(k, i, d);
                return encryptedBytes;
            }
            catch (Exception)
            {

            }
            return null;

        }
        /// <summary>
        /// 生成密钥
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private byte[] GenerateKey(string password)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] key;
            if (passwordBytes.Length > 32)
            {
                key = new byte[32];
                Buffer.BlockCopy(passwordBytes, 0, key, 0, key.Length);
            }
            else if (passwordBytes.Length < 32)
            {
                key = new byte[32];
                Buffer.BlockCopy(passwordBytes, 0, key, 0, passwordBytes.Length);
            }
            else
            {
                key = passwordBytes;
            }
            return key;
        }
        /// <summary>
        /// 写入协议标识
        /// </summary>
        /// <param name="stream">流</param>
        /// <returns></returns>
        public bool WriteTag(Stream stream)
        {
            try
            {
                var bytes = Encoding.UTF8.GetBytes(Tag);
                return WriteBytes(stream, bytes);
            }
            catch (Exception)
            {

            }
            return false;


        }
        /// <summary>
        /// 写入协议头
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="fileName">文件名</param>
        /// <param name="frameCount">帧数量</param>
        /// <returns></returns>
        public bool WriteHeader(Stream stream, string fileName, long frameCount)
        {
            try
            {
                var header = new Header
                {
                    Version = Version,
                    FrameCount = frameCount,
                    Name = fileName
                };
                using var memory = new MemoryStream();
                header.WriteTo(memory);
                var bytes = memory.ToArray();
                return WriteBytes(stream, bytes);
            }
            catch (Exception)
            {
            }
            return false;

        }
        /// <summary>
        /// 写入数据帧
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="frame">数据帧</param>
        /// <returns></returns>
        public bool WriteFrame(Stream stream, Frame frame)
        {
            try
            {
                if (frame == null || stream == null)
                {
                    return false;
                }
                using var memory = new MemoryStream();
                frame.WriteTo(memory);
                var bytes = memory.ToArray();
                return WriteBytes(stream, bytes);
            }
            catch (Exception)
            {
            }
            return false;

        }

        private bool WriteBytes(Stream stream, byte[] bytes)
        {
            try
            {
                if (stream == null || bytes == null || bytes.Length == 0)
                {
                    return false;
                }
                long length = bytes.Length;
                var lengthBytes = BitConverter.GetBytes(length);
                stream.Write(lengthBytes, 0, lengthBytes.Length);
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
                return true;
            }
            catch (Exception)
            {

            }
            return false;


        }
        /// <summary>
        /// 检查给定的文件是否协议编码后的文件
        /// </summary>
        /// <param name="file">文件地址</param>
        /// <returns></returns>
        public bool CheckFile(string file)
        {

            bool ok = false;
            try
            {
                using var stream = new FileStream(file, FileMode.Open);
                var tag = ReadTag(stream);
                ok = tag == Tag;
            }
            catch (Exception)
            {

            }
            return ok;
        }
        /// <summary>
        ///读取协议标识
        /// </summary>
        /// <param name="stream">流</param>
        /// <returns></returns>
        public string ReadTag(Stream stream)
        {
            try
            {
                byte[] buffer = ReadBytes(stream);
                if (buffer == null)
                {
                    return string.Empty;
                }
                return Encoding.UTF8.GetString(buffer);
            }
            catch (Exception)
            {

            }
            return string.Empty;

        }
        /// <summary>
        /// 读取协议头
        /// </summary>
        /// <param name="stream">流</param>
        /// <returns></returns>
        public Header ReadHeader(Stream stream)
        {
            var bytes = ReadBytes(stream);
            if (bytes == null)
            {
                return null;
            }
            try
            {
                Header header = Header.Parser.ParseFrom(bytes);
                return header;
            }
            catch (Exception)
            {

            }
            return null;
        }
        /// <summary>
        /// 读取帧数据
        /// </summary>
        /// <param name="stream">流</param>
        /// <returns></returns>
        public Frame ReadFrame(Stream stream)
        {
            try
            {
                var bytes = ReadBytes(stream);
                return bytes == null ? null : Frame.Parser.ParseFrom(bytes);
            }
            catch (Exception)
            {

            }
            return null;
        }
        /// <summary>
        /// 读取长度
        /// </summary>
        /// <param name="stream">流</param>
        /// <returns></returns>
        private long ReadLength(Stream stream)
        {

            try
            {
                byte[] buffer = new byte[8];
                int c = stream.Read(buffer, 0, buffer.Length);
                if (c != 8)
                {
                    return -1;
                }
                long value = BitConverter.ToInt64(buffer);
                return value;
            }
            catch (Exception)
            {

            }
            return -1;

        }
        /// <summary>
        /// 读取字节
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private byte[] ReadBytes(Stream stream)
        {

            try
            {
                long length = ReadLength(stream);
                if (length <= 0 || length > int.MaxValue)
                {
                    return null;
                }
                byte[] buffer = new byte[length];
                int c = stream.Read(buffer, 0, (int)length);
                if (c == length)
                {
                    return buffer;
                }
                else
                {
                    var subBuffer = new byte[c];
                    Buffer.BlockCopy(buffer, 0, subBuffer, 0, c);
                    return subBuffer;
                }
            }
            catch (Exception)
            {

            }
            return null;
        }
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="id">文件id</param>
        /// <param name="file">文件</param>
        /// <param name="handler">回调接口</param>
        public void ReadFile(string id, string file, ICodecToolkitHandler handler)
        {

            try
            {
                if (!File.Exists(file))
                {
                    handler?.OnError(id, "ERR:1");
                    return;
                }

                var info = new FileInfo(file);
                if (info.Length <= 0)
                {
                    handler?.OnError(id, "ERR:2");
                    return;
                }
                //总字节数
                long maxBytes = info.Length;
                //已读取字节数
                long readBytes = 0;
                //缓冲区
                var byteArray = new byte[BufferSize];
                //数据块总数量
                long frameCount = GetFrameCount(maxBytes);
                using var stream = new FileStream(file, FileMode.Open, FileAccess.Read);
                handler?.OnOpen(id, frameCount);
                do
                {
                    //读取数据到缓冲区
                    int length = stream.Read(byteArray, 0, byteArray.Length);
                    //读完了
                    if (length <= 0)
                    {
                        break;
                    }
                    readBytes += length;
                    var buffer = byteArray;
                    //读取了部分数据
                    if (length != byteArray.Length)
                    {
                        buffer = new byte[length];
                        //从缓冲区复制已读取的数据
                        Buffer.BlockCopy(byteArray, 0, buffer, 0, length);
                    }
                    //计算进度
                    var p = Progress(readBytes, maxBytes);
                    //进度
                    handler?.OnProgress(id, p);
                    //数据
                    handler?.OnNext(buffer);
                    //当前线程休眠100ms,防止进度条刷新过快
                    Thread.Sleep(100);
                } while (true);
                //全部数据读完了
                if (readBytes == maxBytes)
                {
                    handler?.OnComplete(id, "");
                }
                else//读取了部分数据
                {
                    handler?.OnError(id, "ERR:3");
                }
            }
            catch (Exception)
            {
                handler?.OnError(id, "ERR:500");
            }
        }
        /// <summary>
        /// 解码文件
        /// </summary>
        /// <param name="id">文件ID</param>
        /// <param name="file">文件地址</param>
        /// <param name="option">选项</param>
        /// <param name="handler">回调</param>
        public void DecodeFile(string id, string file, Option option, ICodecToolkitHandler handler)
        {

            try
            {
                if (!File.Exists(file))
                {
                    handler?.OnError(id, "ERR:1");
                    return;
                }

                var info = new FileInfo(file);
                if (info.Length <= 0)
                {
                    handler?.OnError(id, "ERR:2");
                    return;
                }

                using var stream = new FileStream(file, FileMode.Open, FileAccess.Read);
                string tag = ReadTag(stream);
                //协议标识不匹配
                if (string.IsNullOrEmpty(tag) || tag != Tag)
                {
                    handler?.OnError(id, "ERR:3");
                    return;
                }
                var header = ReadHeader(stream);
                //协议头不正确或协议版本不对
                if (header == null || header.Version != Version)
                {
                    handler?.OnError(id, "ERR:4");
                    return;
                }
                var output = Utils.GetOutputString(option.Storage, header.Name, false);
                //输入和输出目录不能相同
                if (output.ToLower() == file.ToLower())
                {
                    handler?.OnError(id, "ERR:5");
                    return;
                }
                long frameIndex = 0;
                //数据块总数量
                long frameCount = header.FrameCount;
                handler?.OnOpen(id, header);
                do
                {
                    //读取数据块
                    var frame = ReadFrame(stream);
                    if (frame == null)
                    {
                        break;
                    }
                    frameIndex++;

                    //计算进度
                    var p = Progress(frameIndex, frameCount);
                    //进度
                    handler?.OnProgress(id, p);
                    //数据
                    handler?.OnNext(frame);
                    //当前线程休眠100ms,防止进度条刷新过快
                    Thread.Sleep(100);
                } while (true);
                //全部数据读完了
                if (frameCount == frameIndex)
                {
                    handler?.OnComplete(id, "");
                }
                else//读取了部分数据
                {
                    handler?.OnError(id, "ERR:6");
                }
            }
            catch (Exception)
            {
                handler?.OnError(id, "ERR:500");
            }
        }
        /// <summary>
        /// 计算数据块总数量
        /// </summary>
        /// <param name="length">数据总字节数</param>
        /// <returns></returns>
        private long GetFrameCount(long length)
        {
            var left = length % BufferSize;
            var page = (length - left) / BufferSize;
            return left > 0 ? page + 1 : page;
        }
        /// <summary>
        /// 进度(0~100)
        /// </summary>
        /// <param name="bytes">当前量</param>
        /// <param name="max">总量</param>
        /// <returns></returns>
        private double Progress(long bytes, long max)
        {
            if (max <= 0)
            {
                return 0;
            }
            double p = (double)(bytes * 100) / max;
            if (p > 100.0)
            {
                p = 100.0;
            }
            if (p < 0.0)
            {
                p = 0.0;
            }
            return p;
        }
    }
}
