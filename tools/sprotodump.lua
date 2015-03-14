local parse_core = require "sprotoparse_core"
local spb = require "sprotodump_spb"
local cSharp = require "sprotodump_cSharp"

local README = [[
usage: lua sprotodump.lua [[<out_option> <out>] ...] <option> <sproto_file ...>

  out_option:
    -d               dump to speciffic dircetory
    -o               dump to speciffic file
    -p               set package name(only cSharp code use)

  option: 
    -cs              dump to cSharp code file
    -spb             dump to binary spb  file]]

local function base_name(string_)
  local LUA_DIRSEP = string.sub(package.config,1,1)
  string_ = string_ or ''
  local basename = string.gsub (string_, '[^'.. LUA_DIRSEP ..']*'.. LUA_DIRSEP ..'', '')
  basename = string.gsub(basename, "(.+)%..+$", "%1")
  return basename
end



local function read_file(path)
  local handle = io.open(path, "r")
  local ret = handle:read("*a")
  handle:close()
  return ret
end

local function write_file(path, data, mode)
  local handle = io.open(path, mode)
  handle:write(data)
  handle:close()
  print("dump "..path.." file")
end


local function dump_spb(trunk_list, param)
  local outfile = param.outfile or param.package and base_name(param.package)..".spb" or "sproto.spb"
  outfile = (param.dircetory or "")..outfile
  local _, build = parse_core.gen_trunk(trunk_list)
  local data = spb.parse(build)
  write_file(outfile, data, "wb")
end


local function dump_cSharp(trunk_list, param)
  local package = base_name(param.package or "")
  local ret, build = parse_core.gen_trunk(trunk_list, package)
  local outfile = param.outfile
  local dir = param.dircetory or ""

  if outfile then
    local data = cSharp.parse_all(build, package, table.concat(param.sproto_file, " "))
    write_file(dir..outfile, data, "w")
  else
    -- dump sprototype
    for i,v in ipairs(ret) do
      local name = param.sproto_file[i]
      local outcs = base_name(name)..".cs"
      local data = cSharp.parse_type(v.type, package, name)
      write_file(dir..outcs, data, "w")
    end

    -- dump protocol
    if build.protocol then
      local data = cSharp.parse_protocol(build.protocol, package)
      local outfile = package.."Protocol.cs"
      write_file(dir..outfile, data, "w")
    end
  end
end


local function _2trunk_list(sproto_file)
  local trunk_list = {}
  for i,v in ipairs(sproto_file) do
    table.insert(trunk_list, {read_file(v), v})
  end
  return trunk_list
end


local function _parse_param(...)
  local param = {...}
  local ret = {
    dircetory = false,
    package = false,
    outfile = false,
    sproto_file = {},
    dump_type = false,
  }

  if #param ==0 then
    return false
  end

  local out_option = {
    ["-d"] = "dircetory",
    ["-o"] = "outfile",
    ["-p"] = "package",
  }

  local options = {
    ["-cs"] = true,
    ["-spb"] = true,
  }


  local function read_out_opt(idx)
    local v1 = param[idx]
    local v2 = param[idx+1]
    local k = out_option[v1]
    if k and v2 then
      ret[k] = v2
      idx = idx + 2
      return idx
    end
    return false
  end

  local function read_opt(idx)
    local begin = idx
    local v1 = param[idx]
    if options[v1] then
      ret.dump_type = v1
      idx = idx + 1
      for i=idx,#param do
        local v = param[i]
        if v and not out_option[v] and not options[v] then
          table.insert(ret.sproto_file, v)
          idx = idx + 1
        elseif idx > begin+1 then
          return idx
        else
          return false
        end
      end
    end
    return idx > begin+1 and idx or false
  end

  local idx = 1
  while idx <= #param do
    local v =  read_out_opt(idx) or read_opt(idx) or false
    if not v then 
      return false
    end

    assert(v ~= idx)
    idx = v
  end

  return ret
end


local ret = _parse_param(...)


local function _spb(param)
  local trunk_list = _2trunk_list(param.sproto_file)
  dump_spb(trunk_list, param)
end

local function _cs(param)
  local trunk_list = _2trunk_list(param.sproto_file)
  dump_cSharp(trunk_list, param)
end

local _func = {
  ["-cs"] = _cs,
  ["-spb"] = _spb,
}

if ret and _func[ret.dump_type] then
  _func[ret.dump_type](ret)
else
  print(README)
end
