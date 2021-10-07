namespace API.Errors
{
    public class ApiErrorResponse
    {
        public ApiErrorResponse(int statusCode, string message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }

        public int StatusCode { get; set; }
        public string Message { get; set; }

        private string GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "Bad Request.",
                401 => "Only authorized users can use this resource!",
                403 => "Forbiden!",
                404 => "Not Found!",
                500 => "Internal Server Error!",
                _ => null
            };
        }
    }
}