using System;
using Light.Unity.AspNetCore.HTTP.Extensions.Examples;
using Light.Unity.Extensions;

namespace Light.Unity.AspNetCore.HTTP.Extensions
{
    internal class BaseWebRequestsExample : BaseWebRequests
    {
        public const string SignUpUrl = "/Account/Register";
        public const string SignInUrl = "/Account/Login";
        public const string JoinLobbyUrl = "Lobby/JoinLobby";

        private static readonly BaseWebRequestsExample Instance;

        static BaseWebRequestsExample()
        {
            Instance = new BaseWebRequestsExample();
        }

        public override int GetTimeout()
        {
            return 100000;
        }

        public override string GetBaseDomain()
        {
            return "http://localhost:5151";
        }

        public static async void SignUp(CreateAccountQueryModel query, Action<HttpRequestResult<string>> onResult)
        {
            await SafeInvoke(
                Instance?.SafeRequest<string>(SignUpUrl, message => message.SetRequestBody(query)), result =>
                {
                    if (result.MessageResponse.IsSuccessStatusCode)
                        Instance.SetDefaultHeader("Authorization", $"Bearer {result.Data}");

                    onResult(result);
                });
        }

        public static async void PasswordSignInSample3(PasswordSignInRequestModel query,
            Action<HttpRequestResult<string>> onResult)
        {
            await SafeInvoke(Instance.SafeRequest<string>(SignInUrl, message => message.SetRequestBody(query)),
                             result =>
                             {
                                 if (result.MessageResponse.IsSuccessStatusCode)
                                     Instance.SetDefaultHeader("Authorization", $"Bearer {result.Data}");

                                 onResult(result);
                             });
        }

        public static async void PasswordSignInSample2(PasswordSignInRequestModel query,
            Action<HttpRequestResult<string>> onResult)
        {
            var result = await Instance.SafeRequest<string>(SignInUrl, message => message.SetRequestBody(query));

            if (result.MessageResponse.IsSuccessStatusCode)
                Instance.SetDefaultHeader("Authorization", $"Bearer {result.Data}");

            ThreadHelper.AddAction(() => onResult(result));
        }

        public static async void PasswordSignInSample1(PasswordSignInRequestModel query,
            Action<HttpRequestResult<string>> onResult)
        {
            var client = await Instance.GetClient();

            var result =
                await Instance.SafeRequest<string>(client, SignInUrl, message => { message.SetRequestBody(query); });

            if (result.MessageResponse.IsSuccessStatusCode)
                Instance.SetDefaultHeader("Authorization", $"Bearer {result.Data}");

            ThreadHelper.AddAction(() => onResult(result));

            Instance.FreeClient(client, result);
        }

        public static async void JoinLobby(int query, Action<HttpRequestResult<string>> onResult)
        {
            await SafeInvoke(Instance.SafeRequest<string>(JoinLobbyUrl, message => message.SetRequestBody(query)),
                             result => { onResult(result); });
        }
    }
}