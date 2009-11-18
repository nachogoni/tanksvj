// Mono Framework
using System;


class StringUtil
{
    /// <summary>
    /// Convierte un numero hex en formato string a numero
    /// Ej: "0x00FF00FF" -> 0x00FF00FF
    /// �
    /// Ej: "00FF00FF" -> 0x00FF00FF
    /// </summary>
    /// <param name="strHexNum"></param>
    /// <returns></returns>
    public static int ConvertToHex(string strHexNum)
    {
        uint value = 0;
        int startIndex = 0;
        char ch;
        int weight = 0;

        if (strHexNum.StartsWith("0x"))
            startIndex += 2;

        for (int i = strHexNum.Length - 1; i >= startIndex; i--)
        {
            ch = Char.ToLower(strHexNum[i]);

            if (Char.IsDigit(ch))
                value += (uint)(ch - 48) * (uint)Math.Pow(16, weight);
            else if (ch >= 'a' && ch <= 'f')
                value += (uint)(ch - 87) * (uint)Math.Pow(16, weight);

            weight++;
        }

        unchecked
        {
            return (int)value;
        }
                    
    }
}

