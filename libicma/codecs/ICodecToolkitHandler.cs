using libicma.codecs;

namespace libicma.events
{
    /// <summary>
    /// 编码和解码回调
    /// </summary>
    public interface ICodecToolkitHandler
    {

        /// <summary>
        /// 编码开始
        /// </summary>
        /// <param name="id">文件ID</param>
        /// <param name="frameCount">数据块总数量</param>
        void OnOpen(string id, long frameCount);
        /// <summary>
        /// 解码开始
        /// </summary>
        /// <param name="id">文件ID</param>
        /// <param name="header">头</param>
        void OnOpen(string id, Header header);
        /// <summary>
        /// 进度
        /// </summary>
        /// <param name="id">文件id</param>
        /// <param name="progress">进度(0-100)</param>
        void OnProgress(string id, double progress);
        /// <summary>
        /// 数据到达
        /// </summary>
        /// <param name="data">数据</param>
        void OnNext(object data);

        /// <summary>
        /// 出错
        /// </summary>
        /// <param name="id">文件id</param>
        /// <param name="err">错误原因</param>
        void OnError(string id, string err);
        /// <summary>
        /// 已完成
        /// </summary>
        /// <param name="id">文件ID</param>
        /// <param name="output">输出文件地址</param>
        void OnComplete(string id, string output);


    }
}
