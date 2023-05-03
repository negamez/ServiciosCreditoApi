namespace WebApiEstimadorDeIngresos.Model
{
    public class ErrorResponse
    {
        public List<ErrorsItem>? Errors { get; set; }
    }
    public class ErrorsItem
    {
        public String? Code { get; set; }
        public String? Title { get; set; }
        public String? Detail { get; set; }
    }
}
