namespace WebApiEstimadorDeIngresos.Model
{
    public class ResponseEstimadorIngresos
    {
        public Data? Data { get; set; }
    }

    public class Data
    {
        public EstimadorIngresosAll? EstimadorIngresosAll { get; set; }
    }

    public class EstimadorIngresosAll
    {
        public int? TotalCount { get; set; }
        public List<EstimadoringresosItem>? Estimadoringresos { get; set; }
    }

    public class EstimadoringresosItem
    {
        public Double? IngestionYear { get; set; }
        public Double? IngestionMonth { get; set; }
        public Double? IngestionDay { get; set; }
        public String? IdCliente { get; set; }
        public String? Clasificacion { get; set; }
        public Double? IngFinal { get; set; }
        public Double? AntiguedadLaboralFinal { get; set; }
        public String? LugarTrabajoFinal { get; set; }
        public String? IngresoSolicitudes { get; set; }
        public String? AntiguedadLaboralSolicitudes { get; set; }
        public String? LugarTrabajoSolicitudes { get; set; }
        public String? FlagSolicitudes { get; set; }
        public Double? IngPlanilla { get; set; }
        public Double? AntiguedadLaboralPlanilla { get; set; }
        public String? LugarTrabajoPlanilla { get; set; }
        public Double? Activo { get; set; }
        public Double? Migrar { get; set; }
    }

}
