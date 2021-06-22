using System.Collections.Generic;
using FileUpload_Core.Models;

namespace FileUpload_Core.ViewModels
{
    public class IndexViewModel
    {
        public List<Place> PlaceList { get; set; }
        public List<string> PrefectureList { get; set; }
    }
}