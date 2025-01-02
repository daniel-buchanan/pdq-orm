namespace decaf.Implementation.Execute
{
    public class SelectColumnBuilder : ISelectColumnBuilder
    {
        /// <inheritdoc/>
        public object Is(string name) => null;

        /// <inheritdoc/>
        public object Is(string name, string alias) => null;

        /// <inheritdoc/>
        public T Is<T>(string name) => default(T);

        /// <inheritdoc/>
        public T Is<T>(string name, string alias) => default(T);
    }
}