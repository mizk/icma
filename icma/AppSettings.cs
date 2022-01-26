using libicma.options;
using System;
using System.IO;

namespace icma
{
    /// <summary>
    /// 选项
    /// </summary>
    public class AppSettings
    {
        private static AppSettings instance;
        private readonly static object mutex = new();

        private AppSettings()
        {

        }
        public static AppSettings Instance()
        {
            if (instance == null)
            {
                lock (mutex)
                {
                    if (instance == null)
                    {
                        instance = new AppSettings();
                    }
                }

            }
            return instance;
        }
        public Option AppOption { get; } = new();
        public void ResetDefault()
        {
            var storage = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            AppOption.Storage = storage;
         
            AppOption.Password = "";
          
            AppOption.RandomFileName = false;

        }
        public void Load()
        {
            try
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var config = Path.Combine(appData, "icma", "app.config");
                if (!File.Exists(config))
                {
                    ResetDefault();
                    Save();
                    return;
                }
                var optionConfig = OptionSerializer.DeSerialize(config);
                if (optionConfig != null)
                {
                    AppOption.Clone(optionConfig);
                }
            }
            catch (Exception)
            {

            }
            
        }

        public void Save()
        {
            try
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var directory = Path.Combine(appData, "icma");
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var configPath = Path.Combine(directory, "app.config");
                OptionSerializer.Serialize(AppOption, configPath);
            }
            catch (Exception )
            {

            }
            

        }
    }
}
