namespace WebApiModeva.Model
{
    public class ResponseModeva
    {
        public Data? Data { get; set; }
    }
    public class Data
    {
        public ModevaGruposAll? ModevaGruposAll { get; set; }
    }

    public class ModevaGruposAll
    {
        public int? TotalCount { get; set; }
        public List<ModevagruposItem>? Modevagrupos { get; set; }
    }

    public class ModevagruposItem
    {
        public String? IngestionYear { get; set; }
        public String? IngestionMonth { get; set; }
        public String? IngestionDay { get; set; }
        public String? IdCliente { get; set; }
        public String? GModeva { get; set; }
        public String? Escala { get; set; }
        public String? GFinal { get; set; }
        public String? ModevaTipo { get; set; }
        public String? Activo { get; set; }
        public String? Migrar { get; set; }
    }

}
