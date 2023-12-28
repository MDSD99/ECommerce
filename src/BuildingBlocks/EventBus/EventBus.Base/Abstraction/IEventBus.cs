using EventBus.Base.Events;

namespace EventBus.Base.Abstraction
{
    public interface IEventBus :IDisposable
	{
		void Publish(IntegrationEvent @event); //Event Fırlatır.

		void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;//Subscribe

        void UnSubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;//Unsubscribe
    }
}

