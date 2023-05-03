namespace WebApiModeva.Model
{
    public class ErrorResponseModeva
    {
        public IList<ErrorDetail>? Errors { get; set; }
    }

    public class ErrorDetail
    {
        public String? Error { get; set; }

        public String? Message { get; set; }

        public String? Code { get; set; }

    }

}
