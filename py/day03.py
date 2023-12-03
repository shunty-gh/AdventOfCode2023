
# https://adventofcode.com/2023/day/3

def cellsNearby(x: int, y: int, num: int) -> list[tuple[int,int]]:
    result = [(x-1,y), (x+len(str(num)),y)]
    for dx in range(-1, len(str(num))+1):
        result.append((x+dx,y-1))
        result.append((x+dx, y+1))
    return result

with open("../input/day03-input", "r") as f:
    input = [line.strip() for line in f.readlines()]

symbols = {}
numbers = []
for y,row in enumerate(input):
    x = 0
    while x < len(row):
        cell = row[x]

        if cell != '.' and not cell.isdigit():
            symbols[(x,y)] = (cell, [])
        elif cell.isdigit():
            n = cell
            x2 = x
            while x2 < len(row)-1:
                x2 += 1
                c2 = row[x2]
                if not c2.isdigit():
                    break
                n += c2
            numbers.append((x,y,int(n)))
            x += len(n)-1
        x += 1

p1,p2 = 0, 0
for (x,y,num) in numbers:
    mustadd = False
    for c in cellsNearby(x, y, num):
        if c in symbols:
            mustadd = True
            if symbols[c][0] == '*':
                symbols[c][1].append(num)
    if mustadd:
        p1 += num

for symbol in symbols.values():
    if symbol[0] == '*' and len(symbol[1]) == 2:
        p2 += symbol[1][0] * symbol[1][1]

print("Part 1:", p1)
print("Part 2:", p2)
