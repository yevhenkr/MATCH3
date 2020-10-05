using UnityEngine;
using System.Collections;

public class MovableTitle : MonoBehaviour {

	private GameTitle title;
	private IEnumerator moveCoroutine;

	void Awake() {
		title = GetComponent<GameTitle> ();
	}

	public void Move(int newX, int newY, float time)
	{
		if (moveCoroutine != null) {
			StopCoroutine (moveCoroutine);
		}

		moveCoroutine = MoveCoroutine (newX, newY, time);
		StartCoroutine (moveCoroutine);
	}

	public IEnumerator MoveCoroutine(int newX, int newY, float time)
	{
		title.X = newX;
		title.Y = newY;

		Vector3 startPos = transform.position;
		Vector3 endPos = title.GridRef.GetWorldPosition (newX, newY);

		for (float t = 0; t <= 1 * time; t += Time.deltaTime) {
			title.transform.position = Vector3.Lerp (startPos, endPos, t / time);
			yield return 0;
		}

		title.transform.position = title.GridRef.GetWorldPosition (newX, newY);
	}
}
