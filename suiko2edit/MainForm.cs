using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace suiko2edit
{
public partial class MainForm : Form
{
string isoFileName;
IndexedValues charData;
IndexedValues weaponData;
int selectedCharacter=-1,selectedWeapon=-1;
NumericUpDown []weaponNumericUpDown;

//=============================================================================
/// <summary></summary>
public MainForm()
{
	InitializeComponent();

	// load data
	charData=new IndexedValues(0x12);
	charData.loadDataFromString (UnfilteredAddressData.PlayerOffsetsString);

	weaponData=new IndexedValues(0x10);
	weaponData.loadDataFromString (UnfilteredAddressData.WeaponTypeListString);
	

	// fill stat growth comboboxes
	fillStatGrowthComboBox (m_strGr);
	fillStatGrowthComboBox (m_magGr);
	fillStatGrowthComboBox (m_protGr);
	fillStatGrowthComboBox (m_mdfGr);
	fillStatGrowthComboBox (m_techGr);
	fillStatGrowthComboBox (m_spdGr);
	fillStatGrowthComboBox (m_luckGr);
	fillHpGrowthComboBox (m_hpGr);

	fillRuneAffinity (m_fireAff);
	fillRuneAffinity (m_waterAff);
	fillRuneAffinity (m_windAff);
	fillRuneAffinity (m_earthAff);
	fillRuneAffinity (m_lightningAff);
	fillRuneAffinity (m_resurrAff);
	fillRuneAffinity (m_darkAff);
	fillRuneAffinity (m_brightAff);

	generateWeaponUI ();

}

//=============================================================================
/// <summary></summary>
void fillStatGrowthComboBox (ComboBox cb)
{
	cb.Items.Clear();

	// ranks as described in http://suikosource.com/phpBB3/viewtopic.php?f=9&p=159542&sid=187be27182c79ea61b08df9d9aebff6f#p159542
	// and mixed with stats named at http://www.suikosource.com/games/gs2/guides/statgrowth.php
	cb.Items.AddRange (new List<string>() {
		"(0) E",
		"(1) D",
		"(2) D+",
		"(3) C",
		"(4) C+",
		"(5) B",
		"(6) B+",
		"(7) A+",
		"(8) S",
		"(9) early ....",
		"(A) early 1-20 big increase, decrease after 20",
		"(B) ?B",
		"(C) ?C",
		"(D) Sigfried",
		"(E) Later, big increase after L60",
		"(F) Abizboah"
		
		}.ToArray());
}


//=============================================================================
/// <summary></summary>
void fillHpGrowthComboBox (ComboBox cb)
{
	cb.Items.Clear();

	// ranks as described in http://suikosource.com/phpBB3/viewtopic.php?f=9&p=159542&sid=187be27182c79ea61b08df9d9aebff6f#p159542
	// and mixed with stats named at http://www.suikosource.com/games/gs2/guides/statgrowth.php
	cb.Items.AddRange (new List<string>() {
		"(0) F",
		"(1) E",
		"(2) D+",
		"(3) C",
		"(4) C+",
		"(5) B",
		"(6) B+",
		"(7) A+",
		"(8) S",
		"(9) early ....",
		"(A) early 1-20 big increase, decrease after 20",
		"(B) ?B",
		"(C) ?C",
		"(D) Sigfried",
		"(E) Later, big increase after L60",
		"(F) Abizboah"
		
		}.ToArray());
}




//=============================================================================
/// <summary></summary>
void fillRuneAffinity (ComboBox cb)
{
	cb.Items.Clear();

	// ranks as described in http://suikosource.com/phpBB3/viewtopic.php?f=9&p=159542&sid=187be27182c79ea61b08df9d9aebff6f#p159542
	// mixed with http://www.suikosource.com/games/gs2/guides/affinities.php
	cb.Items.AddRange (new List<string>() {
		"(0) F - None ",
		"(1) A - 40% more dmg",
		"(2) B - 20% more dmg",
		"(3) C - Normal dmg ",
		"(4) D - 20% less dmg",
		"(5) B - 20 more dmg, may backfire",
		"(6) ?",
		"(7) ?",
		"(8) ?",
		"(9) ?",
		"(A) ?",
		"(B) ?",
		"(C) ?",
		"(D) ?",
		"(E) ?",
		"(F) ?",
		
		}.ToArray());
}

//=============================================================================
/// <summary></summary>
private void MainForm_Shown(object sender, EventArgs e)
{
	populateCharacterNames();
	populateWeaponTypes();

	// open file on startup
	openISOToolStripMenuItem_Click(sender,e);
}

//=============================================================================
/// <summary>Fills the list of characters</summary>
void populateCharacterNames ()
{
	m_characterName.Items.Clear();

	foreach (var it in charData.characters)
	{
		m_characterName.Items.Add (it.name);
	}
}

//=============================================================================
/// <summary>Fills the character stats</summary>
void populateCharacterStats ()
{
	if (selectedCharacter==-1) return;

	var rawData=charData.getDataForCharacter (selectedCharacter);

	m_strGr.SelectedIndex=		high (rawData[0]);
	m_magGr.SelectedIndex=		low (rawData[0]);
	m_protGr.SelectedIndex=		high (rawData[1]);
	m_mdfGr.SelectedIndex=		low (rawData[1]);
	m_techGr.SelectedIndex=		high (rawData[2]);
	m_spdGr.SelectedIndex=		low (rawData[2]);
	m_luckGr.SelectedIndex=		high (rawData[3]);
	m_hpGr.SelectedIndex=		low (rawData[3]);

	m_fireAff.SelectedIndex=	high (rawData[8]);
	m_waterAff.SelectedIndex=	low (rawData[8]);
	m_windAff.SelectedIndex=	high (rawData[9]);
	m_earthAff.SelectedIndex=	low (rawData[9]);
	m_lightningAff.SelectedIndex=	high (rawData[10]);
	m_resurrAff.SelectedIndex=	low (rawData[10]);
	m_darkAff.SelectedIndex=	high (rawData[11]);
	m_brightAff.SelectedIndex=	low (rawData[11]);

	m_headLev.Value=rawData[0xe];
	m_rhLev.Value=rawData[0xf];
	m_lhLev.Value=rawData[0x10];

	// build raw data
	string str="Raw data:\r\n";
	for (int i=0;i<charData.BlockDataLength;i++)
	{
		str+=String.Format ("{0} - {1}      (0x{1:X2})\r\n",i,rawData[i]);
	}
	m_rawData.Text=str;

int low (byte val) { return val&0xf; }
int high (byte val) { return (val>>4)&0xf; }
}


//=============================================================================
/// <summary></summary>
private void openISOToolStripMenuItem_Click(object sender, EventArgs e)
{
	OpenFileDialog ofd=new OpenFileDialog ();
	ofd.Title="Open Suikoden 2 USA iso";
	ofd.Filter="*.iso|*.iso";
	if (ofd.ShowDialog ()!=DialogResult.OK) return;
	
	isoFileName=ofd.FileName;

	if (!checkIsoIsUSASuiko2(isoFileName)) 
	{
		MessageBox.Show ("Can only work with suikoden 2 USA.");
		isoFileName=null;
	}

	charData.loadDataFromISO (isoFileName);
	weaponData.loadDataFromISO (isoFileName);
}

//=============================================================================
/// <summary>Checks if fileName points to a USA Suikoden2 file</summary>
bool checkIsoIsUSASuiko2 (string fileName)
{
	// read a part of the ISO. 
	byte []isoData=Tools.readBlock (fileName,0xcaed,13);

	//This part contains "SLUS_009.58;1" in my copy, which should identify Suikoden 2 USA
	if (Tools.bytesToString (isoData)!="SLUS_009.58;1") return false;

	// further checks if needed.

	// OK, it's suikoden 2 USA
	return true;
}

//=============================================================================
/// <summary></summary>
private void m_characterName_SelectedIndexChanged(object sender, EventArgs e)
{
	selectedCharacter=m_characterName.SelectedIndex;
	populateCharacterStats();
}

//=============================================================================
/// <summary></summary>
private void m_weaponList_SelectedIndexChanged(object sender, EventArgs e)
{
	selectedWeapon=m_weaponList.SelectedIndex;
	populateWeaponStats();
}


//=============================================================================
/// <summary>Fills the list of characters</summary>
void populateWeaponTypes ()
{
	m_weaponList.Items.Clear();

	foreach (var it in weaponData.characters)
	{
		m_weaponList.Items.Add (it.name);
	}
}

//=============================================================================
/// <summary>Fills the character stats</summary>
void populateWeaponStats ()
{
int i;

	if (selectedWeapon==-1) return;

	var rawData=weaponData.getDataForCharacter (selectedWeapon);
	for (i=0;i<weaponNumericUpDown.Length;i++)
	{
		weaponNumericUpDown[i].Value=rawData[i];
	}

}

//=============================================================================
/// <summary></summary>
void generateWeaponUI ()
{
int i;
int x,y;
const int WeaponLevels=16;

	weaponNumericUpDown=new NumericUpDown[WeaponLevels];
	for (i=0;i<WeaponLevels;i++)
	{
		x=16+(i/8)*128;
		y=40+(i%8)*32;

		Label label=new Label();
		label.Text=$"Lev{i+1}";
		label.Location=new Point (x,y);
		label.AutoSize=true;
		m_weaponTab.Controls.Add (label);

		NumericUpDown num=new NumericUpDown ();
		num.Location=new Point (x+42,y-2);
		num.Size=new Size (40,20);
		num.Minimum=0;
		num.Maximum=255;
		m_weaponTab.Controls.Add (num);

		weaponNumericUpDown[i]=num;
	}
}
}
}
