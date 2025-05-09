﻿public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public List<string> Errors { get; set; } = new List<string>();

    public static ApiResponse<T> SuccessResponse(T data, string message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message ?? "Operation completed successfully"
        };
    }

    public static ApiResponse<T> ErrorResponse(string errorMessage, List<string> errors = null, T data = default)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = errorMessage,
            Errors = errors ?? new List<string>(),
            Data = data
        };
    }
}

public class ApiResponse : ApiResponse<object>
{
    public static new ApiResponse SuccessResponse(object data = null, string message = null)
    {
        return new ApiResponse
        {
            Success = true,
            Data = data,
            Message = message ?? "Operation completed successfully"
        };
    }

    public static new ApiResponse ErrorResponse(string errorMessage, List<string> errors = null, object data = null)
    {
        return new ApiResponse
        {
            Success = false,
            Message = errorMessage,
            Errors = errors ?? new List<string>(),
            Data = data
        };
    }
}