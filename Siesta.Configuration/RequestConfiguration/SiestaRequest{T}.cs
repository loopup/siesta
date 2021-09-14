namespace Siesta.Configuration.RequestConfiguration
{
    /// <summary>
    /// Base class for a request with the same return type as resource.
    /// </summary>
    /// <typeparam name="T">The resource and return type of the request.</typeparam>
    public class SiestaRequest<T> : SiestaRequest<T, T>
    {
        /// <inheritdoc />
        public override T ExtractResourceFromReturn(T returnObject)
        {
            return returnObject;
        }
    }
}
