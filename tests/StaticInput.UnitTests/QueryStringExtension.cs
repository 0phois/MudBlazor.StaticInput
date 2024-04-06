namespace StaticInput.UnitTests
{
    public static class QueryStringExtension
    {
        public static string ToQueryString(this Type type)
        {
            return $"?Test={type.Name}";
        }
    }
}
