using Amazon.Lambda.Core;
using Alexa.NET.Response;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET;
using System.Collections.Generic;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace AWSLambda1
{
    public class Function
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public SkillResponse FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            var requestType = input.GetRequestType();

            if(requestType == typeof(LaunchRequest))
            {
                return GiveWelcomeMessage(input, context);
            }
            else if(requestType == typeof(IntentRequest))
            {
                return ProcessIntentRequest(input, context);
            }
            else if(requestType == typeof(SessionEndedRequest))
            {
                return SendSessionEndResponse(input, context);
            }

            return new SkillResponse();
        }

        private SkillResponse GiveWelcomeMessage(SkillRequest input, ILambdaContext context)
        {
            var launchRequest = input.Request as LaunchRequest;

            var welcomeSpeech = new PlainTextOutputSpeech
            {
                Text = ResponseMessages.Welcome
            };

            var reprompt = new Reprompt
            {
                OutputSpeech = new PlainTextOutputSpeech
                {
                    Text = ResponseMessages.WelcomeReprompt
                }
            };

            return SendResponse(welcomeSpeech, reprompt, context);
        }

        private SkillResponse GiveHelloWorldMessage(SkillRequest input, ILambdaContext context)
        {
            var speech = new SsmlOutputSpeech
            {
                Ssml = ResponseMessages.HelloWorld
            };
            
            return SendResponse(speech, context);
        }

        private SkillResponse GiveCancelMessage(SkillRequest input, ILambdaContext context)
        {
            var speech = new SsmlOutputSpeech
            {
                Ssml = ResponseMessages.Cancel
            };

            return GiveWelcomeMessage(input, context);
        }

        private SkillResponse GiveHelpMessage(SkillRequest input, ILambdaContext context)
        {
            var speech = new SsmlOutputSpeech
            {
                Ssml = ResponseMessages.Help
            };

            return SendResponse(speech, context);
        }

        private SkillResponse GiveNiceToMeetYouMessage(SkillRequest input, ILambdaContext context)
        {
            var intentRequest = input.Request as IntentRequest;
            var name = intentRequest.Intent.Slots["Name"].Value;
            
            var speech = new SsmlOutputSpeech
            {
                Ssml = string.Format(ResponseMessages.NiceToMeet, name)
            };

            return SendResponse(speech, context);
        }

        private SkillResponse GiveNoNameMessage(SkillRequest input, ILambdaContext context)
        {
            var speech = new SsmlOutputSpeech
            {
                Ssml = ResponseMessages.NoName
            };

            return SendResponse(speech, context);
        }

        private SkillResponse GiveGoodbyeMessage(SkillRequest input, ILambdaContext context)
        {
            var speech = new SsmlOutputSpeech
            {
                Ssml = ResponseMessages.Goodbye
            };

            return SendResponse(speech, context, endSession: true);
        }

        private SkillResponse ProcessIntentRequest(SkillRequest input, ILambdaContext context)
        {
            var intentRequest = input.Request as IntentRequest;

            var logger = context.Logger;

            logger.Log("ProcessIntentRequest:IntentName " + intentRequest.Intent.Name);

            if(intentRequest.Intent.Name == Intents.HelloWorldIntent)
            {
                return GiveHelloWorldMessage(input, context);
            }
            else if(intentRequest.Intent.Name == Intents.MyNameIntent)
            {
                return GiveNiceToMeetYouMessage(input, context);
            }
            else if(intentRequest.Intent.Name == Intents.NoNameIntent)
            {
                return GiveNoNameMessage(input, context);
            }
            else if(intentRequest.Intent.Name == Intents.CancelIntent)
            {
                return GiveCancelMessage(input, context);
            }
            else if(intentRequest.Intent.Name == Intents.StopIntent)
            {
                return GiveGoodbyeMessage(input, context);
            }
            else if(intentRequest.Intent.Name == Intents.HelpIntent)
            {
                return GiveHelpMessage(input, context);          
            }

            return SendSessionEndResponse(input, context);
        }

        private SkillResponse SendSessionEndResponse(SkillRequest input, ILambdaContext context)
        {
            //var sessionEndedRequest = input.Request as SessionEndedRequest;
            
            return GiveGoodbyeMessage(input, context);
        }

        private SkillResponse SendResponse(IOutputSpeech speech, ILambdaContext context, bool endSession = false)
        {         
            var response = ResponseBuilder.Tell(speech); var attributes = new Dictionary<string, object>();
            attributes.Add("foo", "bar");
            response.Version = "1.0";
            response.SessionAttributes = attributes;
            response.Response.ShouldEndSession = endSession;

            context.Logger.Log(response.ToString());

            return response;
        }

        private SkillResponse SendResponse(IOutputSpeech speech, Reprompt reprompt, ILambdaContext context, bool endSession = false)
        {
            
            var attributes = new Dictionary<string, object>();
            attributes.Add("foo", "bar");

            var response = ResponseBuilder.Ask(speech, reprompt);
            response.Version = "1.0";
            response.SessionAttributes = attributes;
            response.Response.ShouldEndSession = endSession;

            context.Logger.Log(response.ToString());

            return response;
        }
    }
}