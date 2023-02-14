using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Requests.Comments
{
    public class CommentUpdateRequest : CommentAddRequest, IModelIdentifier
    {
        public int Id { get; set; }

    }
}
