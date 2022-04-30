using System.ComponentModel;
using System.Drawing;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Results;

namespace HelpSystemBreaker;

[Group("break")]
public class TestGroup : CommandGroup
{

    private readonly FeedbackService _feedbackService;
    private readonly CommandContext  _commandContext;
    
    public TestGroup(FeedbackService feedbackService, CommandContext commandContext)
    {
        _feedbackService = feedbackService;
        _commandContext = commandContext;
    }

    [Command("description")]
    [CommandType(ApplicationCommandType.ChatInput)]
    [Description("The group breaks the help system, but here is a command")]
    public async Task<Result> RandomCommand()
    {
        var res = await _feedbackService.SendContentAsync(_commandContext.ChannelID, "Help system broken", Color.Aqua);
        return res.IsSuccess ? Result.FromSuccess() : Result.FromError(res);
    }
    
}