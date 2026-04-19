using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace oojjrs.onet
{
    public class MyNetAuthenticator : MonoBehaviour
    {
        public interface CallbackInterface
        {
            CancellationToken CancellationToken { get; }
            ILogger Logger { get; }

            void OnAuthenticated(string account, string nickname);
            void OnError(MyNetAuthenticationException e);
            void OnError(OperationCanceledException e);
            void OnError(MyNetRequestFailedException e);
        }

        private bool _isQuitting = false;

        private void OnApplicationQuit()
        {
            _isQuitting = true;
        }

        private async void Start()
        {
            await RunAsync();

            if (_isQuitting == false)
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

                callback?.OnAuthenticated(AuthenticationService.Instance.PlayerId, playerName);
            }
            catch (AuthenticationException e)
            {
                callback?.OnError(MyNet.ToException(e));
            }
            catch (OperationCanceledException e)
            {
                callback?.OnError(e);
            }
            catch (RequestFailedException e)
            {
                callback?.OnError(MyNet.ToException(e));
            }

            bool IsAlive()
            {
                return (this != default) && (callback?.CancellationToken.IsCancellationRequested == false);
            }
        }
    }
}
