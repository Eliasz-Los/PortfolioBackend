using System.Collections.Concurrent;
using System.Threading.Channels;
using BL.DocuGroup.Dto;

namespace BL.DocuGroup.Events;

/*Later to redis Pub Sub*/
public class InMemoryDocumentEventBroker : IDocumentEventBroker
{
    private readonly ConcurrentDictionary<Guid, Channel<DocEvent>> _channels = new();
    
    public ChannelReader<DocEvent> Subscribe(Guid documentId)
    {
        var ch = _channels.GetOrAdd(documentId, _ =>
            Channel.CreateUnbounded<DocEvent>(new UnboundedChannelOptions
            {
                SingleReader = false,
                SingleWriter = false,
                AllowSynchronousContinuations = false
            })
        );
        return ch.Reader;
    }

    public void Publish(DocEvent docEvent)
    {
        if (_channels.TryGetValue(docEvent.DocumentId, out var channel))
        {
            channel.Writer.TryWrite(docEvent);
        }
    }
}