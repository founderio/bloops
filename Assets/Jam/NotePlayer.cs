using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class NotePlayer : MonoBehaviour {

	/*
		Config
	 */
	public float bpm = 100;
	public int noteCount = 16;
	public float graceZoneInNotes = 0.5f;
	public NoteTemplate[] notes = new NoteTemplate[6];
	public GameObject noteSlotPrefab;
	public GameObject circleEffectPrefab;
	public GameObject slotMarker;

	/*
		Runtime
	 */
	public GameObject effectsContaner;
	public float tick;

	public int lastPlayed = 0;
	public int noteIndex;

	public float timePerNote;
	public List<Note> noteSlots;

	private AudioSource source;


	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	void Start() {

		if (effectsContaner == null) {
			effectsContaner = new GameObject("Effects Container");
			effectsContaner.transform.parent = transform;
		}

		source = GetComponent<AudioSource>();

		timePerNote = 60f / bpm;

		noteSlots = new List<Note>();
		for (int i = 0; i < noteCount; i++) {
			Note n = new Note();

			n.obj = GameObject.Instantiate(noteSlotPrefab);
			PositionSlot(i, n.obj);
			n.spriteRenderer = n.obj.GetComponent<SpriteRenderer>();

			noteSlots.Add(n);
		}
	}

	/// <summary>
	/// Update is called every frame, if the MonoBehaviour is enabled.
	/// </summary>
	void Update() {
		tick += Time.deltaTime;
		if (tick >= timePerNote) {
			tick -= timePerNote;
			NotePlay();
			noteIndex++;
			if (noteIndex >= noteCount) {
				noteIndex = 0;
			}
		}
		float singleAngle = 360f / noteCount;

		float currentAngle = noteIndex * singleAngle + (tick / timePerNote) * singleAngle;

		gameObject.transform.rotation = Quaternion.AngleAxis(-currentAngle, Camera.main.transform.forward);

		Note n = noteSlots[noteIndex];

		CheckNoteInput(n);

		if (Input.GetKeyDown(KeyCode.C)) {
			SpawnCircle();
		}
	}

	private void NotePlay() {
		if (lastPlayed == noteIndex) {
			return;
		}
		Note n = noteSlots[noteIndex];
		if (n.clip != null) {
			source.PlayOneShot(n.clip);
			SpawnCircle();
		}
		lastPlayed = noteIndex;
	}

	void SpawnCircle() {
		/*GameObject effect = Instantiate(circleEffectPrefab);
		effect.transform.parent = effectsContaner.transform;
		effect.transform.localPosition = Vector3.zero;

		ReorderSprites();*/
	}

	void ReorderSprites() {
		Transform[] child = this.GetComponentsInChildren<Transform>();
		int order = child.Length + 5;
		foreach (Transform trans in child) {
			if (trans != trans.root) //bcos root object just contains collider and control scripts
			{
				CircleEffect rnd = trans.GetComponent<CircleEffect>();
				if (rnd != null) {
					rnd.Sort(order);
					order += 2;
				}
			}
		}
	}

	private void CheckNoteInput(Note n) {
		float graceTime = (1 - graceZoneInNotes) * timePerNote;
		if (tick < graceTime) {
			return;
		}
		KeyCode first = KeyCode.Alpha0;

		for (int i = 0; i < notes.Length; i++) {
			KeyCode kc = first + i;
			if (Input.GetKeyDown(kc)) {
				n.spriteRenderer.sprite = notes[i].sprite;
				n.clip = notes[i].clip;
			}
		}
	}

	float NoteAngle(int index) {
		float single = 360f / noteCount;
		float angle = single * index;
		// Rotate one note, as the current one is what we are working with
		// it has to move into position, not out of it
		return angle + single;
	}

	void PositionSlot(int index, GameObject go) {
		float angle = NoteAngle(index);

		go.transform.parent = transform;
		go.transform.position = slotMarker.transform.position;
		go.transform.RotateAround(gameObject.transform.position, Camera.main.transform.forward, angle);
	}
}

[Serializable]
public class NoteTemplate {
	public AudioClip clip;
	public Sprite sprite;
}

[Serializable]
public class Note {
	public AudioClip clip;
	public GameObject obj;
	public SpriteRenderer spriteRenderer;
}
