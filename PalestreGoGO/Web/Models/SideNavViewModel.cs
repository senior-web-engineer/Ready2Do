using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class SideNavViewModel
    {
        public bool IsVisible { get; set; }

        public string CurrentController { get; set; }
        public string CurrentAction { get; set; }

        public string ActiveSectionName
        {
            get
            {
                return CurrentController;
            }
        }

        public int OpenedSectionIndex
        {
            get
            {
                switch (CurrentController)
                {
                    case "profilocliente":
                        return 0;
                    default:
                        return -1;
                }
            }
        }
    }
}
