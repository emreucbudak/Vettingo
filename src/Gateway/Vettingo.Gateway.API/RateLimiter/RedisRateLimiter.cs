using StackExchange.Redis;
using Vettingo.Gateway.API.Interface;

namespace Vettingo.Gateway.API.RateLimiter
{
    public class RedisRateLimiter(IConnectionMultiplexer connectionMultiplexer) : IRedisRateLimiter
    {
        private IDatabase database = connectionMultiplexer.GetDatabase();
        public async Task<bool> CheckRateLimit(string key)
        {
            string redisKey = $"rate_limit:{key.Trim().ToLowerInvariant()}";
            var script = """
                local key = KEYS[1]
                local refill_rate = 2
                local refill_interval = 5
                local time = redis.call('TIME')
                local now = tonumber(time[1]) 
                local bucket = redis.call('HMGET', key, 'capacity', 'tokens', 'last_refill')
                local token = tonumber(bucket[2])
                local capacity = tonumber(bucket[1])
                if bucket == false then
                    local capacity = 10
                    local token = 9
                    local last_refill = now
                    redis.call('HMSET', key, 'capacity', capacity, 'tokens', token, 'last_refill', last_refill)
                    return 1
                end
                local time_passed = now - last_refill
                local refills =  math.floor(time_passed / refill_interval)
                if refills > 0 then
                    token = math.min(capacity, token + (refills * refill_rate))
                    last_refill = now
                    redis.call('HMSET', key, 'tokens', token, 'last_refill', last_refill)
                end
                if token > 0 then
                    redis.call('HINCRBY', key, 'tokens', -1)
                    return 1
                else
                    return 0
                end
                """;
            var prepareScript = LuaScript.Prepare(script);
            IServer server = connectionMultiplexer.GetServer(connectionMultiplexer.GetEndPoints()[0]);
            var loadScript = await prepareScript.LoadAsync(server);
            var loads = await loadScript.EvaluateAsync(database, new { keys = redisKey });
            return (bool)loads;




        }
    }
}
