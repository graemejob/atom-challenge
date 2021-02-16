using System;
using System.Collections.Generic;
using System.Text;

namespace ImageServiceCore.Models
{
    public class ImageContent
    {
        public byte[] Bytes { get; set; }
        public string ContentType { get; set; }
    }
}
