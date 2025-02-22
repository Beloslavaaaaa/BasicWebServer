﻿using BasicWebServer.Server.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server.HTTP
{
    public class Header
    {
        public const string ContentType = "Content-Type";
        public const string ContentLength = "ContentLength";
        public const string Date = "Date";
        public const string Location  = "Location";
        public const string Server = "Server";
        public Header(string name, string value) {
            Guard.AgainstNull(name,nameof (name));  
            Guard.AgainstNull(value,nameof (value));
            this.Name = name;
            this.Value = value;
        }
        public string Name { get; init; }    
        public string Value { get; set; }
        public override string ToString() => $"{this.Name}: {this.Value}";
        

    }
}
