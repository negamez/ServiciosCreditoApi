namespace WebApiModeva.Model
{
    public class ResponseMaestroModeva
    {
        public DataMaestroModeva? Data { get; set; }
    }

    public class DataMaestroModeva
    {
        public MaestroModevaAll? MaestroModevaAll { get; set; }
    }

    public class MaestroModevaAll
    {
        public int? TotalCount { get; set; }
        public List<MaestromodevaItem>? Maestromodeva { get; set; }
    }

    public class MaestromodevaItem
    {
        public String? IngestionYear { get; set; }
        public String? IngestionMonth { get; set; }
        public String? IngestionDay { get; set; }
        public String? IdCliente { get; set; }
        public String? ModevaTipo { get; set; }
        public String? Producto { get; set; }
        public String? GOriginacionFinal { get; set; }
        public String? Activo { get; set; }
        public String? Migrar { get; set; }
    }

}
