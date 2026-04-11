using System;
using System.Collections.Generic;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace oojjrs.onet
{
    internal class InternalPlayerSelfUpdater : MonoBehaviour
    {
        public Dictionary<string, PlayerProperty> PlayerProperties { get; set; }
        public ISession Session { get; set; }

        public event Action<MyNetSessionException> OnException;
        public event Action OnFailed;
        public event Action OnOk;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private async void Start()
        {
            try
            {
                if (Session.CurrentPlayer != default)
                {
                    Session.CurrentPlayer.SetProperties(PlayerProperties);

                    await Session.SaveCurrentPlayerDataAsync();

                    OnOk?.Invoke();
                }
                else
                {
                    OnFailed?.Invoke();
                }
            }
            catch (SessionException e)
            {
                OnException?.Invoke(MyNet.ToException(e));
            }
            finally
            {
                if (this != default)
                    Destroy(gameObject);
            }
        }
    }
}
