using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Config))]
public class ConfigEditor : Editor {
	/// <summary>
	/// 具体实现
	/// </summary>

	FieldInfo[] fieldInfoArr;   //存放Config中的所有公共字段
	int fieldInfoLength;
	Type[] typeFieldArr;         //所有字段类型
	Config mScript;
	const string dynConfigPath = "dynConfig_model_";

	void OnEnable()
	{
		mScript = target as Config;

		//获取Config中的所有公共字段
		Type typeConfig = mScript.GetType();
		fieldInfoArr = typeConfig.GetFields();

		//元素个数获取一次就够了
		fieldInfoLength = fieldInfoArr.Length;

		//获取所有字段类型
		typeFieldArr = new Type[fieldInfoLength];
		for (int i = 0; i < fieldInfoLength; i++)
		{
			typeFieldArr[i] = fieldInfoArr[i].FieldType;
		}

		//等同mScript.mInfo = new Info();初始化配置表的对象
		for (int i = 0; i < fieldInfoLength; i++)
		{
			if (fieldInfoArr[i].GetValue(mScript) == null)
			{
				fieldInfoArr[i].SetValue(mScript, typeFieldArr[i].Assembly.CreateInstance(typeFieldArr[i].ToString()));
			}
		}		
	}
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		ImportAndExport(mScript);
	}

	public void ImportAndExport(Config rConfig)
	{
		//导入
		if (GUILayout.Button("Import"))
		{

			for (int i = 0; i < fieldInfoLength; i++)
			{
				//获取类型名，用作保存时的名字
				string rName = rConfig.name + "_" + typeFieldArr[i].ToString();
				string path = PathTool.textPath + dynConfigPath + rName;

				//检查文件是否存在
				if (!File.Exists("Assets/Resources/" + path + ".asset"))
				{
					Debug.Log("File Not Found !!!");
					return;
				}

				var info = Instantiate(Resources.Load(path));

				if (info != null)
				{
					//fieldInfoArr[i]虽然是rConfig对象中的一个字段，但是他只是元数据
					//所以要参数需要知道这个字段属于哪一个对象
					fieldInfoArr[i].SetValue(rConfig, info);
				}
			}
        }

		//导出
		if (GUILayout.Button("Export"))
		{
			for (int i = 0; i < fieldInfoLength; i++)
			{
				//导出路径需要先检查是否存在
				string assetPath = "Assets/Resources/" + PathTool.textPath;
				string rName = typeFieldArr[i].ToString();
				string resPath = assetPath + dynConfigPath + rConfig.name + "_" + rName + ".asset";

				if (!Directory.Exists(assetPath))
					Directory.CreateDirectory(assetPath);

				if (File.Exists(resPath))
				{
					AssetDatabase.DeleteAsset(resPath);
					AssetDatabase.SaveAssets();
				}

				
				var newScript = Instantiate(fieldInfoArr[i].GetValue(rConfig) as UnityEngine.Object);
				AssetDatabase.CreateAsset(newScript, resPath);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
		}
	}

// 	public void ImportAndExport(Config rConfig, string rName)
// 	{
// 		//导入
// 		if(GUILayout.Button("Import"))
// 		{
// 			Info getInfo = null;
// 			string path = PathTool.textPath + dynConfigPath + rName;
// 
// 			if (!File.Exists("Assets/Resources/" + path +".asset"))
// 			{
// 				Debug.Log("File Not Found !!!");
// 				return;
// 			}
// 
// 			//加载路径，自己定义一个位置
// 			getInfo = Resources.Load(path) as Info;
// 
// 			getInfo = (Info)Instantiate(getInfo);
// 
// 			if (getInfo != null)
// 			{
// 				rConfig.mInfo = getInfo;
// 			}
// 		}
// 
// 		//导出
// 		if (GUILayout.Button("Export"))
// 		{
// 			//导出路径需要先检查是否存在
// 			string assetPath = "Assets/Resources/" + PathTool.textPath;
// 			string resPath = assetPath + dynConfigPath + rName + ".asset";
// 
// 			if (!Directory.Exists(assetPath))
// 				Directory.CreateDirectory(assetPath);
// 
// 			if(File.Exists(resPath))
// 			{
// 				AssetDatabase.DeleteAsset(resPath);
// 				AssetDatabase.SaveAssets();
//             }
// 
// 			Info newScript = (Info)Instantiate(rConfig.mInfo);
// 			AssetDatabase.CreateAsset(newScript, resPath);
// 			AssetDatabase.SaveAssets();
// 			AssetDatabase.Refresh();
//         }
// 	}
}
