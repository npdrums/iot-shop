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
                400 => "You've made a bad request!",
                401 => "You're not authorized!",
                403 => "Access to this resource is forbiden!",
                404 => "Sorry! The resource you're looking for is not found!",
                500 => "Oops! Looks like we have an Internal Server Error!",
                _ => null
            };
        }
    }
}