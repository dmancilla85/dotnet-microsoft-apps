using Microsoft.Bot.Builder;

namespace TeamsChatbot
{
  /// <summary>
  /// An empty bot handler.
  /// You can add your customization code here to extend your bot logic if needed.
  /// </summary>
  public class TeamsBot : IBot
  {
    public Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
    {
      Console.WriteLine($"I receive a message: {turnContext.Activity.CallerId}");
      return Task.CompletedTask;
    }
  }
}