local removed = 0
local list = KEYS[1]
local count = ARGV[1]
for key, val in pairs(ARGV) do
	if key ~= 1 then
		removed = removed + redis.call('LREM', list, count, val)
	end
end
return removed