namespace decaf.Implementation.Execute
{
    public class InsertColumnBuilder : IInsertColumnBuilder
    {
        private InsertColumnBuilder() { }

        public static IInsertColumnBuilder Create()
            => new InsertColumnBuilder();

        public T Is<T>() => default(T);
    }
}

