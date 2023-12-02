import os

# https://adventofcode.com/2023/day/2

def round_valid(round):
    return round[0] <= 12 and round[1] <= 13 and round[2] <= 14

def rounds_valid(rounds):
    return all([round_valid(round) for round in rounds])

def power(rounds):
    r = max([r for (r,_,_) in rounds])
    g = max([g for (_,g,_) in rounds])
    b = max([b for (_,_,b) in rounds])
    return r*g*b

with open(os.path.dirname(os.path.realpath(__file__)) + "/../input/day02-input", "r") as f:
    input = [line.strip() for line in f.readlines()]

games: list[int, ] = []
for line in input:
    (gid, game) = line.split(':')
    rounds = []
    for round in game.split(';'):
        for balls in round.strip().split(','):
            (n,r) = balls.strip().split(' ')
            num = int(n)
            red = num if r == "red" else 0
            green = num if r == "green" else 0
            blue = num if r == "blue" else 0
            rounds.append((red, green, blue))
    games.append((int(gid[5:]), rounds))


# Part 1
p1 = sum([game for (game,rounds) in games if rounds_valid(rounds)])
print("Part 1:", p1)

# Part 2
p2 = sum([power(rounds) for (_,rounds) in games])
print("Part 2:", p2)
