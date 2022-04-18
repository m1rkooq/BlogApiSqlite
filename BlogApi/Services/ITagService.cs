using BlogApi.Models.TagsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApiSqlite.Services
{
    public interface ITagService
    {
        IEnumerable<Tags> UpdateTag(Tags model);
        bool DeleteTag(Tags model);
        bool isTrue(int UserId, Tags model);
    }
}
