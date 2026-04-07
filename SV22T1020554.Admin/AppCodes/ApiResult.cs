namespace SV22T1020554.Admin
{
    /// <summary>
    /// Bi?u di?n d? li?u tr? v? c?a các API
    /// </summary>
    public class ApiResult
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        public ApiResult(int code, string message = "")
        {
            Code = code;
            Message = message;
        }

        /// <summary>
        /// M? k?t qu? tr? v? (quy ý?c 0 t?c là l?i ho?c không thành công)
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Thông báo l?i (n?u có)
        /// </summary>
        public string Message { get; set; } = "";
    }
}
