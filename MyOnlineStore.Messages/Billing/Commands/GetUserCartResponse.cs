using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyOnlineStore.Billing.Entities;

namespace MyOnlineStore.Messages.Billing.Commands
{
    public class GetUserCartResponse
    {
        public IReadOnlyCollection<CartItem> Items { get; }

        public GetUserCartResponse(IReadOnlyCollection<CartItem> items)
        {
            Items = items;
        }
    }
}
