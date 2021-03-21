

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wa_asp_mvc_file_operation.Data.Models
{
    public class CloudUser
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Login { get; set; }

        [Required]
        public string Passwords { get; set; }


        [Required]
        public string Name { get; set; }

        [Required]
        public string FolderName { get; set; }

        public bool IsGoodInfo() => (WS.IsGoodString(Login) && WS.IsGoodString(Passwords) && WS.IsGoodString(Name));
        public void SetPersonalFolder() 
        {
            if(IsGoodInfo()) 
            {
                FolderName = $"{Login}-{Name}-{Guid.NewGuid()}";
            }
        }

    }
}