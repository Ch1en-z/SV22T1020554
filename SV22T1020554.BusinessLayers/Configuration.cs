namespace SV22T1020554.BusinessLayers
{
    /// <summary>
    /// L?p kh?i t?o và lýu tr? c?u h?nh chung cho t?ng BusinessLayer
    /// </summary>
    public static class Configuration
    {
        /// <summary>
        /// Chu?i k?t n?i ð?n cõ s? d? li?u
        /// </summary>
        public static string ConnectionString { get; private set; } = string.Empty;

        /// <summary>
        /// Hàm kh?i t?o c?u h?nh (ðý?c g?i t? Program.cs c?a t?ng giao di?n)
        /// </summary>
        /// <param name="connectionString">Chu?i k?t n?i CSDL</param>
        public static void Initialize(string connectionString)
        {
            ConnectionString = connectionString;
        }
    }
}
