using System;
using System.Windows.Media.Imaging;

namespace icma
{
    /// <summary>
    /// 图片获取
    /// </summary>
    public class ImageFetch
    {
        private static ImageFetch instance;
        private static readonly object mutex = new();
        public static ImageFetch Instance()
        {
            if (instance == null)
            {
                lock (mutex)
                {
                    if (instance == null)
                    {
                        instance = new ImageFetch();
                    }
                }

            }
            return instance;
        }
        private ImageFetch()
        {

        }
        private readonly string[] images = new string[]
        {
            "aquatic1","aquatic2","beast1","beast2","bird1","bird2","bug1","bug2","dawn1","dawn2","dusk1","dusk2","mech1","mech2","plant1","plant2","reptile1","reptile2"
        };
        /// <summary>
        /// 成功图标
        /// </summary>
        /// <returns></returns>
        public static BitmapImage Success()
        {
            return GetImage("success");
        }
        /// <summary>
        /// 失败图标
        /// </summary>
        /// <returns></returns>
        public static BitmapImage Error()
        {
            return GetImage("error");
        }
        /// <summary>
        /// 获取图标
        /// </summary>
        /// <param name="name">图标名字</param>
        /// <returns></returns>
        public static BitmapImage GetImage(string name)
        {
            return new BitmapImage(new Uri($"pack://application:,,,/Res/{name}.png"));
        }
        /// <summary>
        /// 随机图标
        /// </summary>
        /// <returns></returns>
        public string Random()
        {
            var rnd = new Random();
            var pos = rnd.Next(images.Length);
            return $"Pets/{images[pos]}";
        }
    }
}
