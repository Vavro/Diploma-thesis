using System;

namespace DragqnLD.WebApi.Configuration
{
    /// <summary>
    /// DragqnLd Server Configuration
    /// </summary>
    public class DragqnLdConfig
    {
        private const string DatabaseUrlSettingsKey = "DragqnLdDatabaseUrl";
        private const string DatabaseUrlDefaultValue = "http://localhost:18296";
        private const string DatabaseNameSettingsKey = "DragqnLdDatabaseName";
        private const string DatabaseNameDefaultValue = "DragqnLd";

        private static DragqnLdConfig _instance;

        private readonly System.Configuration.Configuration _rootWebConfig1;
        private readonly Lazy<string> _databaseUrl;
        private readonly Lazy<string> _databaseName;

        private DragqnLdConfig()
        {
            _rootWebConfig1 = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(null);
            _databaseUrl = new Lazy<string>(() => GetAppSetting(DatabaseUrlSettingsKey, DatabaseUrlDefaultValue));
            _databaseName = new Lazy<string>(() => GetAppSetting(DatabaseNameSettingsKey, DatabaseNameDefaultValue));
        }

        private string GetAppSetting(string settingKey, string defaultValue)
        {
            var setting = _rootWebConfig1.AppSettings.Settings[settingKey];
            if (setting == null)
            {
                return defaultValue;
            }

            return setting.Value;
        }

        /// <summary>
        /// Gets the database URL.
        /// </summary>
        /// <value>
        /// The database URL.
        /// </value>
        public string DatabaseUrl { get { return _databaseUrl.Value; } }

        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        /// <value>
        /// The name of the database.
        /// </value>
        public string DatabaseName { get { return _databaseName.Value; } }

        /// <summary>
        /// Gets the instance of the configuration.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static DragqnLdConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DragqnLdConfig();
                }
                return _instance;
            }
        }
    }
}