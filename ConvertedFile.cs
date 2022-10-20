using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HTML_to_PDF_converter
{
    public class ConvertedFile
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public ConvertedFile(string id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}
