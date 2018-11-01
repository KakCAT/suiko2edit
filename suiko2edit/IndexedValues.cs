using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace suiko2edit
{

public struct NameAndAddressInfo
{
	public string name;
	public long offset;
}


//=============================================================================
/// <summary></summary>
class IndexedValues
{
long minAddress=0,maxAddress=0;			// marks min and max addresses for players, to load data from ISO
public List<NameAndAddressInfo> characters { get; private set; }

public int BlockDataLength { get; private set; }

byte []charInfo;						// data loaded from ISO


//=============================================================================
/// <summary></summary>
public IndexedValues (int blockSize)
{
	BlockDataLength=blockSize;
}

//=============================================================================
/// <summary>Expects a bunch of lines "Player Name" dash HexOffset </summary>
public void loadDataFromString (string str)
{
	characters=new List<NameAndAddressInfo>();

	var memoryStream=new MemoryStream (Tools.stringToBytes (str));

	using (StreamReader sr=new StreamReader (memoryStream))
	{
		while (!sr.EndOfStream)
		{
			string line=sr.ReadLine ();
			parseLine (line);
		}
	}

	characters.Sort ((p1,p2)=> p1.name.CompareTo (p2.name));
}

//=============================================================================
/// <summary></summary>
void parseLine (string line)
{
	if (String.IsNullOrWhiteSpace (line)) return;				// blank lines

	// line should be like  "character - offset"
	string []parts=line.Split ('-');
	if (parts.Length!=2) return;							

	// create and add character
	NameAndAddressInfo cInfo=new NameAndAddressInfo ();
	cInfo.name=parts[0].Trim();
	Int64.TryParse (parts[1].Trim(),NumberStyles.HexNumber,CultureInfo.CurrentCulture,out cInfo.offset);

	characters.Add (cInfo);

	// update offsets
	if (cInfo.offset<minAddress || minAddress==0) minAddress=cInfo.offset;
	if (cInfo.offset>maxAddress || maxAddress==0) maxAddress=cInfo.offset;
}



//=============================================================================
/// <summary>Loads data from ISO which should be character data</summary>
public void loadDataFromISO (string isoFile)
{
	long dataToLoad=maxAddress-minAddress+BlockDataLength;
	if (dataToLoad<BlockDataLength) { MessageBox.Show ("Weird error, character dada probably not loaded"); return; }
	if (dataToLoad>256*BlockDataLength) { MessageBox.Show ("Weird error, offsets are probably wrong"); return; }

	charInfo=Tools.readBlock (isoFile,minAddress,dataToLoad);
}

//=============================================================================
/// <summary></summary>
public byte[] getDataForCharacter (int nChar)
{
	int offset=(int)(characters[nChar].offset-minAddress);

	// copy a block of 12 bytes to a temporal array
	byte []data=new byte [BlockDataLength];
	Array.Copy (charInfo,offset,data,0,BlockDataLength);
	return data;
}

}
}
