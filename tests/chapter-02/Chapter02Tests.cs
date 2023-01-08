using FunctionalProgramming.Exercises.Chapter02;
using static FunctionalProgramming.Exercises.Chapter02.Program;

namespace FunctionalProgramming.Exercises.Tests;

public class Chapter02Tests
{
    [Theory]
    [InlineData(1.78, 58.3, 18.4)]
    [InlineData(1.70, 62.5, 21.63)]
    [InlineData(1.66, 82.7, 30.01)]
    public void CalculateBmi(double height, double weight, double expected)
    {
        double actual = Program.CalculateBMI(height, weight);
        Assert.Equal(expected, actual, 2);
    }

    [Theory]
    [InlineData(18.4, BmiRange.Underweight)]
    [InlineData(21.63, BmiRange.Healthy)]
    [InlineData(30.01, BmiRange.Overweight)]
    public void BmiToRange(double bmi, BmiRange expected)
    {
        var actual = Program.ToBmiRange(bmi);
        Assert.Equal(expected, actual);
    }
}