using System;
using System.ComponentModel.DataAnnotations;

namespace VideoGameAPI.Models
{
    public class PublisherModel
    {
        [Key]
        public int PublisherId { get; set; }
        public string PublisherName { get; set; }
    }
}
