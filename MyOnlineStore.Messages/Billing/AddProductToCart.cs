using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyOnlineStore.Messages.Billing
{
    public record AddProductToCart(string User, string ProductName, int Quantity, decimal Value);
}
