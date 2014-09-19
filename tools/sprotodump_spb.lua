local parse_core = require "sprotoparse_core"
local buildin_types = parse_core.buildin_types

local M = {}

--[[
-- The protocol of sproto
.type {
  .field {
    name 0 : string
    buildin 1 : integer
    type 2 : integer
    tag 3 : integer
    array 4 : boolean
  }
  name 0 : string
  fields 1 : *field
}

.protocol {
  name 0 : string
  tag 1 : integer
  request 2 : integer # index
  response 3 : integer # index
}

.group {
  type 0 : *type
  protocol 1 : *protocol
}
]]

local function packbytes(str)
  local size = #str
  return string.char(bit32.extract(size,0,8))..
    string.char(bit32.extract(size,8,8))..
    string.char(bit32.extract(size,16,8))..
    string.char(bit32.extract(size,24,8))..
    str
end

local function packvalue(id)
  id = (id + 1) * 2
  assert(id >=0 and id < 65536)
  return string.char(bit32.extract(id, 0, 8)) .. string.char(bit32.extract(id, 8, 8))
end

local function packfield(f)
  local strtbl = {}
  if f.array then
    table.insert(strtbl, "\5\0")  -- 5 fields
  else
    table.insert(strtbl, "\4\0")  -- 4 fields
  end
  table.insert(strtbl, "\0\0")  -- name (tag = 0, ref =0)
  if f.buildin then
    table.insert(strtbl, packvalue(f.buildin))  -- buildin (tag = 1)
    table.insert(strtbl, "\1\0")  -- skip (tag = 2)
    table.insert(strtbl, packvalue(f.tag))    -- tag (tag = 3)
  else
    table.insert(strtbl, "\1\0")  -- skip (tag = 1)
    table.insert(strtbl, packvalue(f.type))   -- type (tag = 2)
    table.insert(strtbl, packvalue(f.tag))    -- tag (tag = 3)
  end
  if f.array then
    table.insert(strtbl, packvalue(1))  -- array = true (tag = 4)
  end
  table.insert(strtbl, packbytes(f.name))
  return packbytes(table.concat(strtbl))
end

local function packtype(name, t, alltypes)
  local fields = {}
  local tmp = {}
  for _, f in ipairs(t) do
    tmp.array = f.array
    tmp.name = f.name
    tmp.tag = f.tag

    tmp.buildin = buildin_types[f.typename]
    if not tmp.buildin then
      tmp.type = assert(alltypes[f.typename])
    else
      tmp.type = nil
    end
    table.insert(fields, packfield(tmp))
  end
  local data
  if #fields == 0 then
    data = {
      "\1\0", -- 1 fields
      "\0\0", -- name (id = 0, ref = 0)
      packbytes(name),
    }
  else
    data = {
      "\2\0", -- 2 fields
      "\0\0", -- name (tag = 0, ref = 0)
      "\0\0", -- field[]  (tag = 1, ref = 1)
      packbytes(name),
      packbytes(table.concat(fields)),
    }
  end

  return packbytes(table.concat(data))
end

local function packproto(name, p, alltypes)
--  if p.request == nil then
--    error(string.format("Protocol %s need request", name))
--  end
  if p.request then
    local request = alltypes[p.request]
    if request == nil then
      error(string.format("Protocol %s request type %s not found", name, p.request))
    end
  end
  local tmp = {
    "\4\0", -- 4 fields
    "\0\0", -- name (id=0, ref=0)
    packvalue(p.tag), -- tag (tag=1)
  }
  if p.request == nil and p.response == nil then
    tmp[1] = "\2\0"
  else
    if p.request then
      table.insert(tmp, packvalue(alltypes[p.request])) -- request typename (tag=2)
    else
      table.insert(tmp, "\1\0")
    end
    if p.response then
      table.insert(tmp, packvalue(alltypes[p.response])) -- request typename (tag=3)
    else
      tmp[1] = "\3\0"
    end
  end

  table.insert(tmp, packbytes(name))

  return packbytes(table.concat(tmp))
end

local function packgroup(t,p)
  if next(t) == nil then
    assert(next(p) == nil)
    return "\0\0"
  end
  local tt, tp
  local alltypes = {}
  for name in pairs(t) do
    alltypes[name] = #alltypes
    table.insert(alltypes, name)
  end
  tt = {}
  for _,name in ipairs(alltypes) do
    table.insert(tt, packtype(name, t[name], alltypes))
  end
  tt = packbytes(table.concat(tt))
  if next(p) then
    local tmp = {}
    for name, tbl in pairs(p) do
      table.insert(tmp, tbl)
      tbl.name = name
    end
    table.sort(tmp, function(a,b) return a.tag < b.tag end)

    tp = {}
    for _, tbl in ipairs(tmp) do
      table.insert(tp, packproto(tbl.name, tbl, alltypes))
    end
    tp = packbytes(table.concat(tp))
  end
  local result
  if tp == nil then
    result = {
      "\1\0", -- 1 field
      "\0\0", -- type[] (id = 0, ref = 0)
      tt,
    }
  else
    result = {
      "\2\0", -- 2fields
      "\0\0", -- type array (id = 0, ref = 0)
      "\0\0", -- protocol array (id = 1, ref =1)

      tt,
      tp,
    }
  end

  return table.concat(result)
end

local function encodeall(r)
  return packgroup(r.type, r.protocol)
end

function M.dump(str)
  local tmp = ""
  for i=1,#str do
    tmp = tmp .. string.format("%02X ", string.byte(str,i))
    if i % 8 == 0 then
      if i % 16 == 0 then
        print(tmp)
        tmp = ""
      else
        tmp = tmp .. "- "
      end
    end
  end
  print(tmp)
end

function M.parse(text, name)
  local r = parse_core.gen_ast(text, name)
  local data = encodeall(r)
  return data
end


return M
