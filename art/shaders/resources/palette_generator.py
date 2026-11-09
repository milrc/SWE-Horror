# save the aseprite palette as hex (save palette > File type: 'hex files' > 'palette.hex')
# put the palette in the art/shaders/resources folder (does not appear in godot for some reason)
# then just run this script to update the palette.gdshaderinc

#read hex and add all unique, non empty lines
palette = {'FFFFFF'}
with open("palette.hex", "r") as file:
    for line in file.readlines():
        line = line.strip()
        if len(line) > 0:
            palette.add(line)

#remove black
palette -= {'000000'}

output = f"const vec3 COLOR_PALETTE[{len(palette)}] = vec3[](\n"

for c in range(len(palette)):
    color = list(palette)[c]
    output += "    vec3("
    for i in range(0, 3):
        output += str(int(color[(i*2):(i*2)+2], 16) / 255.0)
        if i < 2:
            output += ", "
    output += ")"
    if c < len(palette) - 1:
        output += "," #only put comma if not last
    output += f" // #{color}\n"

output += "\n);\n"

with open("../includes/palette.gdshaderinc", "w", encoding="utf-8") as file:
    file.write(output)
