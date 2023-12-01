import os

# https://adventofcode.com/2023/day/1 - Day 1:

def digitToNum(s) -> int:
    match s:
        case "one" | "1":
            return 1
        case "two" | "2":
            return 2
        case "three" | "3":
            return 3
        case "four" | "4":
            return 4
        case "five" | "5":
            return 5
        case "six" | "6":
            return 6
        case "seven" | "7":
            return 7
        case "eight" | "8":
            return 8
        case "nine" | "9":
            return 9
        case _:
            return 0


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
        for digit in digits:
            if s[i:].startswith(digit):
                first = digitToNum(digit) * 10
                break
        if first > 0:
            break
        i += 1

    # Find last, scan backward
    i = len(s) - 1
    while i >= 0:
        for digit in digits:
            if s[i:].startswith(digit):
                last = digitToNum(digit)
                break
        if last >= 0:
            break
        i -= 1

    sum2 += first + last

print("Part 2:", sum2)
