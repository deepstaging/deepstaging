namespace Deepstaging.ToDo;

public interface ISlackService
{
    Task PostMessageAsync(string channel, string message);
}