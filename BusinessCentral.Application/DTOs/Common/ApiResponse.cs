using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCentral.Application.DTOs.Common
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public int Code { get; set; }
        public bool IsException { get; set; }

        public static ApiResponse<T> Success(T data, string message, int code = 200)
            => new() { IsSuccess = true, Data = data, Message = message, Code = code };

        public static ApiResponse<T> Failure(string message, int code)
            => new() { IsSuccess = false, Message = message, Code = code };

        public static ApiResponse<T> Exception(string message, int code = 500)
            => new() { IsSuccess = false, IsException = true, Message = message, Code = code };
    }
}
