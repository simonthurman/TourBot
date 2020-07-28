using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Configuration;
using Microsoft.Extensions.Configuration;






namespace TourBot
{
    public class BotService : IBotService
    {
        public BotService(IConfiguration configuration)
        {
            var luisApplication = new LuisApplication(
                configuration["LuisAppId"],
                configuration["LuisAPIKey"],
                $"https://{configuration["LuisAPIHostName"]}.api.cognitive.microsoft.com");

            //var luisApplication = new LuisApplication("c977157a-9296-454f-bc8b-10d9fea979e3", "0c053144d5c74dc4bf12bc61dcb544e1", "https://westus.api.cognitive.microsoft.com");

            var recognizerOptions = new LuisRecognizerOptionsV2(luisApplication)
            
            {
                IncludeAPIResults = true,
                PredictionOptions = new LuisPredictionOptions()
                {
                    IncludeAllIntents = true,
                    IncludeInstanceData = true
                }
            };

            //Dispatch = new LuisRecognizer(luisApplication);
            Dispatch = new LuisRecognizer(recognizerOptions);
            
            SampleQnA = new QnAMaker(new QnAMakerEndpoint
            {
                KnowledgeBaseId = configuration["QnAKnowledgebaseId"],
                EndpointKey = configuration["QnAEndpointKey"],
                Host = configuration["QnAEndpointHostName"]
            });
        }

        public LuisRecognizer Dispatch { get; private set; }
        public QnAMaker SampleQnA { get; private set; }
    }
}
