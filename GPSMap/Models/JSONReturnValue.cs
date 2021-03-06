﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPSMap.Models
{
    public class JSONReturnValue
    {
        public string Message { get; set; }
        public bool Status { get; set; }

        public UserModel Users { get; set; }
    }

    public class FileContentModel
    {
        public string Content { get; set; }
        public string[] Statements { get; set; }
    }
}