using libicma.codecs;
using libicma.options;
using System;
using System.IO;
using libicma.events;
using libicma.utils;

namespace icma
{
    /// <summary>
    /// 编码逻辑单元
    /// </summary>
    public class EncodeUnit : CodecHandlerAdapter
    {

        private readonly Media item;
        private readonly Option option;
        private readonly ICodecToolkitHandler handler;
        private readonly CodecTookit toolkit;
        private string outputFile = "";
        private Stream outputStream;
        public EncodeUnit(CodecTookit toolkit, Media item, Option option, ICodecToolkitHandler handler)
        {
            this.toolkit = toolkit;
            this.item = item;
            this.option = option;
            this.handler = handler;
        }
       
        public override void OnOpen(string id, long frameCount)
        {
            CreateStream();
            if (outputStream == null)
            {
                return;
            }
            //写入协议标识
            toolkit.WriteTag(outputStream);
            //写入协议头
            toolkit.WriteHeader(outputStream, item.Name, frameCount);
            handler?.OnOpen(id, frameCount);
        }

        public override void OnProgress(string id, double progress)
        {
            handler?.OnProgress(id, progress);
        }

        public override void OnNext(object data)
        {
            if (data is byte[] bytes)
            {
                //创建数据块
                var frame = toolkit.CreateFrame(option.Password, bytes, 0);
                //写入数据块
                toolkit.WriteFrame(outputStream, frame);
                handler?.OnNext(data);
            }
        }

        public override void OnError(string id, string err)
        {
            outputStream?.Dispose();
            outputStream = null;
            handler?.OnError(id, err);
        }
        
        public override void OnComplete(string id, string output)
        {
            outputStream?.Dispose();
            outputStream = null;
            handler?.OnComplete(id, outputFile);

        }

        private void CreateStream()
        {
            try
            {
                if (outputStream == null)
                {
                    var fileName = string.IsNullOrEmpty(item.Extension) ? item.Name : item.Name.Replace(item.Extension, "");
                    outputFile = Utils.GetOutputString(option.Storage, $"{fileName}{Utils.Extension}", option.RandomFileName);

                    if (File.Exists(outputFile))
                    {
                        File.Delete(outputFile);
                    }

                    outputStream = new FileStream(outputFile, FileMode.Append, FileAccess.Write);
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
