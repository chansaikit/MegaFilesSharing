﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

/**
 
 * 
 * //------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MegaFilesSharing.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web;
    
    public partial class File
    {
        public File()
        {
            this.Comments = new HashSet<Comment>();
            this.Likes = new HashSet<Like>();
        }
    
        public int FileID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Link")]
        public string Url { get; set; }
        [Display(Name = "File Type")]
        public string FileType { get; set; }
        public string Source { get; set; }
        public string Publisher { get; set; }
        [DataType(DataType.Date)]
        public DateTime PublishDate { get; set; }
        public Nullable<int> UserID { get; set; }
        public int NumOfView { get; set; }
        public int NumOfDownload { get; set; }
        public Nullable<double> FileSize { get; set; }
        public bool Porn { get; set; }
    
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Like> Likes { get; set; }


        //Custom Method 
        public string getURLName(){
            string URLName = Name.Replace(":", "");
            URLName = URLName.Replace("&", "_");
            URLName = URLName.Replace(" ", "_");
            return HttpContext.Current.Server.HtmlEncode(URLName);
        }
    }
}

 
 
 */