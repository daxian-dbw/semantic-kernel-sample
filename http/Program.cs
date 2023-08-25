// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning;
using Microsoft.SemanticKernel.Planning.Sequential;
using Microsoft.SemanticKernel.Skills.Core;
using Microsoft.SemanticKernel.Skills.Web;

namespace MyApp;

public static class Example12_SequentialPlanner
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("======== Use Sequential Planner ================================================");
        Console.WriteLine();

        var kernel = InitializeKernelAndPlanner(out var planner, maxTokens: 1024);
        kernel.ImportSkill(new HttpSkill(), "Http");
        // kernel.ImportSkill(new FileIOSkill(), "FileIO");

        // Load additional skills to enable planner to do non-trivial asks.
        string location = Path.GetDirectoryName(typeof(Example12_SequentialPlanner).Assembly.Location);
        kernel.ImportSemanticSkillFromDirectory(Path.Combine(location, "Skills"), "CodeSkill");

        // Console.WriteLine();
        // var functionsView = kernel.Skills.GetFunctionsView();
        // foreach (var skill in functionsView.NativeFunctions)
        // {
        //     Console.WriteLine($"Skill: {skill.Key}");
        //     foreach (var func in skill.Value)
        //     {
        //         Console.WriteLine($"  name: {func.Name}");
        //         foreach (var param in func.Parameters)
        //         {
        //             Console.WriteLine($"    param: {param.Name}");
        //             Console.WriteLine($"      descp: {param.Description}");
        //         }
        //     }

        //     Console.WriteLine();
        // }

        string prompt = args[0];
        Console.WriteLine($"prompt: {prompt}");
        Console.WriteLine();
        var plan = await planner.CreatePlanAsync(prompt);

        Console.WriteLine("Generated plan:");
        Console.WriteLine(plan.ToPlanString());
        Console.WriteLine();

        Console.Write("Executing the plan? (y/n) ");
        var key = Console.ReadKey(intercept: false);
        Console.WriteLine();

        if (key.KeyChar != 'n')
        {
            Console.WriteLine();
            await ExecutePlanAsync(kernel, plan, string.Empty, 5);
        }
        else
        {
            Console.WriteLine("Execution cancelled. Exiting...");
        }
    }

    private static IKernel InitializeKernelAndPlanner(out SequentialPlanner planner, int maxTokens = 1024)
    {
        var kernel = new KernelBuilder()
            .WithAzureChatCompletionService(
                Environment.GetEnvironmentVariable("AZURE_OPENAI_CHAT_DEPLOYMENT_NAME"),
                Environment.GetEnvironmentVariable("AZURE_OPENAI_CHAT_ENDPOINT"),
                Environment.GetEnvironmentVariable("AZURE_OPENAI_CHAT_KEY"))
            .Build();

        planner = new SequentialPlanner(kernel, new SequentialPlannerConfig { MaxTokens = maxTokens });

        return kernel;
    }

    private static async Task<Plan> ExecutePlanAsync(
        IKernel kernel,
        Plan plan,
        string input = "",
        int maxSteps = 10)
    {
        Stopwatch sw = new();
        sw.Start();

        // loop until complete or at most N steps
        try
        {
            for (int step = 1; plan.HasNextStep && step < maxSteps; step++)
            {
                if (string.IsNullOrEmpty(input))
                {
                    //or await plan.InvokeNextStepAsync(kernel.CreateNewContext());
                    await kernel.StepAsync(plan);
                }
                else
                {
                    plan = await kernel.StepAsync(input, plan);
                    input = string.Empty;
                }

                if (!plan.HasNextStep)
                {
                    Console.WriteLine($"Step {step} - COMPLETE!\n");
                    Console.WriteLine($"\x1b[33m{plan.State}\x1b[0m");
                    Console.WriteLine();
                    break;
                }

                Console.WriteLine($"Step {step} - Results so far:\n");
                Console.WriteLine($"\x1b[33m{plan.State}\x1b[0m");
                Console.WriteLine();
            }
        }
        catch (KernelException e)
        {
            Console.WriteLine("Step - Execution failed:");
            Console.WriteLine(e.Message);
        }

        sw.Stop();
        Console.WriteLine($"======== Execution complete in {sw.ElapsedMilliseconds} ms! =============================================");
        return plan;
    }
}
