using Newtonsoft.Json;

namespace SV22T1020554.Admin
{
    /// <summary>
    /// L?p cung c?p các ti?n ích liên quan ð?n ng? c?nh c?a ?ng d?ng web
    /// </summary>
    public static class ApplicationContext
    {
        private static IHttpContextAccessor? _httpContextAccessor;
        private static IWebHostEnvironment? _webHostEnvironment;
        private static IConfiguration? _configuration;

        /// <summary>
        /// G?i hàm này trong Program
        /// </summary>
        /// <param name="httpContextAccessor">app.Services.GetRequiredService<IHttpContextAccessor>()</param>
        /// <param name="webHostEnvironment">app.Services.GetRequiredService<IWebHostEnvironment>()</param>
        /// <param name="configuration">app.Configuration</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Configure(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException();
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException();
            _configuration = configuration ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// HttpContext
        /// </summary>
        public static HttpContext? HttpContext => _httpContextAccessor?.HttpContext;

        /// <summary>
        /// WebHostEnvironment
        /// </summary>
        public static IWebHostEnvironment? WebHostEnvironment => _webHostEnvironment;

        /// <summary>
        /// Configuration
        /// </summary>
        public static IConfiguration? Configuration => _configuration;

        /// <summary>
        /// URL c?a website, k?t thúc b?i d?u / (ví d?: https://mywebsite.com/)
        /// </summary>
        public static string BaseURL => $"{HttpContext?.Request.Scheme}://{HttpContext?.Request.Host}/";

        /// <summary>
        /// Ðý?ng d?n v?t l? ð?n thý m?c wwwroot
        /// </summary>
        public static string WWWRootPath => _webHostEnvironment?.WebRootPath ?? string.Empty;

        /// <summary>
        /// Ðý?ng d?n v?t l? ð?n thý m?c g?c lýu ?ng d?ng Web
        /// </summary>
        public static string ApplicationRootPath => _webHostEnvironment?.ContentRootPath ?? string.Empty;

        /// <summary>
        /// Ghi d? li?u vào session
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetSessionData(string key, object value)
        {
            try
            {
                string sValue = JsonConvert.SerializeObject(value);
                if (!string.IsNullOrEmpty(sValue))
                {
                    _httpContextAccessor?.HttpContext?.Session.SetString(key, sValue);
                }
            }
            catch
            {
                // X? l? l?i n?u c?n
            }
        }

        /// <summary>
        /// Ð?c d? li?u t? session
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T? GetSessionData<T>(string key) where T : class
        {
            try
            {
                string sValue = _httpContextAccessor?.HttpContext?.Session.GetString(key) ?? "";
                if (!string.IsNullOrEmpty(sValue))
                {
                    return JsonConvert.DeserializeObject<T>(sValue);
                }
            }
            catch
            {
                // X? l? l?i n?u c?n
            }
            return null;
        }

        /// <summary>
        /// L?y chu?i giá tr? c?a c?u h?nh trong appsettings.json
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetConfigValue(string name)
        {
            return _configuration?[name] ?? "";
        }

        /// <summary>
        /// L?y ð?i tý?ng có ki?u là T trong ph?n c?u h?nh có tên là name trong appsettings.json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetConfigSection<T>(string name) where T : new()
        {
            var value = new T();
            _configuration?.GetSection(name).Bind(value);
            return value;
        }
    }
}
