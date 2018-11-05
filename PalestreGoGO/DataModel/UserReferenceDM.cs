using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PalestreGoGo.DataModel
{
    public class UserReferenceDM
    {
        [MaxLength(100)]
        public string UserId { get; set; }

        public UserReferenceDM()
        {                
        }

        public UserReferenceDM(string userId)
        {
            this.UserId = userId;
        }
    }
}
