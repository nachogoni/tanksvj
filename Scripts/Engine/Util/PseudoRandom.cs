// Mono Framework
using System;

public class PseudoRandom
{
	private static Random _ran;
	
	public static float GetNextSingle()
	{
		if (_ran == null)
			_ran = new Random();
		
		return (float) _ran.NextDouble();
	}

    public static int GetNextInt(int maxBound)
    {
        if (_ran == null)
            _ran = new Random();

        return _ran.Next(maxBound);
    }

    public static int GetNextInt(int minBound, int maxBound)
    {
        if (_ran == null)
            _ran = new Random();

        return _ran.Next(minBound, maxBound);
    }

}
