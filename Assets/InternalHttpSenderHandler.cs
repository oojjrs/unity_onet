using System.Collections;
using UnityEngine;

namespace oojjrs.onet
{
    internal class InternalHttpSenderHandler : MonoBehaviour
    {
        private bool _isQuitting;

        private InternalHttpSender CurrentRequester { get; set; }
        public float NetworkCooldownTimeSeconds { get; set; } = 10;
        private MyNetRequest PendingRequest { get; set; }
        public float ResendCooldownTimeSeconds { get; set; } = 3;
        public int RetryCount { get; set; } = 3;

        private void OnApplicationQuit()
        {
            _isQuitting = true;
        }

        private void Start()
        {
            _ = StartCoroutine(Func());

            IEnumerator Func()
            {
                while (true)
                {
                    if (HasRequest() == false)
                        yield return new WaitUntil(HasRequest);

                    if (_isQuitting)
                        break;

                    MyNetRequest request;
                    if (PendingRequest != default)
                        request = PendingRequest;
                    else
                        MyNet.Packets.TryDequeue(out request);

                    if (request != default)
                    {
                        for (int i = 0; i < RetryCount; ++i)
                        {
                            PendingRequest = default;

                            CurrentRequester = MyNet.Packets.CreateNew(request, new("https://ThisIsNotWorking"));
                            CurrentRequester.OnError += () =>
                            {
                                CurrentRequester = default;
                                PendingRequest = request;
                            };
                            CurrentRequester.OnReceived += stream =>
                            {
                                CurrentRequester = default;
                            };
                            //CurrentRequester.OnUnauthorized += () =>
                            //{
                            //    CurrentRequester = default;

                            //    var t = Hub.Web.Requests.ToArray();
                            //    Hub.Web.Requests.Clear();

                            //    UserCommand.Refresh();
                            //    Hub.Web.Send(request);
                            //    foreach (var r in t)
                            //        Hub.Web.Send(r);
                            //};

                            yield return new WaitUntil(() => CurrentRequester == default);

                            if (PendingRequest != default)
                                yield return new WaitForSeconds(ResendCooldownTimeSeconds);
                            else
                                break;
                        }
                    }

                    // 뭔가 네트워크 에러가 있을 것이므로 한참 기다린다.
                    if (PendingRequest != default)
                        yield return new WaitForSeconds(NetworkCooldownTimeSeconds);
                }

                bool HasRequest()
                {
                    if (_isQuitting)
                    {
                        return true;
                    }
                    else
                    {
                        // TODO: 더 이상 여기서 토큰 체크를 하지 않는다. 일단 이걸 쓸건지부터가...
                        //var request = PendingRequest;
                        //if (request == default)
                        //    Hub.Web.Requests.TryPeek(out request);

                        //if (request != default)
                        //{
                        //    // 다음 요청은 토큰을 필요로 하지 않는 패킷들이다.
                        //    if (request is UserLogin or UserRefresh)
                        //        return true;
                        //    else
                        //        return string.IsNullOrWhiteSpace(Hub.Web.Token) == false;
                        //}
                        //else
                        //{
                        //    return false;
                        //}
                        return MyNet.Packets.HasRequest();
                    }
                }
            }
        }
    }
}
