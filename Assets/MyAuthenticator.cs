using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace oojjrs.onet
{
    public class MyAuthenticator : MonoBehaviour
    {
        public interface CallbackInterface
        {
            CancellationToken CancellationToken { get; }
            ILogger Logger { get; }

            void OnAuthenticated(string account, string nickname);
            void OnError(MyAuthenticationException e);
            void OnError(OperationCanceledException e);
            void OnError(MyRequestFailedException e);
        }

        private async void Start()
        {
            await RunAsync();

            Destroy(gameObject);
        }

        private Task RunAsync()
        {
            return RunAsync(GetComponent<CallbackInterface>());
        }

        private async Task RunAsync(CallbackInterface callback)
        {
            var logger = callback?.Logger ?? Debug.unityLogger;
            if (callback == default)
            {
                // 경고 로깅을 이상하게 해야되네 -.-
                logger.Log(LogType.Warning, $"{name}> DON'T HAVE CALLBACK FUNCTION.");
                return;
            }

            try
            {
                if (UnityServices.State is not (ServicesInitializationState.Initialized or ServicesInitializationState.Initializing))
                    await UnityServices.InitializeAsync();

                if (IsAlive() == false)
                    return;

                if (AuthenticationService.Instance.IsSignedIn == false)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                    if (IsAlive() == false)
                        return;

                    logger.Log($"{name}> Sign in anonymously succeeded!");
                }

                var playerName = await AuthenticationService.Instance.GetPlayerNameAsync();
                if (IsAlive() == false)
                    return;

                callback.OnAuthenticated(AuthenticationService.Instance.PlayerId, playerName);
            }
            catch (AuthenticationException e)
            {
                callback.OnError(MyNet.ToException(e));
            }
            catch (OperationCanceledException e)
            {
                callback.OnError(e);
            }
            catch (RequestFailedException e)
            {
                callback.OnError(MyNet.ToException(e));
            }

            bool IsAlive()
            {
                return (this != default) && (callback.CancellationToken.IsCancellationRequested == false);
            }
        }
    }
}
