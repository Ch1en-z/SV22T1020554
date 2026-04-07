namespace SV22T1020554.DataLayers.Interfaces
{
    /// <summary>
    /// Ð?nh ngh?a các phép x? l? d? li?u s? d?ng cho t? ði?n d? li?u
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataDictionaryRepository<T> where T : class
    {
        /// <summary>
        /// L?y danh sách d? li?u
        /// </summary>
        /// <returns></returns>
        Task<List<T>> ListAsync();
    }
}
