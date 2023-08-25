# HTTP and Semantic Skills Example

The example is meant to demonstrate the use a planner (e.g. `SequentialPlanner`) to orchestrate a `HTTP` skill (e.g. `GET`) and a semantic skill (e.g. `ExplainCode`) to fetch needed content for responding to a query.

## Preparing your environment

Before you get started, make sure you have the following requirements in place:
- [.NET 7.0 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- [Azure OpenAI](https://aka.ms/oai/access) resource.

## Running the sample

1. Clone the repository
2. Set the following environment variables to configure your AI service and GitHub credentials.
   - AZURE_OPENAI_CHAT_DEPLOYMENT_NAME
   - AZURE_OPENAI_CHAT_ENDPOINT
   - AZURE_OPENAI_CHAT_KEY
3. Run the sample
   Open a terminal window, change directory to the `http` project, then run
   ```
   dotnet run "get the PowerShell code from https://gist.githubusercontent.com/daxian-dbw/38f2cf2eb4e3a37f316dad73ca551e94/raw/c9929d101a112c142bf22b9e59fd2ecfef178e59/Add-User.ps1 and explain what it does"
   ```
