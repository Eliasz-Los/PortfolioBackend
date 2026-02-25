using System.Threading.Channels;
using BL.DocuGroup.Dto;

namespace BL.DocuGroup.Events;

public interface IDocumentEventBroker
{
    ChannelReader<DocEvent> Subscribe(Guid documentId);
    void Publish(DocEvent docEvent);
}