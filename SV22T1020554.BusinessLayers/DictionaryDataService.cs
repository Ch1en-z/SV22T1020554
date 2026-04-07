
using SV22T1020554.DataLayers.Interfaces;
using SV22T1020554.DataLayers.SQLServer;
using SV22T1020554.Models.DataDictionary;
using System.Threading.Tasks;

namespace SV22T1020554.BusinessLayers
{
    /// <summary>
    /// Cung c?p các ch?c nãng x? l? d? li?u liên quan ð?n t? ði?n d? li?u
    /// </summary>
    public static class DictionaryDataService
    {
        private static readonly IDataDictionaryRepository<Province> provinceDB;

        /// <summary>
        /// Ctor
        /// </summary>
        static DictionaryDataService()
        {
            provinceDB = new ProvinceRepository(Configuration.ConnectionString);
        }
        /// <summary>
        /// L?y danh sách t?nh thành
        /// </summary>
        /// <returns></returns>
        public static async Task<List<Province>> ListProvincesAsync()
        {
            return await provinceDB.ListAsync();
        }
    }
}
