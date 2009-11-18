using System;


/// <summary>
/// 
/// </summary>
public class FileUtil
{
	
	/// <summary>
    /// Retorna el file path del archivo pasado como par�metro
	/// </summary>
	/// <param name="strFilename"></param>
	/// <returns></returns>
	static public string GetFilePath(string strFilename)
	{
        int idx = strFilename.LastIndexOf("/");

        if (idx == -1)
            idx = strFilename.LastIndexOf("\\");

        if (idx == -1)
			return "";

        return strFilename.Substring(0, idx);

	}

	/// <summary>
    /// Retorna el filename sin el path
	/// </summary>
	/// <param name="strFilename"></param>
	/// <returns></returns>
	static public string GetFileName(string strFilename)
	{
        int idx = strFilename.LastIndexOf("/");

        if (idx == -1)
            idx = strFilename.LastIndexOf("\\");

        if (idx == -1)
			return strFilename;

        return strFilename.Substring(idx + 1, strFilename.Length - idx - 1);

	}

    /// <summary>
    /// Dado un nombre de archivo con extensi�n, se la saca y retorna 
    /// el string restante.
    /// </summary>
    /// <param name="strFilename"></param>
    /// <returns></returns>
    static public string ExtraceFileExt(string strFilename)
    {
        int idx = strFilename.LastIndexOf(".");

        if (idx != -1)
            return strFilename.Substring(0, idx);
        else
            return strFilename;
    }

	/// <summary>
    /// Cambia la terminaci�n de un archivo por la especificada
	/// </summary>
	/// <param name="strFilename"></param>
	/// <param name="strExt"></param>
	/// <returns></returns>
	static public string ChangeFileExt(string strFilename, string strExt)
	{
        int idx = strFilename.LastIndexOf('.');

        if (idx != 1)
            return strFilename.Substring(0, idx) + strExt;
		else
			return strFilename;
	}

	/// <summary>
    /// Retorna la terminaci�n del archivo
	/// </summary>
	/// <param name="strFilename"></param>
	/// <returns></returns>
	static public string GetFileExt(string strFilename)
	{
        int idx = strFilename.LastIndexOf('.');

        if (idx != 1)
            return strFilename.Substring(idx, strFilename.Length - idx);
		else
			return strFilename;
	}	
	
}

