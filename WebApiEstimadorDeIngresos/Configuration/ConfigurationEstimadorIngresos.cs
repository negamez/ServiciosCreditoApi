namespace WebApiEstimadorDeIngresos
{
    public static class ConfigurationEstimadorIngresos
    {
        public static  IConfiguration AppsettingsEstimador { get; }
         static ConfigurationEstimadorIngresos()
        {
            AppsettingsEstimador = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.ResourcesEstimador.json")
                .Build();
        }
    }
}
