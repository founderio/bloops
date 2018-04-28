using System;
using System.Collections.Generic;
using UnityEngine;

public class NoteGenerator : MonoBehaviour {
	public AudioClip[] allNotes;
	public int[] steps = { 3, 2, 2, 3, 2 };
	public Sprite[] sprites = new Sprite[6];

	public NoteTemplate[] GetNotes(int key, int count) {
		List<NoteTemplate> selected = new List<NoteTemplate>();

		int current = key;
		selected.Add(new NoteTemplate {
			clip = allNotes[key],
			sprite = sprites[0]
		});

		for (int i = 0; i < count - 1; i++) {
			int step = steps[i];
			current += step;
			selected.Add(new NoteTemplate {
				clip = allNotes[current],
				sprite = sprites[i + 1]
			});
		};

		return selected.ToArray();
	}
}
