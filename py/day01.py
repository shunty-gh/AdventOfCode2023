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
digits = ["one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"]
sum2 = 0
for s in input:
    i = 0
    first = -1
    last = -1
    # Find first, scan forward
    while i < len(s):
        for idx, digit in enumerate(digits):
            if s[i:].startswith(digit):
                first = 10 * (idx + 1 if len(digit) > 1 else int(digit))
                break
        if first > 0:
            break
        i += 1

    # Find last, scan backward
    i = len(s) - 1
    while i >= 0:
        for idx, digit in enumerate(digits):
            if s[i:].startswith(digit):
                last = idx + 1 if len(digit) > 1 else int(digit)
                break
        if last >= 0:
            break
        i -= 1

    sum2 += first + last

print("Part 2:", sum2)
