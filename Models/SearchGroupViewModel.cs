using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FruityNET.Models
{
    public class SearchGroupViewModel
    {
        [Required]
        public string Groupname { get; set; }

        public List<GroupResultViewModel> Groups { get; set; }

        public SearchGroupViewModel()
        {
            Groups = new List<GroupResultViewModel>();
        }
    }


    public class GroupResultViewModel
    {
        public Guid Id { get; set; }
        public string Groupname { get; set; }
        public Guid GroupId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}