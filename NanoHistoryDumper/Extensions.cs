﻿namespace NanoHistoryDumper
{
    using System.Numerics;
    using System.Threading.Tasks;
    using NanoRpcSharp;
    using NanoRpcSharp.Messages;

    public static class Extensions
    {
        public static async Task<BigInteger> BanToRawAsync(this NanoRpcClient client, BigInteger amount)
        {
            var r = await client.SendAsync(new BanToRawRequest(amount));
            return r.Amount;
        }
    }
}
