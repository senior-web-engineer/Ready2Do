using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.DataModel
{
    public class UserReferenceApiModel
    {
        public string UserId { get; set; }

        public UserReferenceApiModel()
        {
        }

        public UserReferenceApiModel(string userId)
        {
            UserId = userId;
        }
    }
}
