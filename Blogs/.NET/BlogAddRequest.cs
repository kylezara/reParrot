using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Requests.Blogs
{
    public class BlogAddRequest
    {

        [Required]
        [Range(1, 3)]
        public int BlogTypeId { get; set; }

        

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Title { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Subject { get; set; }

        [AllowNull]
        public string Content { get; set; }

        [AllowNull]
        [Url]
        [StringLength(255, MinimumLength = 2)]
        public string ImageUrl { get; set; }

        [Required]
        public bool IsPublished { get; set; }

        [AllowNull]
        [DataType(DataType.DateTime)]
        public DateTime DatePublish { get; set; }

    }
}
