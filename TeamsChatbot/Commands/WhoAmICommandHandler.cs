using AdaptiveCards.Templating;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.TeamsFx.Conversation;
using Newtonsoft.Json;
using TeamsChatbot.Models;

namespace TeamsChatbot.Commands
{
  /// <summary>
  /// The <see cref="WhoAmICommandHandler"/> registers a pattern with the <see cref="ITeamsCommandHandler"/> and
  /// responds with an Adaptive Card if the user types the <see cref="TriggerPatterns"/>.
  /// </summary>
  public class WhoAmICommandHandler : ITeamsCommandHandler
  {
    private readonly ILogger<WhoAmICommandHandler> _logger;
    private readonly string _adaptiveCardFilePath = Path.Combine(".", "Resources", "WhoAmICard.json");

    public IEnumerable<ITriggerPattern> TriggerPatterns => new List<ITriggerPattern>
        {
            // Used to trigger the command handler if the command text contains 'helloWorld'
            new RegExpTrigger("who am I")
        };

    public WhoAmICommandHandler(ILogger<WhoAmICommandHandler> logger)
    {
      _logger = logger;
    }

    public async Task<ICommandResponse> HandleCommandAsync(ITurnContext turnContext, CommandMessage message, CancellationToken cancellationToken = default)
    {
      _logger?.LogInformation($"Bot received message: {message.Text}");

      // Read adaptive card template
      string cardTemplate = await File.ReadAllTextAsync(_adaptiveCardFilePath, cancellationToken);

      // Render adaptive card content
      string cardContent = new AdaptiveCardTemplate(cardTemplate).Expand
      (
          new HelloWorldModel
          {
            Title = "You want to know who am I?",
            Body = "Congratulations! Your hello world bot is running. Click the documentation below to learn more about Bots and the Teams Toolkit.",
          }
      );

      // Build attachment
      IMessageActivity activity = MessageFactory.Attachment
      (
          new Attachment
          {
            ContentType = "application/vnd.microsoft.card.adaptive",
            Content = JsonConvert.DeserializeObject(cardContent),
          }
      );

      // send response
      return new ActivityCommandResponse(activity);
    }
  }
}