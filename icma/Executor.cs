using libicma.events;
using libicma.utils;
using System.Collections.Concurrent;
using System.Threading;

namespace icma
{
    public enum ExecutorAction
    {
        Encode,
        Decode
    }
    /// <summary>
    /// 任务执行者
    /// </summary>
    public class Executor
    {
        private static  Executor instance;
        private static readonly object mutex=new();
        private readonly ConcurrentDictionary<string, Media> taskList = new();

        public static Executor Instance() { 
            if(instance == null)
            {
                lock (mutex)
                {
                    if(instance == null)
                    {
                        instance = new Executor();
                    }
                }
            }
            return instance;
        }
        private Executor()
        {

        }

       
        /// <summary>
        /// 移除任务
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <returns></returns>
        public bool Remove(string taskId)
        {
            return taskList.TryRemove(taskId, out _);
        }
        /// <summary>
        /// 执行编码或解码任务
        /// </summary>
        /// <param name="item">文件对象</param>
        /// <param name="action">编码或解码</param>
        /// <param name="option">选项</param>
        /// <param name="listener">回调</param>
        public void Execute(Media item, ExecutorAction action, libicma.options.Option option, ICodecToolkitHandler listener)
        {
            if (taskList.TryAdd(item.ID, item))
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    var engine = libicma.codecs.CodecTookit.GetInstance();
                    if (action == ExecutorAction.Encode)
                    {
                        Encode(engine, item, option, listener);
                    }
                    else
                    {
                        Decode(engine, item, option, listener);
                    }
                });
            }

        }
        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="toolkit"></param>
        /// <param name="item"></param>
        /// <param name="option"></param>
        /// <param name="listener"></param>
        private static void Encode(libicma.codecs.CodecTookit toolkit, Media item, libicma.options.Option option, ICodecToolkitHandler listener)
        {
            var output = Utils.GetOutputString(option.Storage, $"{item.Name}{Utils.Extension}", option.RandomFileName);
            
            if (output.ToLower() == item.Input.ToLower())
            {
                listener?.OnError( item.ID, "ERR:-1");
                return;
            }
            //编码逻辑单元
            var unit = new EncodeUnit(toolkit, item, option, listener);
            //分块读取文件执行编码
            toolkit.ReadFile(item.ID, item.Input, unit);
        }
        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="toolkit"></param>
        /// <param name="item"></param>
        /// <param name="option"></param>
        /// <param name="listener"></param>
        private static void Decode(libicma.codecs.CodecTookit toolkit, Media item, libicma.options.Option option, ICodecToolkitHandler listener)
        {
            //解码逻辑单元
            var unit = new DecodeUnit(toolkit,option, listener);
            //解码
            toolkit.DecodeFile(item.ID, item.Input, option, unit);
        }
    }
}
