namespace ModularBackend.Api.Extensions
{
    public static class HttpContextExtensions
    {
        public static async Task InsertMetadataHeaderasync<T>(this HttpContext httpContext, IQueryable<T> values)
        {
            if(httpContext is null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }
        }
    }
}
