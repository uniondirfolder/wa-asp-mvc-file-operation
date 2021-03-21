using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wa_asp_mvc_file_operation.Models
{
    public class ServerFile
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public FileType FileType { get; set; }
    }
}