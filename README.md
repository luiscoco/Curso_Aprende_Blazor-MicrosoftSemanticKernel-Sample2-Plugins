# ConsoleAppAIPluginSample1

For more information about the concepts implemented in the sample see this URL: 

https://learn.microsoft.com/en-us/semantic-kernel/concepts/plugins/?pivots=programming-language-csharp

## 1. Create and Configure the Azure OpenAI service

Login Azure Portal and search for the **Azure OpenAI** service 

![image](https://github.com/user-attachments/assets/05aad478-92f0-47b4-8abe-01831620fbc1)

Create a new **Azure OpenAI** service

![image](https://github.com/user-attachments/assets/b637be9e-7eca-442f-9b50-536b31191c84)

Input the required data for defining the service and press the Next button

![image](https://github.com/user-attachments/assets/afb38d18-ae9e-4251-92d5-3cc85999881c)

![image](https://github.com/user-attachments/assets/f5e04951-4f2c-4812-b0f3-6ebf4d59c37a)

![image](https://github.com/user-attachments/assets/753347c8-747f-416e-a133-a08b1d6e17f5)

![image](https://github.com/user-attachments/assets/ecd7885e-da9e-4ce1-b568-4fb332f52ec7)

![image](https://github.com/user-attachments/assets/a068e83e-122d-4865-9ec3-1636bef2c660)

![image](https://github.com/user-attachments/assets/1944dfc4-afff-4b65-b494-ee79634ad11b)

![image](https://github.com/user-attachments/assets/6daca8cd-d4a2-476e-aa45-f0614d62ba58)

![image](https://github.com/user-attachments/assets/9bcb0b48-27b8-42ef-9442-c3dbe5dd58cb)

In the Azure OpenAI Studio press in the **Deploy model**

![image](https://github.com/user-attachments/assets/34936d15-d9a0-49fa-996a-bf2e08d80c38)

Now select the option **Deploy base model**

![image](https://github.com/user-attachments/assets/d742730a-f487-41a7-8103-a44e6fb87988)

Select the **AI model** 

![image](https://github.com/user-attachments/assets/c1a48ec7-aef3-4c3b-bf9e-d4b723f66dd5)

Input the data for defining the **AI model**

![image](https://github.com/user-attachments/assets/44fb5a2a-f94b-4ef0-8219-22f0ca56f746)

We copy the deployed model URI and Key

![image](https://github.com/user-attachments/assets/ed4e8744-8309-4f9c-8214-d1e017208a1a)

We paste the model URI, Key and model ID in the middleware (Program.cs):

```csharp
string modelId = "gpt-4";
string endpoint = "https://cocoe-m292d2lk-francecentral.openai.azure.com/";
string apiKey = "9176d49c31da45959c50f593c60e2034";

var builder = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);
```
