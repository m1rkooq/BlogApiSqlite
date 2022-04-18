using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.Models.CommentsModels
{
    public class CommentCreate
    {      
        public string CommentText { get; set; }
        public string CreateTime { get; set; } = DateTime.Now.ToShortDateString();
        public int PostId { get; set; }
    }
}
