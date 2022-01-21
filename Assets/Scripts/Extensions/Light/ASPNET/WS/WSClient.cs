using System;
using System.Threading.Tasks;
using Light.Unity.AspNetCore.Phantom;
using Light.Unity.Extensions;
using UnityEngine;

namespace Light.Unity.AspNetCore.WS.Extensions
{
    public class WSClient : IDisposable
    {
        private readonly PhantomHubConnection _hubConnection;

        protected WSClient()
        {
            _hubConnection = new PhantomHubConnectionBuilder()
                .WithUrl(GetUrl(), o => { o.AccessTokenProvider = () => Task.FromResult(GetAccessToken()); })
                .WithAutomaticReconnect(GetReconnectPolicy())
                .Build();

            _hubConnection.Closed += e =>
            {
                ThreadHelper.AddAction(() => OnClosed(e));
                return Task.CompletedTask;
            };
            _hubConnection.Reconnected += e =>
            {
                ThreadHelper.AddAction(() => OnReconnected(e));
                return Task.CompletedTask;
            };
            _hubConnection.Reconnecting += e =>
            {
                ThreadHelper.AddAction(() => OnReconnecting(e));
                return Task.CompletedTask;
            };
        }

        public HubConnectionState CurrentState => _hubConnection.State;

        public void Dispose()
        {
            _hubConnection.DisposeAsync();
        }

        protected virtual string GetUrl()
        {
            return "http://localhosts/hubs/hub";
        }

        protected virtual WSRetryPolicy GetReconnectPolicy()
        {
            return WSRetryPolicy.CreateNone();
        }

        protected virtual string GetAccessToken()
        {
            return string.Empty;
        }

        public async Task Connect()
        {
            try
            {
                await _hubConnection.StartAsync();
            }
            catch (Exception e)
            {
                ThreadHelper.AddAction(() => OnClosed(e));
            }
        }

        public async void ConnectAsync()
        {
            await Connect();
        }

        public async void ConnectAsync(Action<WSClient> afterConnect)
        {
            await Connect();
            ThreadHelper.AddAction(() => afterConnect(this));
        }

        public Task Disconnect()
        {
            return _hubConnection.StopAsync();
        }

        public async void DisconnectAsync()
        {
            await Disconnect();
        }

        public async void DisconnectAsync(Action<WSClient> afterDisconnect)
        {
            await Disconnect();
            ThreadHelper.AddAction(() => afterDisconnect(this));
        }

        public void Handle<T1>(string methodName, Action<T1> args)
        {
            _hubConnection.On<T1>(methodName, p1 => { ThreadHelper.AddAction(() => { args(p1); }); });
        }

        public void Handle(string methodName, Action args)
        {
            _hubConnection.On(methodName, () => { ThreadHelper.AddAction(() => { args(); }); });
        }

        public async void SendAsync(string methodName)
        {
            await _hubConnection.SendAsync(methodName);
        }

        public async void SendAsync<TData>(string methodName, TData data)
        {
            await _hubConnection.SendAsync(methodName, data);
        }

        public async void SendAsync(string methodName, Action<WSClient> afterSend)
        {
            await _hubConnection.SendAsync(methodName);
            ThreadHelper.AddAction(() => afterSend(this));
        }

        public async void SendAsync<TData>(string methodName, TData data, Action<WSClient, TData> afterSend)
        {
            await _hubConnection.SendAsync(methodName, data);
            ThreadHelper.AddAction(() => afterSend(this, data));
        }

        protected virtual void OnClosed(Exception ex)
        {
            if (ex != null) Debug.LogException(ex);
        }

        protected virtual void OnReconnecting(Exception ex) { }

        protected virtual void OnReconnected(string ex) { }
    }
}