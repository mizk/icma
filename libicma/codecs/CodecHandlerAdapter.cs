
using libicma.events;

namespace libicma.codecs

{
    /// <summary>
    /// 协议回调适配器
    /// </summary>
    public class CodecHandlerAdapter : ICodecToolkitHandler
    {
        public virtual void OnComplete(string id,string output)
        {
           
        }

        public virtual void OnError(string id, string err)
        {
           
        }

        public virtual void OnNext(object data)
        {
           
        }

        public virtual void OnOpen(string id, long frameCount)
        {
           
        }

        public virtual void OnOpen(string id, Header header)
        {
          
        }

        public virtual void OnProgress(string id, double progress)
        {
            
        }
    }
}
