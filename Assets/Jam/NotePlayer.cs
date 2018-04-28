using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class NotePlayer : MonoBehaviour {

	public float speed = 45;
	public int noteCount = 16;
	public float graceZone = 360f / 32;

	public float currentAngle = 0;

	public int lastPlayed = 0;

	public NoteTemplate[] notes = new NoteTemplate[6];

	public List<Note> noteSlots;

	public GameObject noteSlotTemplate;
	public GameObject slotMarker;

	private AudioSource source;

	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	void Start() {
		source = GetComponent<AudioSource>();

		noteSlots = new List<Note>();
		for (int i = 0; i < noteCount; i++) {
			Note n = new Note();

			n.obj = GameObject.Instantiate(noteSlotTemplate);
			PositionSlot(i, n.obj);
			n.spriteRenderer = n.obj.GetComponent<SpriteRenderer>();

			noteSlots.Add(n);
		}
	}

	/// <summary>
	/// Update is called every frame, if the MonoBehaviour is enabled.
	/// </summary>
	void Update() {
		currentAngle += Time.deltaTime * speed;
		if (currentAngle >= 360) {
			currentAngle -= 360;
			lastPlayed = 0;
		}

		gameObject.transform.rotation = Quaternion.AngleAxis(-currentAngle, Camera.main.transform.forward);

		int index = 0;
		Note n = GetCurrentNote(out index);
		float angle = NoteAngle(index);

		if (n != null) {
			CheckNoteInput(index, angle, n);

			CheckNotePlay(index, angle, n);
		}
	}

	private void CheckNotePlay(int index, float angle, Note n) {
		if (lastPlayed == index) {
			return;
		}
		if (currentAngle > angle) {
			if (n.clip != null)
				source.PlayOneShot(n.clip);
			lastPlayed = index;
		}
	}

	private void CheckNoteInput(int index, float angle, Note n) {
		KeyCode first = KeyCode.Alpha0;

		for (int i = 0; i < notes.Length; i++) {
			KeyCode kc = first + i;
			if (Input.GetKeyDown(kc)) {
				n.spriteRenderer.sprite = notes[i].sprite;
				n.clip = notes[i].clip;
			}
		}

	}

	Note GetCurrentNote(out int index) {
		for (int i = 0; i < noteCount; i++) {
			float angle = NoteAngle(i);
			if (currentAngle - graceZone < angle &&
			currentAngle + graceZone > angle) {
				index = i;
				return noteSlots[i];
			}
		}
		index = -1;
		return null;
	}

	float NoteAngle(int index) {
		float single = 360f / noteCount;
		float angle = single * index;
		return angle;
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
