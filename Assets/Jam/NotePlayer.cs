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

	public string playerOne;
	public string playerTwo;

	public GameObject noteSlotPrefab;
	public GameObject circleEffectPrefab;
	public GameObject slotMarkerOne;
	public GameObject slotMarkerTwo;

	/*
		Runtime
	 */
	public GameObject effectsContaner;
	public float tick;

	public int lastPlayed = 0;
	public int noteIndex;

	public float timePerNote;
	public List<Note> slotsPlayerOne;
	public List<Note> slotsPlayerTwo;

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

		SpawnNotes(ref slotsPlayerOne, playerOne, slotMarkerOne);
		SpawnNotes(ref slotsPlayerTwo, playerTwo, slotMarkerTwo);
	}

	void SpawnNotes(ref List<Note> slots, string playerTrack, GameObject noteMarker) {
		slots = new List<Note>();
		for (int i = 0; i < noteCount; i++) {
			Note n = new Note();

			char active = '1';
			if (i < playerTrack.Length) {
				active = playerTrack[i];
			}

			n.enabled = active == '1';

			if (n.enabled) {
				n.obj = GameObject.Instantiate(noteSlotPrefab);
				PositionSlot(i, n.obj, noteMarker);
				n.spriteRenderer = n.obj.GetComponent<SpriteRenderer>();

			}

			slots.Add(n);
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

		CheckNoteInput();

		if (Input.GetKeyDown(KeyCode.C)) {
			SpawnCircle();
		}
	}

	private void NotePlay() {
		if (lastPlayed == noteIndex) {
			return;
		}

		Note notePlayerOne = slotsPlayerOne[noteIndex];
		Note notePlayerTwo = slotsPlayerTwo[noteIndex];

		bool spawnEffect = false;
		if (notePlayerOne.clip != null) {
			source.PlayOneShot(notePlayerOne.clip);
			spawnEffect = true;
		}
		if (notePlayerTwo.clip != null) {
			source.PlayOneShot(notePlayerTwo.clip);
			spawnEffect = true;
		}
		if (spawnEffect)
			SpawnCircle();
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

	private void CheckNoteInput() {
		Note notePlayerOne = slotsPlayerOne[noteIndex];
		Note notePlayerTwo = slotsPlayerTwo[noteIndex];

		float graceTime = (1 - graceZoneInNotes) * timePerNote;
		if (tick < graceTime) {
			return;
		}

		for (int i = 0; i < notes.Length; i++) {
			string codeOne = "p1_note" + i;
			string codeTwo = "p2_note" + i;

			if (notePlayerOne.enabled && Input.GetButtonDown(codeOne)) {
				notePlayerOne.spriteRenderer.sprite = notes[i].sprite;
				notePlayerOne.clip = notes[i].clip;
			}
			if (notePlayerTwo.enabled && Input.GetButtonDown(codeTwo)) {
				notePlayerTwo.spriteRenderer.sprite = notes[i].sprite;
				notePlayerTwo.clip = notes[i].clip;
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

	void PositionSlot(int index, GameObject go, GameObject noteMarker) {
		float angle = NoteAngle(index);

		go.transform.parent = transform;
		go.transform.position = noteMarker.transform.position;
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
	internal bool enabled;
}
