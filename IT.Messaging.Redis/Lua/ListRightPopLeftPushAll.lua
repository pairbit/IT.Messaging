local i = 0
while (redis.call('RPOPLPUSH', KEYS[1], KEYS[2]) ~= false) do
    i = i + 1
end
return i