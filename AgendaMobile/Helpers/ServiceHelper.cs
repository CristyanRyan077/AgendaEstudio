using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaMobile.Helpers
{
    public static class ServiceHelper
    {
        public static IServiceProvider Services { get; set; }

        public static T GetService<T>() where T : class
        {
            return Services.GetService(typeof(T)) as T
                   ?? throw new InvalidOperationException($"Serviço {typeof(T)} não encontrado.");
        }
    }
}
