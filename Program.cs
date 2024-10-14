//#pragma warning disable SKEXP0001
//#pragma warning disable SKEXP0003
//#pragma warning disable SKEXP0010
//#pragma warning disable SKEXP0011
//#pragma warning disable SKEXP0050
//#pragma warning disable SKEXP0052

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text.Json.Serialization;
//using System.Threading.Tasks;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.SemanticKernel;
//using Microsoft.SemanticKernel.ChatCompletion;
//using Microsoft.SemanticKernel.Connectors.OpenAI;

//public class Program
//{
//    public static async Task Main(string[] args)
//    {
//        // Define your Azure OpenAI credentials
//        string modelId = "gpt-4"; // Ensure this matches your deployed model name
//        string endpoint = "https://cocoe-m292d2lk-francecentral.openai.azure.com/"; // Base endpoint for your deployment
//        string apiKey = "9176d49c31da45959c50f593c60e2034"; // Replace with your API key

//        // Create a kernel with Azure OpenAI chat completion
//        var builder = Kernel.CreateBuilder()
//            .AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);

//        // Build the kernel
//        Kernel kernel = builder.Build();
//        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

//        // Add the Lights plugin to the kernel
//        kernel.Plugins.AddFromType<LightsPlugin>("Lights");

//        // Enable planning
//        OpenAIPromptExecutionSettings executionSettings = new()
//        {
//            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
//        };

//        // Create a history store for the conversation
//        var history = new ChatHistory();
//        history.AddUserMessage("Please turn on the lamp");

//        // Get the response from the AI
//        try
//        {
//            var result = await chatCompletionService.GetChatMessageContentAsync(
//                history,
//                executionSettings: executionSettings,
//                kernel: kernel
//            );

//            // Extract the assistant's message content
//            string assistantMessage = result.Content; // Use .Content to get the message string

//            // Print the results
//            Console.WriteLine("Assistant > " + assistantMessage);

//            // Add the message from the agent to the chat history
//            history.AddAssistantMessage(assistantMessage);
//        }
//        catch (Microsoft.SemanticKernel.HttpOperationException ex)
//        {
//            Console.WriteLine($"Error: {ex.Message}");
//        }
//    }
//}

//// Define the LightsPlugin
//public class LightsPlugin
//{
//    // Mock data for the lights
//    private readonly List<LightModel> lights = new()
//    {
//        new LightModel { Id = 1, Name = "Table Lamp", IsOn = false, Brightness = 100, Hex = "FF0000" },
//        new LightModel { Id = 2, Name = "Porch Light", IsOn = false, Brightness = 50, Hex = "00FF00" },
//        new LightModel { Id = 3, Name = "Chandelier", IsOn = true, Brightness = 75, Hex = "0000FF" }
//    };

//    [KernelFunction("get_lights")]
//    [System.ComponentModel.Description("Gets a list of lights and their current state")]
//    [return: System.ComponentModel.Description("An array of lights")]
//    public async Task<List<LightModel>> GetLightsAsync()
//    {
//        return await Task.FromResult(lights);
//    }

//    [KernelFunction("get_state")]
//    [System.ComponentModel.Description("Gets the state of a particular light")]
//    [return: System.ComponentModel.Description("The state of the light")]
//    public async Task<LightModel?> GetStateAsync([System.ComponentModel.Description("The ID of the light")] int id)
//    {
//        return await Task.FromResult(lights.FirstOrDefault(light => light.Id == id));
//    }

//    [KernelFunction("change_state")]
//    [System.ComponentModel.Description("Changes the state of the light")]
//    [return: System.ComponentModel.Description("The updated state of the light; will return null if the light does not exist")]
//    public async Task<LightModel?> ChangeStateAsync(int id, LightModel lightModel)
//    {
//        var light = lights.FirstOrDefault(light => light.Id == id);

//        if (light == null)
//        {
//            return null;
//        }

//        // Update the light with the new state
//        light.IsOn = lightModel.IsOn;
//        light.Brightness = lightModel.Brightness;
//        light.Hex = lightModel.Hex;

//        return await Task.FromResult(light);
//    }
//}

//// Define the LightModel
//public class LightModel
//{
//    [JsonPropertyName("id")]
//    public int Id { get; set; }

//    [JsonPropertyName("name")]
//    public string Name { get; set; }

//    [JsonPropertyName("is_on")]
//    public bool? IsOn { get; set; }

//    [JsonPropertyName("brightness")]
//    public byte? Brightness { get; set; }

//    [JsonPropertyName("hex")]
//    public string? Hex { get; set; }
//}


#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0003
#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0011
#pragma warning disable SKEXP0050
#pragma warning disable SKEXP0052

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

public class Program
{
    public static async Task Main(string[] args)
    {
        string modelId = "gpt-4";
        string endpoint = "https://cocoe-m292d2lk-francecentral.openai.azure.com/";
        string apiKey = "9176d49c31da45959c50f593c60e2034";

        var builder = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);

        Kernel kernel = builder.Build();
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        kernel.Plugins.AddFromType<LightsPlugin>("Lights");

        // Load initial state of lights from a JSON file
        var lightsPlugin = new LightsPlugin();
        await lightsPlugin.LoadStateAsync();

        var history = new ChatHistory();
        string userInput;

        Console.WriteLine("Welcome to the Smart Light Control System!");
        Console.WriteLine("Available commands: 'list', 'turn on [light name]', 'turn off [light name]', 'set brightness [light name] [value]', 'set color [light name] [hex code]', 'exit'");

        while (true)
        {
            Console.Write("Enter command: ");
            userInput = Console.ReadLine()?.Trim().ToLower();

            if (userInput == "exit")
            {
                await lightsPlugin.SaveStateAsync();
                break;
            }

            await HandleUserInput(userInput, lightsPlugin, chatCompletionService, history);
        }
    }

    private static async Task HandleUserInput(string userInput, LightsPlugin lightsPlugin, IChatCompletionService chatCompletionService, ChatHistory history)
    {
        try
        {
            if (userInput.StartsWith("turn on"))
            {
                var lightName = userInput.Substring(8);
                await lightsPlugin.ChangeStateAsync(lightName, true);
                Console.WriteLine($"{lightName} has been turned on.");
            }
            else if (userInput.StartsWith("turn off"))
            {
                var lightName = userInput.Substring(9);
                await lightsPlugin.ChangeStateAsync(lightName, false);
                Console.WriteLine($"{lightName} has been turned off.");
            }
            else if (userInput.StartsWith("set brightness"))
            {
                var parts = userInput.Split(' ');
                var lightName = parts[2];
                var brightness = byte.Parse(parts[3]);
                await lightsPlugin.ChangeBrightnessAsync(lightName, brightness);
                Console.WriteLine($"{lightName} brightness set to {brightness}.");
            }
            else if (userInput.StartsWith("set color"))
            {
                var parts = userInput.Split(' ');
                var lightName = parts[2];
                var color = parts[3];
                await lightsPlugin.ChangeColorAsync(lightName, color);
                Console.WriteLine($"{lightName} color set to {color}.");
            }
            else if (userInput == "list")
            {
                var lights = await lightsPlugin.GetLightsAsync();
                foreach (var light in lights)
                {
                    Console.WriteLine($"Light: {light.Name}, State: {(light.IsOn == true ? "On" : "Off")}, Brightness: {light.Brightness}, Color: {light.Hex}");
                }
            }
            else
            {
                Console.WriteLine("Unknown command.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}

// Define the LightsPlugin
public class LightsPlugin
{
    private readonly List<LightModel> lights = new()
    {
        new LightModel { Id = 1, Name = "Table Lamp", IsOn = false, Brightness = 100, Hex = "FF0000" },
        new LightModel { Id = 2, Name = "Porch Light", IsOn = false, Brightness = 50, Hex = "00FF00" },
        new LightModel { Id = 3, Name = "Chandelier", IsOn = true, Brightness = 75, Hex = "0000FF" }
    };

    [KernelFunction("get_lights")]
    [System.ComponentModel.Description("Gets a list of lights and their current state")]
    public async Task<List<LightModel>> GetLightsAsync()
    {
        return await Task.FromResult(lights);
    }

    public async Task ChangeStateAsync(string name, bool state)
    {
        var light = lights.FirstOrDefault(l => l.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (light != null)
        {
            light.IsOn = state;
            await SaveStateAsync(); // Save state after change
        }
    }

    public async Task ChangeBrightnessAsync(string name, byte brightness)
    {
        var light = lights.FirstOrDefault(l => l.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (light != null)
        {
            light.Brightness = brightness;
            await SaveStateAsync(); // Save state after change
        }
    }

    public async Task ChangeColorAsync(string name, string hex)
    {
        var light = lights.FirstOrDefault(l => l.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (light != null)
        {
            light.Hex = hex;
            await SaveStateAsync(); // Save state after change
        }
    }

    public async Task LoadStateAsync()
    {
        if (File.Exists("lights.json"))
        {
            var json = await File.ReadAllTextAsync("lights.json");
            var loadedLights = JsonSerializer.Deserialize<List<LightModel>>(json);
            if (loadedLights != null)
            {
                lights.Clear();
                lights.AddRange(loadedLights);
            }
        }
    }

    public async Task SaveStateAsync()
    {
        var json = JsonSerializer.Serialize(lights);
        await File.WriteAllTextAsync("lights.json", json);
    }
}

// Define the LightModel
public class LightModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("is_on")]
    public bool? IsOn { get; set; }

    [JsonPropertyName("brightness")]
    public byte? Brightness { get; set; }

    [JsonPropertyName("hex")]
    public string? Hex { get; set; }
}

