using TdLib;

namespace Tel.Egram.Model.Messaging.Explorer.Messages.Special;

public class DocumentMessageModel : AbstractSpecialMessageModel
{
    public required TdApi.Document Document { get; init; }
    
    public required string FileName  { get; init; }
    public required string Caption  { get; init; }
    public required string Size  { get; init; }
}