local m = {}

local start = function ()
  print ('bootstrap start')
end
m.start = start

local tick = 0
local update = function ()
  tick = tick + 1
  if (tick % 100 == 0) then
    print ('bootstrap update change1')
  end
end
m.update = update

local hotfix = function (path)
  print ('hotfix', path)
  require(path)
end
m.hotfix = hotfix

return m
