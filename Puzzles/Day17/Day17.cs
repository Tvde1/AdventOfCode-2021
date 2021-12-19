﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Common;
using AdventOfCode.Common.Models;

namespace AdventOfCode.Puzzles.Day17;

public class Day17 : AdventDay
{
    private const int MAX_MISS_COUNT = 512;

    private const string InputFile = "Day17/day17.txt";

    private const string TestInput = @"target area: x=20..30, y=-10..-5";

    public Day17()
        : base(17, AdventDayImplementation.Build(AdventDataSource.FromFile(InputFile), TargetArea.Parse, PartOne))
    { }

    private readonly record struct FiringError(bool IsHit, Vector2 Distance);

    private readonly record struct TargetArea(int MinX, int MaxX, int MinY, int MaxY)
    {
        public static TargetArea Parse(string input)
        {
            var r = new Regex(@"x=(\-?\d+)\.\.(\-?\d+), y=(\-?\d+)\.\.(-?\d+)");

            var match = r.Match(input);

            return new TargetArea(
                int.Parse(match.Groups[1].Value), 
                int.Parse(match.Groups[2].Value), 
                int.Parse(match.Groups[3].Value), 
                int.Parse(match.Groups[4].Value));
        }
    }

    private static string PartOne(TargetArea data)
    {
        var xPossibilities = Enumerable.Range(0, data.MaxX + 1).ToArray();
        var yPossibilities = Enumerable.Range(data.MinY, 1000).ToArray();

        var highestY = int.MinValue;
        var best = new Vector2(int.MinValue, int.MinValue);

        foreach (var x in xPossibilities)
            foreach (var y in yPossibilities)
            {
                var probe = new Probe(new Point2D(0, 0), new Vector2(x, y));

                var highestProbeY = probe.Point.Y;

                var missCount = 0;

                while (true)
                {
                    probe = CalculateProbeStep(probe);

                    if (probe.Point.Y > highestProbeY)
                    {
                        highestProbeY = probe.Point.Y;
                    }

                    var firingError = GetFiringError(data, probe.Point);

                    if (firingError.IsHit)
                    {
                        if (highestProbeY > highestY)
                        {
                            highestY = highestProbeY;
                            best = new Vector2(x, y);
                        }
                        break;
                    }

                    missCount++;

                    if (missCount > MAX_MISS_COUNT)
                    {
                        break;
                    }
                }
            }

        return highestY.ToString();
    }

    private readonly record struct Probe(Point2D Point, Vector2 Velocity);

    private static Probe CalculateProbeStep(Probe probe)
    {
        var newX = probe.Point.X + probe.Velocity.X;
        var newY = probe.Point.Y + probe.Velocity.Y;

        var newXVelocity = probe.Velocity.X switch
        {
            > 0 => probe.Velocity.X - 1,
            < 0 => probe.Velocity.X + 1,
            0 => 0,
        };
        var newYVelocity = probe.Velocity.Y - 1;

        return new Probe(new Point2D(newX, newY), new Vector2(newXVelocity, newYVelocity));
    }

    private static FiringError GetFiringError(TargetArea targetArea,Point2D target)
    {
        var isInX = target.X >= targetArea.MinX && target.X <= targetArea.MaxX;
        var isInY = target.Y >= targetArea.MinY && target.Y <= targetArea.MaxY;

        if (isInX && isInY)
        {
            return new FiringError(true, new Vector2(0, 0));
        }

        var toLeftOfX = targetArea.MinX - target.X;
        var rightOfX = target.X - targetArea.MaxX;

        var aboveY = target.Y - targetArea.MaxY;
        var belowY = targetArea.MinY - target.Y;

        int firingErrorX;
        if (toLeftOfX > 0)
        {
            firingErrorX = -toLeftOfX;
        }
        else
        {
            firingErrorX = rightOfX;
        }

        int firingErrorY;
        if (belowY > 0)
        {
            firingErrorY = -belowY;
        }
        else
        {
            firingErrorY = aboveY;
        }

        return new FiringError(false, new Vector2(firingErrorX, firingErrorY));
    }
}