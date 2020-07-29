// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.9.2

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Schema;



namespace TourBot.Bots
{
    public class DispatchBot : ActivityHandler
    {
        private readonly IBotService myBotServices;

        public DispatchBot(IBotService botServices)
        {
            myBotServices = botServices;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {

            //use dispatch model to figure our which service

            var recogniserResult = await myBotServices.Dispatch.RecognizeAsync(turnContext, cancellationToken);

            //determine top intent 
            var topIntent = recogniserResult.GetTopScoringIntent();

            //call dispatcher 
            await DispatchtoTopIntentAsync(turnContext, topIntent.intent, recogniserResult, cancellationToken);

            //var replyText = $"Echo: {turnContext.Activity.Text}";
            //await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello to all!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }

        private async Task DispatchtoTopIntentAsync(ITurnContext<IMessageActivity> turnContext, string intent, RecognizerResult recognizerResult, CancellationToken cancellationToken)
        {
            switch (intent)
            {
                case "l_WhichTeam":
                    await ProcessTeamAsync(turnContext, recognizerResult.Properties["luisResult"] as LuisResult, cancellationToken);
                    break;
                case "q_RiderKB":
                    await ProcessRiderAsync(turnContext, recognizerResult.Properties["luisResult"] as LuisResult, cancellationToken);
                    break;
                default:
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Dispatch unrecognised intent: {intent}."), cancellationToken);
                    break;
            }
        }
        
        private async Task ProcessTeamAsync(ITurnContext<IMessageActivity> turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            //call Power BI
            var token = "H4sIAAAAAAAEACWSR66DVgAA7_K3RKLzIFIW9GJ6h93DBtMxxbQod8-Psp_NjObvHxde_QRfP3_--Cy3kNZrvuy-eVvyRDy6fLSjrNwCKtJ0UqhnRZ8e39QK2jyct0F0PTganpW0ZzgeMWWNiSMug2RNYObZg6xJhVWlD1E651wxSKztxZhOsfzh4HKuvA6ZcOGUGHvaoyA1GIwsdLcVzPHI3u9AxeVb0ggtIr5JhGttKdVNYxtfT0SLLiKMjBQM4ZuGxvruBFjQcTmeMzWwbQJ8fkw4o_DX-JI3PsWRbdEIERaOohUoaQg2Rb4jeo6ukWqSqIG3QT5UhrILRqTvXY993YmVRTd11jz9PcUatk5PhchRo6wtZbJR2lVfHoDw6bGQs2kb7jBsNLVOozZLtEU-kqyC9NTGG19z1zvncRdydxnUw03r28NeoO0fLOEw-OGdR6yY8wu9cD6t44Uce9fNRJPBs9UzitanR8Hjc7GhgoICWS596Jd71IXDBXbTsFbf7ggy6HzZXN6D6aQ1rsYaexitpvpESZj7PnGPRdeRgvimU6HrhhQyG-AzgJ6g3ic7xTr86oBzEjLiVpOq4iKnofAIlYbCv8YrbztTP7Yu1z_6Jzc_3FGLqi1k33MVD3lHCIIKGfkVwBKnEgHvq1fX-FjgGme-Cl8XmTuQnco34W6CIkbwFT7mOHBYu-Cv9_bCb22854ankXv13f0FDPZ7jdjHbY8DHU7B--uvnz9-xOX6bNOjvH433Rkv34mwfxDsnKrRLo-Z8GXSMKoAz5HZxE0ZZ6E3O5vHUFQPyVifyG7GBrsD9r7XhqmR843xvmRZlOgyRFrSDwKtHbOt6gQrQJJ70ru3dY8GczMtRKD2OvcryGf6gcvSPg0fvlLuCvW5Vo6xEKz6lrPqkRAuaPJpWhD43Z9SORcXOgYO7yjgCR63nn50HsvIoikOeiW9NtduztJbZO36qBacoRRnaRupjnKeXrTC_JThGsq9yvubs7uAHCDludvJPnriTLpvtc8Us2JX5TF5kK6kFN18W87KLNvX0e-y6ADSHThr_BDyoGXSUk7GJC0A1rqdATIVk9Wr8k_cksDU6eP_zNenLhc9_q0sa-HBYrXPDFj4colQHAVk-58KmvcIt-9S_mJGoZmkU5FFkBc8DFUC_YZHxVqdAdNDQFRohpf0NA71HRTPTcEjLbwndO-yaK4M7And3vaU2-KDhdfCk8aGrBsmtAl8DdcKHrsbCUaKEiyHbCjZW0Nif6YNSl5Qb8f2Yv9d4MBGLKyaqMyy57t7vpRAzjq96N58cCx3D5Rht3-PKcUY27kvdJ9TjMg2alrXgPR7M20FZwNF2uTTSwkFJsSF7ip6JEiIUNQHYVEKXxYtYhy_h2WQdzKDhRqhe6oDdMKpPYgMSBa0gQOOyTCrhxvIh5dN2VNzJVNI0roCF1E9Y2dHvxq6ZJW2haskWFWK8LH6Wmmcf6yMaKxedgB6lzUv-i_zP_8CPDqOxZoFAAA=.eyJjbHVzdGVyVXJsIjoiaHR0cHM6Ly9ERi1NU0lULVNDVVMtcmVkaXJlY3QuYW5hbHlzaXMud2luZG93cy5uZXQiLCJlbWJlZEZlYXR1cmVzIjp7Im1vZGVybkVtYmVkIjpmYWxzZX19";
            var PBurl = "https://app.powerbi.com/qnaEmbed?groupId=5832e4e8-3075-4f7a-b967-c5604652a29a";

            HttpWebRequest PBrequest = WebRequest.Create(PBurl) as HttpWebRequest;
            PBrequest.KeepAlive = true;
            PBrequest.Method = "POST";
            PBrequest.ContentLength = 0;
            PBrequest.ContentType = "application/json";

            PBrequest.Headers.Add("Authorization", String.Format("Bearer {0}", token));

            var PBresponse = (HttpWebResponse)PBrequest.GetResponse();

            var encoding = ASCIIEncoding.ASCII;

            var reader = new StreamReader(PBresponse.GetResponseStream(), encoding);
            
            string PBreply = reader.ReadToEnd();

  
            if (PBreply != null && PBreply.Length >0)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(PBreply), cancellationToken);
                //var attachment = new Attachment();
                //attachment.ContentType = "text/html";
                //attachment.Content = PBreply;
                //await turnContext.SendActivityAsync(MessageFactory.Attachment(attachment), cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("No anwers found from PB"), cancellationToken);
            }
        }

  
        private async Task ProcessRiderAsync(ITurnContext<IMessageActivity> turnContext, LuisResult luisResult, CancellationToken cancellationToken)
        {
            //call QnA Maker 
            var options = new QnAMakerOptions { Top = 1 };
            var response = await myBotServices.SampleQnA.GetAnswersAsync(turnContext, options);

            if (response != null && response.Length >0)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(response[0].Answer), cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("No anwers found from KB"), cancellationToken);
            }

        }
    }
}
