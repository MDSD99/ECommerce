using System;
using System.Net.Sockets;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Polly;

#nullable disable
namespace EventBus.RabbitMQ
{
    public class RabbitMQPersistenceConnection : IDisposable
    {
        private IConnectionFactory ConnectionFactory;
        private readonly int retryCount;
        private IConnection Connection;
        private object lock_object=new();
        private bool Disposed;


        public RabbitMQPersistenceConnection(IConnectionFactory connectionFactory,int retryCount = 5 )
        { 
            this.ConnectionFactory = connectionFactory;
            this.retryCount = retryCount;
        }

        public bool IsConnected => Connection != null && Connection.IsOpen;

        public IModel CreatedModel()
            => Connection.CreateModel();

        public void Dispose()
        {
            Disposed = true;
            Connection.Dispose();
        }

        public bool TryConnect()
        {
            lock (lock_object)
            {
                var policy = Policy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(retryCount, retryattempt => TimeSpan.FromSeconds(Math.Pow(2, retryattempt)),
                    (ex,time) =>
                    {

                    });

                policy.Execute(() =>
                {
                    Connection = ConnectionFactory.CreateConnection();
                });

                if (IsConnected)
                {
                    Connection.ConnectionShutdown += Connection_ConnectionShutdown;
                    Connection.CallbackException += Connection_CallbackException;

                    return true;
                }
                return false;
            }
        }

        private void Connection_CallbackException(object sender, global::RabbitMQ.Client.Events.CallbackExceptionEventArgs e)
        {
            if (Disposed)
                return;
            TryConnect();
        }

        private void Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            if (Disposed)
                return;
            TryConnect();
        }
    }
}

