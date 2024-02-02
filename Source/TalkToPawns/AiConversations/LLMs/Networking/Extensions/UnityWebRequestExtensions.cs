using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace AiConversations.LLMs.Networking.Extensions
{
    public static class UnityWebRequestExtensions
    {
        public static Task<string> SendWebRequestAsync(this UnityWebRequest request)
        {
            var completionSource = new TaskCompletionSource<string>();
            request.SendWebRequest().completed += asyncOperation =>
            {
                if (request.isNetworkError || request.isHttpError)
                {
                    // Include both the error message and the response body in the exception
                    var errorMessage = $"Error: {request.error}, Response Body: {request.downloadHandler?.text}";
                    completionSource.TrySetException(new Exception(errorMessage));
                }
                else
                {
                    completionSource.TrySetResult(request.downloadHandler.text);
                }
            };
            return completionSource.Task;
        }
    }
}
