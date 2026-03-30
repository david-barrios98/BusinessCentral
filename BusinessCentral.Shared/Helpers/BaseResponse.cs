using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BusinessCentral.Shared.Helper;

public class BaseResponse<T>
{
    public bool IsSuccess { get; set; }
    public T Data { get; set; } = default;
    public string Message { get; set; }
    /// <summary>
    /// Marcar como true si el backend lanzó una excepción.
    /// </summary>
    public bool IsException { get; set; } = false;
    /// <summary>
    /// Mensaje de la excepción
    /// </summary>
    public string ExceptionMessage { get; set; }

    [JsonIgnore]
    public int StatusCode { get; set; } = default;

    [JsonIgnore]
    public bool IsDataNullOrEmpty
    {
        get
        {
            // Verifica si Data es null
            if (Data == null) return true;

            // Verifica si Data es una colección y está vacía
            if (Data is ICollection collection)
            {
                return collection.Count == 0;
            }

            return false;
        }
    }
}

public static class RepositoryReturn
{
    public static BaseResponse<T> Success<T>(T data, string message = null, int? statusCode = 200) => new BaseResponse<T>()
    {
        IsSuccess = true,
        Data = data,
        Message = message,
        IsException = false,
        StatusCode = (int)statusCode
    };

    public static BaseResponse<Paged<T>> Success<T>(List<T> data, int count, int offset, int pageSize, string message = null, int? statusCode = 200) => new BaseResponse<Paged<T>>()
    {
        IsSuccess = true,
        Data = new Paged<T> { Records = data, CurrentPageNumber = offset, PageSize = pageSize, TotalRecords = count },
        Message = message,
        IsException = false,
        StatusCode = (int)statusCode
    };

    public static BaseResponse<List<T>> SuccessList<T>(List<T> data, string message = null, int? statusCode = 200) => new BaseResponse<List<T>>()
    {
        IsSuccess = true,
        Data = data,
        Message = message,
        IsException = false,
        StatusCode = (int)statusCode
    };



    public static BaseResponse<T> Error<T>(string message, int? statusCode = null) => new BaseResponse<T>()
    {
        IsSuccess = false,
        Message = message,
        IsException = false,
        StatusCode = (int)statusCode
    };

    public static BaseResponse<T> ErrorException<T>(Exception ex, string contextMessage = "Excepción no controlada")
    {
        return new BaseResponse<T>
        {
            IsSuccess = false,
            IsException = true,
            Message = $"{contextMessage}: {ex.Message}",
            StatusCode = 500
        };
    }



}
