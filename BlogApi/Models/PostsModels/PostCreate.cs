using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.Models.PostsModels
{
    public class PostCreate
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string CreatedDate { get; set; } = DateTime.Now.ToShortDateString();     
        public string UpdatedDate { get; set; } = DateTime.Now.ToShortDateString();
        public string[] tags { get; set; }
    }
}
