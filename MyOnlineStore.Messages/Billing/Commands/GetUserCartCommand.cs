using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyOnlineStore.Messages.Billing.Commands
{
    public class GetUserCartCommand
    {
        public string User { get; }

        public GetUserCartCommand(string user)
        {
            User = user;
        }
    }
}
