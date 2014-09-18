local lpeg = require "lpeg"
local bit32 = require "bit32"
local table = require "table"
local print_r = require "print_r"

local P = lpeg.P
local S = lpeg.S
local R = lpeg.R
local C = lpeg.C
local Ct = lpeg.Ct
local Cg = lpeg.Cg
local Cc = lpeg.Cc
local V = lpeg.V

local function count_lines(_,pos, parser_state)
	if parser_state.pos < pos then
		parser_state.line = parser_state.line + 1
		parser_state.pos = pos
	end
	return pos
end

local exception = lpeg.Cmt( lpeg.Carg(1) , function ( _ , pos, parser_state)
	error(string.format("syntax error at [%s] line (%d)", parser_state.file or "", parser_state.line))
	return pos
end)

local eof = P(-1)
local newline = lpeg.Cmt((P"\n" + "\r\n") * lpeg.Carg(1) ,count_lines)
local line_comment = "#" * (1 - newline) ^0 * (newline + eof)
local blank = S" \t" + newline + line_comment
local blank0 = blank ^ 0
local blanks = blank ^ 1
local alpha = R"az" + R"AZ" + "_"
local alnum = alpha + R"09"
local word = alpha * alnum ^ 0
local name = C(word)
local typename = C(word * ("." * word) ^ 0)
local tag = R"09" ^ 1 / tonumber

local function multipat(pat)
	return Ct(blank0 * (pat * blanks) ^ 0 * pat^0 * blank0)
end

local function namedpat(name, pat)
	return Ct(Cg(Cc(name), "type") * Cg(pat))
end

local typedef = P {
	"ALL",
	FIELD = namedpat("field", (name * blanks * tag * blank0 * ":" * blank0 * (C"*")^0 * typename)),
	STRUCT = P"{" * multipat(V"FIELD" + V"TYPE") * P"}",
	TYPE = namedpat("type", P"." * name * blank0 * V"STRUCT" ),
	SUBPROTO = Ct((C"request" + C"response") * blanks * (name + V"STRUCT")),
	PROTOCOL = namedpat("protocol", name * blanks * tag * blank0 * P"{" * multipat(V"SUBPROTO") * P"}"),
	ALL = multipat(V"TYPE" + V"PROTOCOL"),
}

local proto = blank0 * typedef * blank0

local convert = {}

function convert.protocol(all, obj)
	local result = { tag = obj[2] }
	for _, p in ipairs(obj[3]) do
		assert(result[p[1]] == nil)
		local typename = p[2]
		if type(typename) == "table" then
			local struct = typename
			typename = obj[1] .. "." .. p[1]
			all.type[typename] = convert.type(all, { typename, struct })
		end
		result[p[1]] = typename
	end
	return result
end

function convert.type(all, obj)
	local result = {}
	local typename = obj[1]
	local tags = {}
	local names = {}
	for _, f in ipairs(obj[2]) do
		if f.type == "field" then
			local name = f[1]
			if names[name] then
				error(string.format("redefine %s in type %s", name, typename))
			end
			names[name] = true
			local tag = f[2]
			if tags[tag] then
				error(string.format("redefine tag %d in type %s", tag, typename))
			end
			tags[tag] = true
			local field = { name = name, tag = tag }
			table.insert(result, field)
			local fieldtype = f[3]
			if fieldtype == "*" then
				field.array = true
				fieldtype = f[4]
			end
			field.typename = fieldtype
		else
			assert(f.type == "type")	-- nest type
			local nesttypename = typename .. "." .. f[1]
			f[1] = nesttypename
			assert(all.type[nesttypename] == nil, "redefined " .. nesttypename)
			all.type[nesttypename] = convert.type(all, f)
		end
	end
	table.sort(result, function(a,b) return a.tag < b.tag end)
	return result
end

local function adjust(r)
	local result = { type = {} , protocol = {} }

	for _, obj in ipairs(r) do
		local set = result[obj.type]
		local name = obj[1]
		assert(set[name] == nil , "redefined " .. name)
		set[name] = convert[obj.type](result,obj)
	end

	return result
end

local buildin_types = {
	integer = 0,
	boolean = 1,
	string = 2,
}

local function checktype(types, ptype, t)
	if buildin_types[t] then
		return t
	end
	local fullname = ptype .. "." .. t
	if types[fullname] then
		return fullname
	else
		ptype = ptype:match "(.+)%..+$"
		if ptype then
			return checktype(types, ptype, t)
		elseif types[t] then
			return t
		end
	end
end

local function flattypename(r)
	for typename, t in pairs(r.type) do
		for _, f in pairs(t) do
			local ftype = f.typename
			local fullname = checktype(r.type, typename, ftype)
			if fullname == nil then
				error(string.format("Undefined type %s in type %s", ftype, typename))
			end
			f.typename = fullname
		end
	end

	return r
end

local function parser(text,filename)
	local state = { file = filename, pos = 0, line = 1 }
	local r = lpeg.match(proto * -1 + exception , text , 1, state )
	return flattypename(adjust(r))
end


local function gen_ast(text, name)
	return parser(text, name or "=text")
end


return {
	buildin_types = buildin_types,
	gen_ast = gen_ast
}
