using System.Collections.Generic;

namespace FileUpload_Core.Models
{
    public class Prefecture
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<Place> Places { get; set; }
    }
}