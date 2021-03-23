namespace CSRO.Server.Entities.Entity
{
    public static class EntityHelper
    {
        //public static bool IsNullOrEmptyCollection<T>(this IEnumerable<T> source)
        //{
        //    return source == null || source.Any() == false || source.FirstOrDefault() == null;
        //}

        public static bool ForbidenStatusForDuplicatePojectNames(this AdoProject project)
        {
            var cond = project.Status == Status.Submitted && project.Status == Status.Approved && project.Status == Status.Completed;
            return cond;        
        }
    }
}
