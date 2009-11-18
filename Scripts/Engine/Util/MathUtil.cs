// Mono Framework
using System;

public class MathUtil
{
    public static void Swap(ref float n1, ref float n2)
    {
        float aux = n2;
        n2 = n1;
        n1 = aux;
    }

}
