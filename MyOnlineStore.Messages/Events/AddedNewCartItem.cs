using MyOnlineStore.Messages.Billing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyOnlineStore.Messages.Events
{
    public class AddedNewCartItem
    {
        public AddProductToCart Message { get; set; }
    }
}
