namespace WebApiModeva
{
    public static class ConfigurationModeva
    {
        public static IConfiguration AppsettingsModeva { get; }
        static ConfigurationModeva()
        {
            AppsettingsModeva = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.ResourcesModeva.json")
                .Build();
        }
    }
}
