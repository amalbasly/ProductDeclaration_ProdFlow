using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

namespace ProdFlow.Models.Responses
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Dictionary<string, List<string>> Errors { get; set; }

        public ApiResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public ApiResponse(bool success, string message, ModelStateDictionary modelState)
        {
            Success = success;
            Message = message;
            Errors = new Dictionary<string, List<string>>();

            foreach (var key in modelState.Keys)
            {
                var state = modelState[key];
                if (state.Errors.Count > 0)
                {
                    Errors[key] = new List<string>();
                    foreach (var error in state.Errors)
                    {
                        Errors[key].Add(error.ErrorMessage);
                    }
                }
            }
        }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T Data { get; set; }

        public ApiResponse(bool success, string message, T data) : base(success, message)
        {
            Data = data;
        }
    }
}