// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;

namespace consumer
{
    public static class LoginEventConsumer
    {
        [FunctionName("LoginEventConsumer")]
        public static void RunLoginEventConsumer([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            log.LogInformation("In LoginEventConsumer");
            log.LogInformation(eventGridEvent.Data.ToString());
        }

        [FunctionName("LogoutEventConsumer")]
        public static void RunLogoutEventConsumer([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            log.LogInformation("In LogoutEventConsumer");
            log.LogInformation(eventGridEvent.Data.ToString());
        }
    }
}
