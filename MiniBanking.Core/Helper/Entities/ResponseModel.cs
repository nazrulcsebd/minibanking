using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MiniBanking.Core.Helper.Entities
{
    public class ResponseModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Object Notification { get; set; }
        public Object Data { get; set; }
        public ArrayList DataList { get; set; }
    }
}
