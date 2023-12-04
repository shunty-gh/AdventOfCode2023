from collections import defaultdict

# https://adventofcode.com/2023/day/4

with open("../input/day04-input", "r") as f:
    input = [line.strip() for line in f.readlines()]

p1 = 0
game_counts = defaultdict(int)
for idx,line in enumerate(input):
    game_counts[idx] += 1
    win, hand = line.split(':')[1].split('|')
    winning_numbers = [int(n) for n in win.split()]
    inhand_numbers = [int(n) for n in hand.split()]
    num_wins = len(set(winning_numbers) & set(inhand_numbers))
    p1 += num_wins if num_wins <= 1 else 1 << (num_wins-1)

    # P2
    for dx in range(num_wins):
        cidx = idx + 1 + dx
        game_counts[cidx] += game_counts[idx]

print("Part 1:", p1)

# It turns out we don't need to check for card index
# beyond the last card id. I didn't read the problem description
# properly first time round:
# "(Cards will never make you copy a card past the end of the table.)""
#maxCardId = len(input)
#p2 = sum([gc for id,gc in game_counts.items() if id <= maxCardId])
p2 = sum(game_counts.values())
print("Part 2:", p2)
