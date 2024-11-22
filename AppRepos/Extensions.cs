using System.Linq.Expressions;

namespace AppRepos
{
    public static class Extensions
    {
        public static List<T> SortBy<T>(this List<T> list, Expression<Func<T, object>> fieldSelector, bool ascending = true)
        {
            var keySelector = fieldSelector.Compile();

            if (ascending)
                list.Sort((x, y) => Comparer<object>.Default.Compare(keySelector(x), keySelector(y)));
            else
                list.Sort((x, y) => Comparer<object>.Default.Compare(keySelector(y), keySelector(x)));
            return list;
        }


        public static async Task<List<T>> GetPagedAsync<T>(this List<T> list, int pageNumber, int pageSize)
        {
            try
            {
                if (pageNumber < 1 || pageSize < 1)
                    throw new ArgumentException("Page number and page size must be greater than zero.");

                int skipCount = (pageNumber - 1) * pageSize;

                List<T> result = list.Skip(skipCount).Take(pageSize).ToList();

                await Console.Out.WriteLineAsync($"\t<Retrieved {result.Count} results in page number: {pageNumber}>");

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"Failed. Error: {ex.Message}");
                throw;
            }
        }

        public static List<T> SearchBy<T>(this List<T> list, Expression<Func<T, object>>? fieldSelector, string? searchKey)
        {
            if (string.IsNullOrEmpty(searchKey) || fieldSelector == null) return list;

            var compiledSelector = fieldSelector.Compile();

            return list.Where(item =>
            {
                var fieldValue = compiledSelector(item)?.ToString();
                return fieldValue != null && fieldValue.IndexOf(searchKey, StringComparison.OrdinalIgnoreCase) >= 0;
            }).ToList();
        }

        public static List<T> SearchByOr<T>(this List<T> list, Expression<Func<T, object>>? fieldSelector, string? searchKey)
        {
            if (string.IsNullOrEmpty(searchKey) || fieldSelector == null) return list;

            var compiledSelector = fieldSelector.Compile();

            var filteredList = list.Where(item =>
            {
                var fieldValue = compiledSelector(item)?.ToString();
                return fieldValue != null && fieldValue.IndexOf(searchKey, StringComparison.OrdinalIgnoreCase) >= 0;
            }).ToList();

            return list.Concat(filteredList)
                       .Distinct()
                       .ToList();
        }
    }
}
