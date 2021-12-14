﻿using System;
using System.Linq;
using AdventOfCode.Common;

namespace AdventOfCode.Puzzles.Day01;

public class Day1 : AdventDayBase
{
    private const string InputFile = "Day1/day1.txt";

    public Day1()
        : base(1)
    {
        AddPart(BuildPartOne());
        AddPart(BuildPartTwo());
    }

    public static AdventAssignment BuildPartOne()
    {
        return AdventAssignment.Build(
            InputFile,
            input => input.Split(Environment.NewLine).Select(int.Parse),
            data => data.Pairs().Count(x => x.Item1 < x.Item2));
    }

    public static AdventAssignment BuildPartTwo()
    {
        return AdventAssignment.Build(
            InputFile,
            input => input.Split(Environment.NewLine).Select(int.Parse),
            data => data
                .Triplets()
                .Select(x => x.Item1 + x.Item2 + x.Item3)
                .Pairs()
                .Count(x => x.Item1 < x.Item2)
        );
    }
}