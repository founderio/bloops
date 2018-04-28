using System;
using System.Collections.Generic;
using UnityEngine;

public class CircleEffect : MonoBehaviour {
	public GameObject outerCircle;
	public GameObject innerCircle;


	public float durationShow = .5f;

	public float duration = 1;
	public float tick;

	public float maxScale = 50;

	public float scaleOuter;
	public float scaleInner;

	public SpriteRenderer outerRenderer;
	public SpriteRenderer innerRenderer;


	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake() {
		outerRenderer = outerCircle.GetComponent<SpriteRenderer>();
		innerRenderer = innerCircle.GetComponent<SpriteRenderer>();
		outerCircle.transform.localScale = Vector3.zero;
		innerCircle.transform.localScale = Vector3.zero;
		outerCircle.SetActive(true);
		innerCircle.SetActive(true);
	}

	/// <summary>
	/// Update is called every frame, if the MonoBehaviour is enabled.
	/// </summary>
	void Update() {
		tick += Time.deltaTime;

		if (tick > durationShow) {
			transform.localScale = new Vector3(1, 1, 1);
		} else {
			float ssc = Mathf.Lerp(0, 1, tick / durationShow);
			transform.localScale = new Vector3(ssc, ssc, 1);
		}

		if (tick > duration) {
			Destroy(gameObject);
			return;
		}
		scaleOuter = outerCircle.transform.localScale.x;
		scaleInner = innerCircle.transform.localScale.x;

		Scale(outerCircle, scaleOuter);
		Scale(innerCircle, scaleInner);
	}

	void Scale(GameObject gameObject, float initialScale) {
		float scale = Mathf.Lerp(initialScale, maxScale, tick / duration);
		gameObject.transform.localScale = new Vector3(scale, scale, 1);
	}

	internal void Sort(int order) {
		innerRenderer.sortingOrder = order- 1;
		outerRenderer.sortingOrder = order;
	}
}
