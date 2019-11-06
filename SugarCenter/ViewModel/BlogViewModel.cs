using System.Collections.Generic;
using InstaSharp.Models;
using InstaSharp.Models.Responses;

namespace SugarCenter.ViewModel
{
    public class BlogViewModel
    {
        public UserResponse UserData { get; set; }

        public List<Media> MediaList { get; set; }
    }
}
