using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

public class AgentService
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chat;

    public AgentService(Kernel kernel)
    {
        _kernel = kernel;
        _chat = kernel.GetRequiredService<IChatCompletionService>();
    }

    public async Task<string> RunAsync(string systemPrompt, string userMessage)
    {
        // Build the conversation with a system prompt and user message
        var history = new ChatHistory();
        history.AddSystemMessage(systemPrompt);
        history.AddUserMessage(userMessage);

        // Send to Claude and get a response
        var result = await _chat.GetChatMessageContentAsync(
            history,
            kernel: _kernel
        );

        return result.Content ?? "No response generated.";
    }
}