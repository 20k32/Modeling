using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Core.Messages.Base.SynchronousMessages;
using Modeling.Models.Drawing.DrawingMessageInterpreter;
using Modeling.Models.Drawing.DrawingMessageValues;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Modeling.Models.Drawing.DrawingPipeline
{
    sealed class DrawingPipeline : IDrawingPipeline
    {
        const int MAXIUM_PROCESSED_MESSAGES_COUNT_64 = 500;
        const int MAXIUM_PROCESSED_MESSAGES_COUNT_86 = 200;
        static readonly TimeSpan MAXIUM_TIME_TO_WAIT = TimeSpan.FromMilliseconds(800);

        readonly int _maximumProcessedMessagesCount;

        readonly ConcurrentQueue<DrawingMessageValue> _pipeline;
        readonly IDrawingMessageInterpreter _messageInterpreter;

        BlockingCollection<DrawingMessageValue> _consumerCollection;
        CancellationTokenSource _consumerCollectionCancelationSource;

        public bool HasItems => !_pipeline.IsEmpty;

        public event Action<DrawingMessageValue> MessageReceived;

        public DrawingPipeline()
        {
            _maximumProcessedMessagesCount = Environment.Is64BitProcess
                ? MAXIUM_PROCESSED_MESSAGES_COUNT_64
                : MAXIUM_PROCESSED_MESSAGES_COUNT_86;

            _messageInterpreter = Ioc.Default.GetService<IDrawingMessageInterpreter>();

            _pipeline = new ConcurrentQueue<DrawingMessageValue>();
        }

        public bool TryEnqueue(Message message)
        {
            var messageEnqueued = _messageInterpreter.TryInterpretMessage(message, out var interpretedValue);

            if (messageEnqueued)
            {
                messageEnqueued = _consumerCollection.TryAdd(interpretedValue, MAXIUM_TIME_TO_WAIT);
            }

            return messageEnqueued;
        }

        public void Initialize()
        {
            _consumerCollection = new BlockingCollection<DrawingMessageValue>(_pipeline, _maximumProcessedMessagesCount);
            _consumerCollectionCancelationSource = new CancellationTokenSource();

            foreach (var messageValue in _consumerCollection.GetConsumingEnumerable(_consumerCollectionCancelationSource.Token))
            {
                MessageReceived?.Invoke(messageValue);
            }
        }

        public async ValueTask DisposeAsync()
        {
            _consumerCollection.CompleteAdding();

            await _consumerCollectionCancelationSource.CancelAsync();

            _consumerCollectionCancelationSource.Dispose();
        }
    }
}
