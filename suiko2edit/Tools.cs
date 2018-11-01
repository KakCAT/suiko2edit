using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace suiko2edit
{
static class Tools
{

//=============================================================================
/// <summary></summary>
public static string bytesToString (byte []data)
{
	if (data==null) return null;
	return System.Text.Encoding.UTF8.GetString (data,0,data.Length);
}


//=============================================================================
/// \brief 
public static byte[] stringToBytes (string str)
{
	if (str==null) return null;
	return System.Text.Encoding.UTF8.GetBytes (str);
}


//=============================================================================
/// <summary>Reads a file block</summary>
public static byte[] readBlock (string fileName,long offset,long length)
{
	try
	{
		using (FileStream fs=new FileStream(fileName,FileMode.Open,FileAccess.Read))
		{
			fs.Seek (offset,SeekOrigin.Begin);
			byte []data=new byte [length];
			fs.Read (data,0,(int)length);
			return data;
		}

	}
	catch (Exception e)
	{
		MessageBox.Show ("Exception reading ISO : "+e.Message);
	}
	return null;
}

}
}
