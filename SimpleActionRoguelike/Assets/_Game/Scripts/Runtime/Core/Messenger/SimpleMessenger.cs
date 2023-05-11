using Cysharp.Threading.Tasks;
using System;
using ZBase.Foundation.PubSub;

namespace Runtime.Core.Message
{
    public static class SimpleMessenger
    {
        private static Messenger s_messenger = new Messenger();

        #region Subcribe

        public static ISubscription Subscribe<TMessage>(Action handler) where TMessage : IMessage
        {
            return s_messenger.MessageSubscriber.Global().Subscribe<TMessage>(handler);
        }

        public static ISubscription Subscribe<TMessage>(Action<TMessage> handler) where TMessage : IMessage
        {
            return s_messenger.MessageSubscriber.Global().Subscribe<TMessage>(handler);
        }

        public static MessageSubscriber.Subscriber<TScope> Scope<TScope>(TScope scope)
        {
            return s_messenger.MessageSubscriber.Scope(scope);
        }

        #endregion Subcribe

        #region Publish

        public static void Publish<TMessage>() where TMessage : IMessage, new()
        {
            s_messenger.MessagePublisher.Global().Publish<TMessage>();
        }

        public static void Publish<TMessage>(TMessage message) where TMessage : IMessage
        {
            s_messenger.MessagePublisher.Global().Publish(message);
        }

        public static void Publish<TScope, TMessage>(TScope scope, TMessage message) where TMessage : IMessage
        {
            s_messenger.MessagePublisher.Scope(scope).Publish(message);
        }

        public static async UniTask PublishAsync<TMessage>() where TMessage : IMessage, new()
        {
            await s_messenger.MessagePublisher.Global().PublishAsync<TMessage>();
        }

        public static async UniTask PublishAsync<TMessage>(TMessage message) where TMessage : IMessage
        {
            await s_messenger.MessagePublisher.Global().PublishAsync(message);
        }

        public static async UniTask PublishAsync<TScope, TMessage>(TScope scope, TMessage message) where TMessage : IMessage
        {
            await s_messenger.MessagePublisher.Scope(scope).PublishAsync(message);
        }

        #endregion Publish
    }
}
