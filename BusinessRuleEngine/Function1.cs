using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Reflection;

namespace BusinessRuleEngine
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string paramVal = "";
            //Execute fundtions

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            var bb = data.name;
            foreach(var items in bb)
            {
                var j = items.Value;
                CallServices(j);
            }
            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }

        private static void CallServices(string j)
        {
            using (StreamReader r = new StreamReader("./ProductReturn_schema.json"))
            {
                string json = r.ReadToEnd();
                dynamic items = JsonConvert.DeserializeObject(json);
                foreach(var item in items.Items)
                {
                    if(item.Name==j)
                    {
                        foreach(string methods in item.Value)
                        {
                            Type calledType = Type.GetType(methods);
                            String s = (String)calledType.InvokeMember(
                                            j,
                                            BindingFlags.InvokeMethod | BindingFlags.Public |
                                                BindingFlags.Static,
                                            null,
                                            null,
                                            new Object[] { "" });
                        }
                    }
                }
            }
        }        
    }
}
