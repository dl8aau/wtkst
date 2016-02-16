using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace wtKST.Properties
{
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0"), CompilerGenerated]
	public sealed class Settings : ApplicationSettingsBase
	{
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());

		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}

		[DefaultSettingValue("www.on4kst.org"), UserScopedSetting, DebuggerNonUserCode]
		public string KST_ServerName
		{
			get
			{
				return (string)this["KST_ServerName"];
			}
			set
			{
				this["KST_ServerName"] = value;
			}
		}

		[DefaultSettingValue("23000"), UserScopedSetting, DebuggerNonUserCode]
		public string KST_ServerPort
		{
			get
			{
				return (string)this["KST_ServerPort"];
			}
			set
			{
				this["KST_ServerPort"] = value;
			}
		}

		[DefaultSettingValue(""), UserScopedSetting, DebuggerNonUserCode]
		public string KST_UserName
		{
			get
			{
				return (string)this["KST_UserName"];
			}
			set
			{
				this["KST_UserName"] = value;
			}
		}

		[DefaultSettingValue(""), UserScopedSetting, DebuggerNonUserCode]
		public string KST_Password
		{
			get
			{
				return (string)this["KST_Password"];
			}
			set
			{
				this["KST_Password"] = value;
			}
		}

		[DefaultSettingValue("3 - Microwave"), UserScopedSetting, DebuggerNonUserCode]
		public string KST_Chat
		{
			get
			{
				return (string)this["KST_Chat"];
			}
			set
			{
				this["KST_Chat"] = value;
			}
		}

		[DefaultSettingValue("False"), UserScopedSetting, DebuggerNonUserCode]
		public bool KST_AutoConnect
		{
			get
			{
				return (bool)this["KST_AutoConnect"];
			}
			set
			{
				this["KST_AutoConnect"] = value;
			}
		}

		[DefaultSettingValue("False"), UserScopedSetting, DebuggerNonUserCode]
		public bool KST_StartAsHere
		{
			get
			{
				return (bool)this["KST_StartAsHere"];
			}
			set
			{
				this["KST_StartAsHere"] = value;
			}
		}

		[DefaultSettingValue("True"), UserScopedSetting, DebuggerNonUserCode]
		public bool KST_StartAsAway
		{
			get
			{
				return (bool)this["KST_StartAsAway"];
			}
			set
			{
				this["KST_StartAsAway"] = value;
			}
		}

		[DefaultSettingValue("C:\\ProgramData\\Win-Test\\extras\\qrv.xdt"), UserScopedSetting, DebuggerNonUserCode]
		public string WinTest_QRV_FileName
		{
			get
			{
				return (string)this["WinTest_QRV_FileName"];
			}
			set
			{
				this["WinTest_QRV_FileName"] = value;
			}
		}

		[DefaultSettingValue("C:\\ProgramData\\Win-Test\\cfg\\wt.ini"), UserScopedSetting, DebuggerNonUserCode]
		public string WinTest_INI_FileName
		{
			get
			{
				return (string)this["WinTest_INI_FileName"];
			}
			set
			{
				this["WinTest_INI_FileName"] = value;
			}
		}

		[DefaultSettingValue(""), UserScopedSetting, DebuggerNonUserCode]
		public string WinTest_FileName
		{
			get
			{
				return (string)this["WinTest_FileName"];
			}
			set
			{
				this["WinTest_FileName"] = value;
			}
		}

		[DefaultSettingValue("qrv.xml"), UserScopedSetting, DebuggerNonUserCode]
		public string WinTest_QRV_Table_FileName
		{
			get
			{
				return (string)this["WinTest_QRV_Table_FileName"];
			}
			set
			{
				this["WinTest_QRV_Table_FileName"] = value;
			}
		}

		[DefaultSettingValue("60"), UserScopedSetting, DebuggerNonUserCode]
		public string UpdateInterval
		{
			get
			{
				return (string)this["UpdateInterval"];
			}
			set
			{
				this["UpdateInterval"] = value;
			}
		}

		[DefaultSettingValue("True"), UserScopedSetting, DebuggerNonUserCode]
		public bool KST_ShowBalloon
		{
			get
			{
				return (bool)this["KST_ShowBalloon"];
			}
			set
			{
				this["KST_ShowBalloon"] = value;
			}
		}

		[DefaultSettingValue("20000"), UserScopedSetting, DebuggerNonUserCode]
		public string AS_Planes_MinAlt
		{
			get
			{
				return (string)this["AS_Planes_MinAlt"];
			}
			set
			{
				this["AS_Planes_MinAlt"] = value;
			}
		}

		[DefaultSettingValue("300"), UserScopedSetting, DebuggerNonUserCode]
		public string AS_MinDist
		{
			get
			{
				return (string)this["AS_MinDist"];
			}
			set
			{
				this["AS_MinDist"] = value;
			}
		}

		[DefaultSettingValue("1000"), UserScopedSetting, DebuggerNonUserCode]
		public string AS_MaxDist
		{
			get
			{
				return (string)this["AS_MaxDist"];
			}
			set
			{
				this["AS_MaxDist"] = value;
			}
		}

		[DefaultSettingValue("50"), UserScopedSetting, DebuggerNonUserCode]
		public string AS_SCPRadius
		{
			get
			{
				return (string)this["AS_SCPRadius"];
			}
			set
			{
				this["AS_SCPRadius"] = value;
			}
		}

		[DefaultSettingValue("False"), UserScopedSetting, DebuggerNonUserCode]
		public bool AS_Active
		{
			get
			{
				return (bool)this["AS_Active"];
			}
			set
			{
				this["AS_Active"] = value;
			}
		}

		[DefaultSettingValue("200"), UserScopedSetting, DebuggerNonUserCode]
		public string AS_PlaneRadius
		{
			get
			{
				return (string)this["AS_PlaneRadius"];
			}
			set
			{
				this["AS_PlaneRadius"] = value;
			}
		}

		[DefaultSettingValue("False"), UserScopedSetting, DebuggerNonUserCode]
		public bool ShowBeacons
		{
			get
			{
				return (bool)this["ShowBeacons"];
			}
			set
			{
				this["ShowBeacons"] = value;
			}
		}

		[DefaultSettingValue("beacons.lst"), UserScopedSetting, DebuggerNonUserCode]
		public string BeaconFileName
		{
			get
			{
				return (string)this["BeaconFileName"];
			}
			set
			{
				this["BeaconFileName"] = value;
			}
		}

		[DefaultSettingValue("9871"), UserScopedSetting, DebuggerNonUserCode]
		public int WinTest_Port
		{
			get
			{
				return (int)this["WinTest_Port"];
			}
			set
			{
				this["WinTest_Port"] = value;
			}
		}

		[DefaultSettingValue("9872"), UserScopedSetting, DebuggerNonUserCode]
		public int AS_Port
		{
			get
			{
				return (int)this["AS_Port"];
			}
			set
			{
				this["AS_Port"] = value;
			}
		}

		[DefaultSettingValue(""), UserScopedSetting, DebuggerNonUserCode]
		public string KST_Loc
		{
			get
			{
				return (string)this["KST_Loc"];
			}
			set
			{
				this["KST_Loc"] = value;
			}
		}

		[DefaultSettingValue(""), UserScopedSetting, DebuggerNonUserCode]
		public string KST_Name
		{
			get
			{
				return (string)this["KST_Name"];
			}
			set
			{
				this["KST_Name"] = value;
			}
		}

		[DefaultSettingValue("False"), UserScopedSetting, DebuggerNonUserCode]
		public bool WinTest_Activate
		{
			get
			{
				return (bool)this["WinTest_Activate"];
			}
			set
			{
				this["WinTest_Activate"] = value;
			}
		}

		[DefaultSettingValue("10"), UserScopedSetting, DebuggerNonUserCode]
		public string AS_Timeout
		{
			get
			{
				return (string)this["AS_Timeout"];
			}
			set
			{
				this["AS_Timeout"] = value;
			}
		}

		[DefaultSettingValue("KST"), UserScopedSetting, DebuggerNonUserCode]
		public string AS_My_Name
		{
			get
			{
				return (string)this["AS_My_Name"];
			}
			set
			{
				this["AS_My_Name"] = value;
			}
		}

		[DefaultSettingValue("AS"), UserScopedSetting, DebuggerNonUserCode]
		public string AS_Server_Name
		{
			get
			{
				return (string)this["AS_Server_Name"];
			}
			set
			{
				this["AS_Server_Name"] = value;
			}
		}

		[DefaultSettingValue("1.2G"), UserScopedSetting, DebuggerNonUserCode]
		public string AS_QRG
		{
			get
			{
				return (string)this["AS_QRG"];
			}
			set
			{
				this["AS_QRG"] = value;
			}
		}

		private void SettingChangingEventHandler(object sender, SettingChangingEventArgs e)
		{
		}

		private void SettingsSavingEventHandler(object sender, CancelEventArgs e)
		{
		}
	}
}
