using libicma.codecs;
using libicma.utils;
using System.IO;

namespace icma
{
    public enum MediaStatus
    {
        Unknown,
        Ready,
        Processing,
        ProccedSuccess,
        ProccedFailure
    }
    public class Media : Model
    {
        /// <summary>
        /// 是否编码过的文件
        /// </summary>
        public bool IsEncoded { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 扩展名
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public long Length { get; private set; }
        /// <summary>
        /// 状态
        /// </summary>
        public MediaStatus Status { get; set; } = MediaStatus.Unknown;
        /// <summary>
        /// 图标
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// 原文件
        /// </summary>
        public string Input { get; private set; }
        /// <summary>
        /// 输出文件
        /// </summary>
        public string Output { get; set; }
        /// <summary>
        /// 是否已编码或解码
        /// </summary>
        public bool Handled => Status == MediaStatus.ProccedSuccess || Status == MediaStatus.ProccedFailure;
        /// <summary>
        /// 复制或设置元数据
        /// </summary>
        /// <param name="value">元数据</param>
        public override void Clone(object value)
        {
            switch (value)
            {
                case string filePath:
                    FileInfo info = new(filePath);
                    Input = info.FullName;
                    Name = info.Name;
                    Extension = info.Extension;
                    Length = info.Length;
                    IsEncoded = CodecTookit.GetInstance().CheckFile(filePath);
                    id = Utils.Hash(filePath.ToLower());
                    Image = ImageFetch.Instance().Random();
                    Status = MediaStatus.Ready;
                    Output = "";
                    break;
                case Media item:
                    Name = item.Name;
                    Length = item.Length;
                    IsEncoded = item.IsEncoded;
                    Status = item.Status;
                    id = item.ID;
                    Image = item.Image;
                    Output = item.Output;
                    Extension = item.Extension;
                    break;
                default:
                    break;
            }

        }
    }
}
