namespace WebApiModeva.Model
{
    public class RequestModeva
    {
        public string? Query { get; set; }
        public Variables? Variables { get; set; }
    }
    public class Variables
    {
        public int? IdCliente { get; set; }
        public String? producto { get; set; }
    }
}
