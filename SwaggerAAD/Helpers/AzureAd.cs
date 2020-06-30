using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SwaggerAAD.Helpers
{
    public class AzureAd
    {
        public string Instance { get; set; }
        public string Domain { get; set; }
        public Guid TenantId { get; set; }
        public Guid ClientId { get; set; }
        public string CallbackPath { get; set; }
    }
}
