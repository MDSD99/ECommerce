using System;

namespace EventBus.Base
{
    public class SubscriptionInfo
	{
        //Dışarıdan Gönderilen verilerin içerde tutulması için
        //Alınan Tipe Göre Event Bulunacak.
        public Type HandlerType { get; set; }

        public SubscriptionInfo(Type handlerType)
        {
            HandlerType = handlerType??throw new ArgumentException(nameof(handlerType));
        }
        public static SubscriptionInfo Typed(Type HandlerType) => new SubscriptionInfo(HandlerType);
    }
}

