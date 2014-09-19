local spb = require "sprotodump_spb"
local cSharp = require "sprotodump_cSharp"

local README = [[
usage: lua sprotodump.lua [option] <sproto_file>  <outfile_name>

  option: 
    -cs              dump to cSharp code file, is default
    -spb             dump to binary spb  file]]

local function base_name(string_, suffix)
  local LUA_DIRSEP = string.sub(package.config,1,1)
  string_ = string_ or ''
  local basename = string.gsub (string_, '[^'.. LUA_DIRSEP ..']*'.. LUA_DIRSEP ..'', '')
  if suffix then
    basename = string.gsub (basename, suffix, '')
  end
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
end


local function dump_spb(sproto_file, outfile_name)
  local text = read_file(sproto_file)
  local data = spb.parse(text, sproto_file)
  write_file(outfile_name, data, "wb")
end

local function dump_cSharp(sproto_file, outfile_name)
  local text = read_file(sproto_file)
  local namespace = base_name(outfile_name, ".cs")
  local data = cSharp.parse(text, sproto_file, namespace)
  write_file(outfile_name, data, "w")
end


local option, sproto_file, outfile_name = ...

local args = {...}
if #args == 2 then
  option = "-cs"
  sproto_file = args[1]
  outfile_name = args[2]
end


if option == "-spb" then
  dump_spb(sproto_file, outfile_name)
elseif option == "-cs" then
  dump_cSharp(sproto_file, outfile_name)
else
  print(README)
end
