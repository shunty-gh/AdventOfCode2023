import os

# https://adventofcode.com/2023/day/1

with open(os.path.dirname(os.path.realpath(__file__)) + "/../input/day01-input", "r") as f:
    input = [line.strip() for line in f.readlines()]

# Part 1
sum1 = 0
for s in input:
    nums = [int(c) for c in s if c >= '0' and c <= '9']
    sum1 += (nums[0] * 10) + nums[-1]

print("Part 1:", sum1)

# Part 2
digits = ["one", "two", "three", "four", "five", "six", "seven", "eight", "nine"]
sum2 = 0
for s in input:
    nums = []
    i = 0
    for i,c in enumerate(s):
        if c.isdigit():
            nums.append(int(c))
        else:
            for idx, digit in enumerate(digits):
                if s[i:].startswith(digit):
                    nums.append(idx + 1)
                    break
    sum2 += nums[0] * 10 + nums[-1]

print("Part 2:", sum2)
