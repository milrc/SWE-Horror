# save the aseprite palette as hex (save palette > File type: 'hex files' > 'palette.hex')
# put the palette in the art/shaders/resources folder (does not appear in godot for some reason)
# then just run this script to update the palette.gdshaderinc
# pass palette name inline or default to palette.hex

import sys

#read hex and add all unique, non empty lines
filename = "palette.hex"
if len(sys.argv) > 1:
    filename = sys.argv[1]

palette = {'ffffff'}
with open(filename, "r") as file:
    for line in file.readlines():
        line = line.strip()
        if len(line) > 0:
            palette.add(line)

#remove black, and white
palette -= {'000000', 'ffffff'}

palette_ordered = list(palette)
palette_ordered.append('ffffff') #add white back in at the end

#print palette backwards
output = f"const vec3 COLOR_PALETTE[{len(palette_ordered)}] = vec3[](\n"
for c in range(len(palette_ordered) - 1, -1, -1):
    color = palette_ordered[c]
    output += "    vec3("
    for i in range(0, 3):
        output += str(int(color[(i*2):(i*2)+2], 16) / 255.0) # convert hex to dec and divide by 255
        if i < 2:
            output += ", "
    output += ")"
    if c > 0:
        output += "," #only put comma if not last
    output += f" // #{color}\n" # comment actual hex code

output += ");\n"

with open("../includes/palette.gdshaderinc", "w", encoding="utf-8") as file:
    file.write(output)
