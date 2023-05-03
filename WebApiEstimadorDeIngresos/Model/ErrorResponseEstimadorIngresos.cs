namespace WebApiEstimadorDeIngresos.Model
{
    public class ErrorResponseEstimadorIngresos
    {
        public List<ErrorItems>? Errors { get; set; }
    }

    public class ErrorItems
    {
        public String? Error { get; set; }
        public String? Message { get; set; }
        public String? Code { get; set; }
    }
}
