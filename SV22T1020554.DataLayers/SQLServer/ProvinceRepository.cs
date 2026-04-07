using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;
using SV22T1020554.DataLayers.Interfaces;
using SV22T1020554.Models.DataDictionary;

// Thay ð?i "SV22T1020554.Models.Common.Province" thành ðúng namespace ch?a l?p Province c?a b?n n?u c?n

namespace SV22T1020554.DataLayers.SQLServer
{
    /// <summary>
    /// Cài ð?t phép x? l? d? li?u ð? l?y danh sách T?nh/Thành ph?
    /// </summary>
    public class ProvinceRepository : IDataDictionaryRepository<Province>
    {
        private readonly string _connectionString;

        public ProvinceRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Province>> ListAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // Gi? ð?nh b?ng ch?a t?nh thành tên là Provinces và có c?t ProvinceName
                var sql = "select ProvinceName from Provinces order by ProvinceName";
                var data = await connection.QueryAsync<Province>(sql);
                return data.ToList();
            }
        }
    }
}
