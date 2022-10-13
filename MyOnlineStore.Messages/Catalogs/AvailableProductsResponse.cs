using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyOnlineStore.Entities;

namespace MyOnlineStore.Messages.Catalogs
{
    public record AvailableProductsResponse(IReadOnlyCollection<Product> Products);
}
