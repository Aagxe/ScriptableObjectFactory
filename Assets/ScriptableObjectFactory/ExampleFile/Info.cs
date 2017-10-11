using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 测试文件
/// </summary>
public class Info : ScriptableObject
{

	//必须设置为公有，这样检视面板才能自动生成并可见
	public Info() { }

	public float width;
	public float height;
	public GameObject shpare;

}