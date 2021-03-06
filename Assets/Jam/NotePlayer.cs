using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class NotePlayer : MonoBehaviour {

	/*
		Config
	 */
	//public bool rememberNoteInputOffset = true;
	public bool playNoteOnInput = true;
	public float bpm = 100;
	public int noteCount = 16;
	public int playableNoteCount = 6;
	public int key = 9;
	public float graceZoneInNotes = 0.5f;
	public float delayInSeconds = 1.5f;
	public NoteGenerator instrumentPlayerOne;
	public NoteGenerator instrumentPlayerTwo;

	public string playerOne;
	public string playerTwo;

	public GameObject noteSlotPrefab;
	public GameObject circleEffectPrefab;
	public GameObject slotMarkerOne;
	public GameObject slotMarkerTwo;

	/*
		Runtime
	 */
	public NoteTemplate[] notesPlayerOne;
	public NoteTemplate[] notesPlayerTwo;
	public GameObject effectsContaner;
	public float tick;

	public int lastPlayedOne = 0;
	public int lastPlayedTwo = 0;
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
		notesPlayerOne = instrumentPlayerOne.GetNotes(key, playableNoteCount);
		notesPlayerTwo = instrumentPlayerTwo.GetNotes(key, playableNoteCount);

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
		// Delay until start
		if (delayInSeconds >= 0) {
			delayInSeconds -= Time.deltaTime;
			return;
		}

		// Move notes
		tick += Time.deltaTime;
		if (tick >= timePerNote) {
			tick -= timePerNote;
			NotePlay(true, true);
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

	private void NotePlay(bool one, bool two) {
		Note notePlayerOne = slotsPlayerOne[noteIndex];
		Note notePlayerTwo = slotsPlayerTwo[noteIndex];

		bool spawnEffect = false;
		if (one && lastPlayedOne != noteIndex) {

			if (notePlayerOne.clip != null) {
				source.PlayOneShot(notePlayerOne.clip);
				spawnEffect = true;
			}
			lastPlayedOne = noteIndex;
		}

		if (two && lastPlayedTwo != noteIndex) {

			if (notePlayerTwo.clip != null) {
				source.PlayOneShot(notePlayerTwo.clip);
				spawnEffect = true;
			}
			lastPlayedTwo = noteIndex;
		}
		if (spawnEffect)
			SpawnCircle();


	}

	void SpawnCircle() {
		GameObject effect = Instantiate(circleEffectPrefab);
		effect.transform.parent = effectsContaner.transform;
		effect.transform.position = transform.position;

	}

	private void CheckNoteInput() {
		Note notePlayerOne = slotsPlayerOne[noteIndex];
		Note notePlayerTwo = slotsPlayerTwo[noteIndex];

		float graceTime = (1 - graceZoneInNotes) * timePerNote;
		if (tick < graceTime) {
			return;
		}

		for (int i = 0; i < playableNoteCount; i++) {
			string codeOne = "p1_note" + i;
			string codeTwo = "p2_note" + i;

			if (notePlayerOne.enabled && Input.GetButtonDown(codeOne)) {
				notePlayerOne.spriteRenderer.sprite = notesPlayerOne[i].sprite;
				notePlayerOne.clip = notesPlayerOne[i].clip;

				Debug.Log(codeOne);

				if (playNoteOnInput) {
					NotePlay(true, false);
				}
			}
			if (notePlayerTwo.enabled && Input.GetButtonDown(codeTwo)) {
				notePlayerTwo.spriteRenderer.sprite = notesPlayerTwo[i].sprite;
				notePlayerTwo.clip = notesPlayerTwo[i].clip;

				Debug.Log(codeTwo);

				if (playNoteOnInput) {
					NotePlay(false, true);
				}
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
