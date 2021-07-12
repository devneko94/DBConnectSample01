using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DBConnectSample01.Models
{
    public class MemberModel
    {
        public string MemberID { get; set; }

        public string MemberName { get; set; }

        public string MemberAddress { get; set; }

        public MemberModel Clone()
        {
            return (MemberModel)MemberwiseClone();
        }
    }
}
