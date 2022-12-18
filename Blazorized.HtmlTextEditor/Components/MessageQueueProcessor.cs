using System.Collections.Concurrent;
namespace Blazorized.HtmlTextEditor;
public class MessageQueueProcessor
{
    public ConcurrentQueue<string> MessageQueue { get; set; }
    // Create an AutoResetEvent to signal the timeout threshold in the
    // timer callback has been reached.
    readonly AutoResetEvent _autoEvent = new(false);
    private Timer _fireTimer = default!;
    private readonly HtmlTextEditor _textEditor;
    private bool _messageDisplayInProgress = false;

    public MessageQueueProcessor(HtmlTextEditor textEditor)
    {
        _textEditor = textEditor;
        MessageQueue = new ConcurrentQueue<string>();
    }

    public void Enqueue(string message)
    {
        MessageQueue.Enqueue(message);
        while (MessageQueue.Count > 0)
        {
            if (_messageDisplayInProgress)
            {
                Task.Delay(1000).Wait();
                continue;
            }

            _messageDisplayInProgress = true;

            _fireTimer = new Timer(DequeueMessage!, _autoEvent, _textEditor.DelayInMsBetweenStatusChanges, _textEditor.DelayInMsBetweenStatusChanges);
        }
    }

    private async void DequeueMessage(object stateInfo)
    {
        var autoEvent = (AutoResetEvent)stateInfo;
        if (MessageQueue.Count <= 0)
        {
            autoEvent.Set();
            return;
        }

        if (!MessageQueue.TryDequeue(out var message)) return;

        await _textEditor.ShowStatusMessage(message);
        _messageDisplayInProgress = false;
    }
}
