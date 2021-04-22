using System;
using System.Collections.Generic;
using System.Text;

namespace CSRO.Server.Domain
{
    //public class ResponseMessageDto
    //{
    //    public bool Success { get; set; }
    //    public string Message { get; set; }
    //    public object ReturnedObject { get; set; }
    //}

    public class ResponseMessage <T> where T : class
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T ReturnedObject { get; set; }
    }
}
