local i = 0
while (redis.call('LMOVE', KEYS[1], KEYS[2], ARGV[1], ARGV[2]) ~= false) do
    i = i + 1
end
return i