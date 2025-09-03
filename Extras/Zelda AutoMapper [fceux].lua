--Simple colors: "clear", "red", "green", "blue", "white", "black", "gray", "grey", "orange", "yellow", "green", "teal", "cyan", "purple", "magenta".
--default settings
color_visited = "yellow"
color_current = "green"
X = 16
Y = 24
rooms = {}



local function DrawMinimap()
    if memory.readbyte(0x0010) == 0x00 and      --overworld check
       memory.readbyte(0x00E1) == 0x00 then     --pause check
        --loop through 128 rooms
        for i = 1, 128 do
            if rooms[i] ~= nil then
                --calculate square position
                local row = string.format("%d", (i - 1) / 16)
                local column = string.format("%d", (i - 1) % 16)
                local posX = X + column * 4
                local posY = Y + row * 4
                --draw 4x4 square
                if memory.readbyte(0x00EB) == i - 1 then
                    gui.drawbox(posX, posY, posX + 3, posY + 3, color_current, color_current)
                else
                    gui.drawbox(posX, posY, posX + 3, posY + 3, color_visited, color_visited)
                end
            end
        end
    end
end



while true do        
    if memory.readbyte(0x0010) == 0x00 and      --overworld check   
       memory.readbyte(0x0012) == 0x05 then     --gameplay script check
        --flag room as visited
        rooms[memory.readbyte(0x00EB) + 1] = 1
    end
    
    DrawMinimap()
    
    emu.frameadvance()
end

