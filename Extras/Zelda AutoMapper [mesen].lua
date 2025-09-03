
--default settings
color_visited = 0xFFFF00
color_current = 0x00FF00
X = 16
Y = 24
rooms = {}



local function DrawMinimap()
    if emu.read(0x0010, emu.memType.nesMemory, false) == 0x00 and       --overworld check
       emu.read(0x00E1, emu.memType.nesMemory, false) == 0x00 then      --pause check
        --loop through 128 rooms
        for i = 1, 128 do
            if rooms[i] ~= nil then
                --calculate square position
                local row = (i - 1) // 16
                local column = (i - 1) % 16
                local posX = X + column * 4
                local posY = Y + row * 4
                --draw 4x4 square
                if emu.read(0x00EB, emu.memType.nesMemory, false) == i - 1 then
                    emu.drawRectangle(posX, posY, 4, 4, color_current, true)
                else
                    emu.drawRectangle(posX, posY, 4, 4, color_visited, true)
                end
            end
        end
    end
end



local function Main() 
    if emu.read(0x0010, emu.memType.nesMemory, false) == 0x00 and       --overworld check
       emu.read(0x0012, emu.memType.nesMemory, false) == 0x05 then      --gameplay script check
        --flag room as visited
        rooms[emu.read(0x00EB, emu.memType.nesMemory, false) + 1] = 1
    end
	
    DrawMinimap()
end




emu.addEventCallback(Main, emu.eventType.startFrame)

