using MyOnlineStore.Billing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyOnlineStore.Billing.Actors.State
{
    public class CartActorState
    {
        public List<CartItem> Items { get; set; }

        public CartActorState()
        {
            Items = new List<CartItem>();
        }
    }
}
