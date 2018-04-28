using System;
using System.Collections.Generic;
using UnityEngine;

public class CircleEffect : MonoBehaviour {
	public float durationShow = .5f;

	public float duration = 1;
	public float tick;

	public float maxScale = 50;
	public float minScale = 1;
	public float minAlphaCutoff = 0.8f;

	public float scaleOuter;
	public float scaleInner;

	public SpriteRenderer outerRenderer;
	public SpriteMask mask;


	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake() {
		outerRenderer = GetComponent<SpriteRenderer>();
		mask = GetComponent<SpriteMask>();
		transform.localScale = Vector3.zero;
		tick = -durationShow;
	}

	/// <summary>
	/// Update is called every frame, if the MonoBehaviour is enabled.
	/// </summary>
	void Update() {
		tick += Time.deltaTime;

		if (tick < 0) {
			float ssc = Mathf.Lerp(0, 1, tick / durationShow);
			transform.localScale = new Vector3(ssc, ssc, 1);
			return;
		}

		if (tick > duration) {
			Destroy(gameObject);
			return;
		}
		scaleOuter = transform.localScale.x;

		float scale = Mathf.Lerp(minScale, maxScale, tick / duration);
		gameObject.transform.localScale = new Vector3(scale, scale, 1);
		mask.alphaCutoff = Mathf.Lerp(minAlphaCutoff, 1, tick / duration);
	}

}
