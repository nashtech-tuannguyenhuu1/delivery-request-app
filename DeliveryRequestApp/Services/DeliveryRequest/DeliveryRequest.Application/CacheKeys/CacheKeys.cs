using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryRequest.Application.CacheKeys;
public static class CacheKeys
{
    public static string GetRequestKey(Guid id) => $"request-{id}";
}
