using UnityEngine;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System;

public class HandleJson {

	private static  HandleJson Instance;
	public static HandleJson GetInstance()
	{
		if(Instance==null)
		{
			Instance = new HandleJson();
		}
		return Instance;
	}

	//write to file
	//fileName: the file name with path
	//attribute: the attribute name
	//valueStr: write the value to the file
	public void WriteToFileXml(string fileName, string attribute, string valueStr)
	{
		string filepath = Application.dataPath + "/" + fileName;
		#if UNITY_ANDROID
		filepath = Application.persistentDataPath + "//" + fileName;
		#endif
		
		//create file
		if(!File.Exists (filepath))
		{
			XmlDocument xmlDoc = new XmlDocument();
			XmlElement root = xmlDoc.CreateElement("transforms");
			XmlElement elmNew = xmlDoc.CreateElement("attribute");
			
			root.AppendChild(elmNew);
			xmlDoc.AppendChild(root);
			xmlDoc.Save(filepath);
			File.SetAttributes(filepath, FileAttributes.Normal);
		}
		
		//update value
		if(File.Exists (filepath))
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(filepath);
			XmlNodeList nodeList=xmlDoc.SelectSingleNode("transforms").ChildNodes;
			
			foreach(XmlElement xe in nodeList)
			{
				xe.SetAttribute(attribute, valueStr);
			}
			File.SetAttributes(filepath, FileAttributes.Normal);
			xmlDoc.Save(filepath);
		}
	}

	
	public void WriteToFilePathXml(string filepath, string attribute, string valueStr)
	{
		//string filepath = Application.dataPath + "/" + fileName;
		#if UNITY_ANDROID
//		filepath = Application.persistentDataPath + "//" + fileName;
		#endif
		
		//create file
		if(!File.Exists (filepath))
		{
			XmlDocument xmlDoc = new XmlDocument();
			XmlElement root = xmlDoc.CreateElement("transforms");
			XmlElement elmNew = xmlDoc.CreateElement("attribute");
			
			root.AppendChild(elmNew);
			xmlDoc.AppendChild(root);
			xmlDoc.Save(filepath);
			File.SetAttributes(filepath, FileAttributes.Normal);
		}
		
		//update value
		if(File.Exists (filepath))
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(filepath);
			XmlNodeList nodeList=xmlDoc.SelectSingleNode("transforms").ChildNodes;
			
			foreach(XmlElement xe in nodeList)
			{
				xe.SetAttribute(attribute, valueStr);
			}
			File.SetAttributes(filepath, FileAttributes.Normal);
			xmlDoc.Save(filepath);
		}
	}
	
	//read information according to the attribute
	//return the string type value
	//int aaa = int.Parse(valueStr);
	//int.TryParse(valueStr, out aaa);
	public string ReadFromFileXml(string fileName, string attribute)
	{
		string filepath = Application.dataPath + "/" + fileName;
		#if UNITY_ANDROID
		filepath = Application.persistentDataPath + "//" + fileName;
		#endif
		string valueStr = null;
		if(File.Exists (filepath))
		{
			try
			{
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.Load(filepath);
				XmlNodeList nodeList=xmlDoc.SelectSingleNode("transforms").ChildNodes;
				foreach(XmlElement xe in nodeList)
				{
					valueStr = xe.GetAttribute(attribute);
				}
				File.SetAttributes(filepath, FileAttributes.Normal);
				xmlDoc.Save(filepath);
			}			
			catch (Exception exception)
			{
				File.SetAttributes(filepath, FileAttributes.Normal);
				File.Delete(filepath);
				UnityEngine.Debug.LogError("error: xml was wrong! " + exception);
			}
		}
		return valueStr;
	}

	public string ReadFromFilePathXml(string filepath, string attribute)
	{
		//string filepath = Application.dataPath + "/" + fileName;
		#if UNITY_ANDROID
		//filepath = Application.persistentDataPath + "//" + fileName;
		#endif
		string valueStr = null;
		if(File.Exists (filepath))
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(filepath);
			XmlNodeList nodeList=xmlDoc.SelectSingleNode("transforms").ChildNodes;
			foreach(XmlElement xe in nodeList)
			{
				valueStr = xe.GetAttribute(attribute);
			}
			File.SetAttributes(filepath, FileAttributes.Normal);
			xmlDoc.Save(filepath);
		}
		
		return valueStr;
	}

	#region handle ini file
	[DllImport("kernel32.dll")]
	public extern static int GetPrivateProfileStringA(string segName, string keyName, string sDefault, byte[] buffer, int iLen, string fileName);
	[DllImport("kernel32.dll")]
	public extern static int WritePrivateProfileString(string segName, string keyName, string sValue, string fileName);
	#endregion
}
