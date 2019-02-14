namespace NanoHistoryDumper
{
    using System;
    using System.Numerics;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NanoRpcSharp;

    public class Program
    {
        private readonly NanoRpcClient nanoRpcClient;

        private readonly ILogger logger;

        public Program(NanoRpcClient nanoRpcClient, ILogger<Program> logger)
        {
            this.nanoRpcClient = nanoRpcClient;
            this.logger = logger;
        }

        public static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var endpoint = config["NodeEndpoint"];

            var services = new ServiceCollection()
                .AddLogging(o => o.AddConfiguration(config.GetSection("Logging")).AddConsole())
                .AddTransient<Program>()
                .AddTransient<NanoRpcClient>()
                .AddHttpClient<NanoRpcClient>(o => o.BaseAddress = new Uri("http://" + endpoint)).Services
                .BuildServiceProvider();

            if (args.Length != 1)
            {
                Console.WriteLine("Wrong invocation! Put account as first/only commandline argument!");
                return;
            }

            var account = new Account(args[0]);

            var program = services.GetRequiredService<Program>();

            await program.Export(account);
        }

        public async Task Export(Account account)
        {
            var version = await nanoRpcClient.VersionAsync();
            logger.LogInformation($"Node vendor: {version.NodeVendor}");

            var multiplier = version.NodeVendor.StartsWith("Banano")
                ? await nanoRpcClient.BanToRawAsync(new BigInteger(1))
                : await nanoRpcClient.MraiToRawAsync(new BigInteger(1));

            var count = 0;

            Hex32? prev = null;
            do
            {
                var history = await nanoRpcClient.AccountHistoryAsync(account, false, 10, prev);
                prev = history.Previous;
                foreach (var item in history.History)
                {
                    var amnt = item.Amount * 1000 / multiplier;
                    var a = ((decimal)amnt) / 1000;
                    count++;

                    var msg = $"{item.Hash} {item.Type.PadRight(8)} {item.Account} {a}";
                    logger.LogDebug(msg);
                    Console.WriteLine(msg);
                }
            }
            while (prev != null);

            logger.LogInformation($"Done. {count} transactions found.");
        }
    }
}
