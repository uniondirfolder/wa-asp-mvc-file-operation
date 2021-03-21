

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
        private string _passwords;

        public string Passwords
        {
            get { return _passwords; }
            set { _passwords = value; SetPersonalFolder(); }
        }

        [Required]
        public string Name { get; set; }

        [Required]
        public string FolderName { get; set; }

        public bool IsGoodInfo() => (WS.IsGoodString(Login) && WS.IsGoodString(Passwords) && WS.IsGoodString(Name));
        private void SetPersonalFolder() 
        {
            if(IsGoodInfo()) 
            {
                FolderName = $"{Login}-{Name}-{Guid.NewGuid()}";
            }
        }

    }
}