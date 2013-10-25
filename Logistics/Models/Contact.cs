using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace Logistics.Models
{
    public class Contact
    {
        // ID automately updated
        [Required]
        public int ContactID { get; set; }

        // Name of user sent contact
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        
        // Email of user sent contact
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
        [Required(ErrorMessage = "The email address is required")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email:")]
        public string Email { get; set; }

        // Subject of a contact
        [Required(ErrorMessage = "Subject is required")]
        public string Subject { get; set; }

        // Content of News
        [Required(ErrorMessage = "Message is required")]
        [MaxLength]
        public string Message { get; set; }

    }
}