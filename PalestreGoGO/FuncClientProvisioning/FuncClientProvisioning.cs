using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace FuncClientProvisioning
{
    public static class FuncClientProvisioning
    {
        [FunctionName("FuncClientProvisioning")]
        public static void Run([QueueTrigger("clients-to-provision", Connection = "QueueClientsToProvision")]string myQueueItem, TraceWriter log)
        {
            log.Info($"C# Queue trigger function processed: {myQueueItem}");
            //Invocazione API provisioning 
            //Potenziale problema di security
        }
    }
}
