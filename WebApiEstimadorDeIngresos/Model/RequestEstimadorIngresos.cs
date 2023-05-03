namespace WebApiEstimadorDeIngresos.Model
{
    public class RequestEstimadorIngresos
    {
        public string? Query { get; set; }
        public Variables? Variables { get; set; }
    }
    public class Variables
    {
        public int? IdCliente { get; set; }
    }
}
