namespace libicma.events
{
    /// <summary>
    /// UI操作分类
    /// </summary>
    public enum Action
    {
        /// <summary>
        /// 执行
        /// </summary>
        Execute,
        /// <summary>
        /// 删除
        /// </summary>
        Delete,
        /// <summary>
        /// 打开所在目录
        /// </summary>
        Open
    }
    /// <summary>
    /// UI操作委托
    /// </summary>
    /// <param name="sender">触发者</param>
    /// <param name="action">事件</param>
    /// <param name="state">携带参数</param>
    public delegate void ActionListener(object sender, Action action, object state);
    /// <summary>
    /// 操作触发器
    /// </summary>
    public class ActionInvoker
    {
        /// <summary>
        /// 事件
        /// </summary>
        public event ActionListener Event;

        private static ActionInvoker instance;

        private static readonly object mutex = new object();
        private ActionInvoker()
        {

        }
        public static ActionInvoker Instance()
        {
            if (instance == null)
            {
                lock (mutex)
                {
                    if (instance == null)
                    {
                        instance = new ActionInvoker();
                    }
                }

            }
            return instance;
        }
        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="sender">触发者</param>
        /// <param name="action">事件</param>
        /// <param name="state">携带参数</param>
        public void Invoke(object sender, Action action, object state)
        {
            Event?.Invoke(sender, action, state);
        }
    }
}
