using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyOnlineStore.Messages.Billing.Responses;

public class ProductAddedToCartResponse
{
    public DateTime DateTime { get; }

    public ProductAddedToCartResponse(DateTime dateTime)
    {
        DateTime = dateTime;
    }
}