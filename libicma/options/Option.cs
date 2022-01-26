namespace libicma.options
{
    /// <summary>
    /// 选项
    /// </summary>
    public class Option : ICloneable
    {
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = "";
        /// <summary>
        /// 存储目录
        /// </summary>
        public string Storage { get; set; } = "";

       
        /// <summary>
        /// 随机名
        /// </summary>
        public bool RandomFileName { get; set; }
      
        /// <summary>
        /// 克隆
        /// </summary>
        /// <param name="source"></param>
        public void Clone(object value)
        {
            if (value is Option option)
            {
                Password = option.Password;
                Storage = option.Storage;
                RandomFileName = option.RandomFileName;
                
            }
        }
    }
}
