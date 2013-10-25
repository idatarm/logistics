using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Logistics.Models
{
    /// <summary>
    /// Class mô tả về các thông tin về các tin tức
    /// </summary>
    public class News
    {
        // ID automately updated
        [Required]
        public int NewsID { get; set; }

        // Title of News
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        // Description of News (short content)
        [Required(ErrorMessage = "Description is required")]
        [MaxLength]
        public string Description { get; set; }

        // Content of News
        [Required(ErrorMessage = "Content is required")]
        [MaxLength]
        public string Content { get; set; }

        // Link of News
        public string Link { get; set; }
        
        // Datetime of News
        public string DateTime { get; set; }

        // Source of News : Get from where ...
        public string Source { get; set; }

        // Category include: News of company (true) or of other (false)
        public bool Category { get; set; }
        
        // Path of image
        public string ImageUrl { get; set; }

        // Language of News: English (true) and Vietnamese (false-default)
        public bool Lang { get; set; }
    }
}