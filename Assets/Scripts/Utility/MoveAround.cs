using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAround : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Vector3> positions;
    public bool isMoveWith = true;

    [Tooltip("How much percent of distance it travels per second from initPos to endPos")]
    public float travelRate = 0.33f;

    Vector3 velocity;
    float speed;
    float dist = 0;

    int now = 0;
    int next = 0;
    void Start()
    {
        if (positions.Count == 0)
        {
            positions.Add(transform.position);
        }
        for (int i = 0; i < positions.Count; i++)
        {
            dist += (positions[i + 1 < positions.Count? i+1 : 0] - positions[i]).magnitude;
        }
        speed = dist * travelRate;
        if (positions.Count == 1)
        {
            velocity = Vector3.zero;
            return;
        }
        velocity = (positions[1] - positions[0]).normalized * speed;
        next = 1;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += velocity * Time.deltaTime;
        if ((transform.position - positions[next]).magnitude < 0.1f ||
            (transform.position - positions[now]).magnitude > (positions[next]-positions[now]).magnitude * 1.1f)
        {
            transform.position = positions[next];
            ChooseEnd();
            velocity = (positions[next] - positions[now]).normalized * speed;

        }

    }

    void ChooseEnd()
    {
        now = next;
        next += 1;
        if (next >= positions.Count)
        {
            next -= positions.Count;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!isMoveWith || !collision.gameObject.CompareTag("Player")) return;
        collision.gameObject.transform.SetParent(transform);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!isMoveWith || !collision.gameObject.CompareTag("Player")) return;
        collision.gameObject.transform.SetParent(null);
    }
}
