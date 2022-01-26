
using libicma.codecs;
using libicma.events;
using libicma.options;
using libicma.utils;
using System;
using System.IO;

namespace icma
{
    /// <summary>
    /// 解码逻辑单元
    /// </summary>
    public class DecodeUnit : CodecHandlerAdapter
    {
       
        private readonly Option option;
        private readonly ICodecToolkitHandler handler;
        private readonly CodecTookit toolkit;
        private string outputFile = "";
        private Stream outputStream;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="toolkit"></param>
        /// <param name="item"></param>
        /// <param name="option"></param>
        /// <param name="handler"></param>
        public DecodeUnit(CodecTookit toolkit,  Option option, ICodecToolkitHandler handler)
        {
            this.toolkit = toolkit;
            this.option = option;
            this.handler = handler;
        }
        public override void OnOpen(string id, Header header)
        {
            CreateStream(header.Name);
            handler?.OnOpen(id, header);
        }
        public override void OnProgress(string id, double progress)
        {
            handler.OnProgress(id, progress);
        }
        public override void OnNext(object data)
        {

            try
            {
                if (data is Frame frame)
                {
                    var bytes = toolkit.DecodeFrame(option.Password, frame);
                    outputStream?.Write(bytes);
                }
            }
            catch (Exception)
            {
            }
            handler?.OnNext(data);
        }

        public override void OnComplete(string id, string output)
        {
            outputStream?.Dispose();
            outputStream = null;
            handler?.OnComplete(id, outputFile);
        }

        public override void OnError(string id, string err)
        {
            outputStream?.Dispose();
            outputStream = null;
            handler?.OnError(id, err);

        }
        
        private void CreateStream(string fileName)
        {

            try
            {
                if (outputStream == null)
                {
                    outputFile = Utils.GetOutputString(option.Storage, fileName, false);
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
