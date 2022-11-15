using UnityEngine;
using System.Collections;

[CreateAssetMenu()]
public class TerrainData : UpdatableData
{

	public float uniformScale = 2.5f;
	public bool useFlatShading;
	public bool useFalloff;
	public bool useGradient;
	public float worldSize;
	public MRange[] mRanges;




	public float meshHeightMultiplier;
	public AnimationCurve meshHeightCurve;

	public float minHeight
	{
		get
		{
			return uniformScale * meshHeightMultiplier * meshHeightCurve.Evaluate(0);
		}
	}

	public float maxHeight
	{
		get
		{
			return uniformScale * meshHeightMultiplier * meshHeightCurve.Evaluate(1);
		}
	}
}
[System.Serializable]
public class MRange
{
	public float rangeSize;
	public float originX;
	public float originY;
	public bool indent;
}