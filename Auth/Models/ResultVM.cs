using System;
using System.ComponentModel.DataAnnotations;

namespace Formula.SimpleAPI.Auth
{
    public enum Status
    {
        Success = 1,
        Error = 2
    }    

    public class ResultVM
    {
        public Status Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
