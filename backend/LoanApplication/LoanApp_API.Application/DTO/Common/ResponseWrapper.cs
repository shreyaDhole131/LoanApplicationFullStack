using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.DTO.Common
{
    public class ResponseWrapper<T>
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public T? Data { get; set; }
    }
}
