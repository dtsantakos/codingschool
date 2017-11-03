using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;

namespace MainSite.Data
{
    public class Post
    {
        [Column("PostId")]
        public int Id { get; set; }

        [Column("Title")]
        public string Title { get; set; }

        [Column("Content")]
        public string Content { get; set; }

        [Column("Created")]
        public DateTime Created { get; set; }

    }
}