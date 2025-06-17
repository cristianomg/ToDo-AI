using MediatR;
using System.Text.Json.Serialization;

namespace Todo.Application.Commands
{
    public class DeleteChecklistItemCommand : IRequest<bool>
    {
        [JsonIgnore]
        public int TaskId { get; set; }
        public int ItemId { get; set; }
        [JsonIgnore]
        public int UserId { get; set; }
    }
} 