using UnityEngine;
using System.Collections;

public class ObjectMover : MonoBehaviour {
    public Points points;

    private int _pointIndex = 0;
    private Vector3 _target;
    private GameObject _gameObject;
    private Transform _transform;

    void Start()
    {
        _gameObject = gameObject;
        _transform = transform;
        _pointIndex = 0;
        _target = points.cords[1];
    }

	void Update () {
        if (points.smoothTurn)
        {
            var relativePos = _target - _transform.position;
            var rotation = Quaternion.LookRotation(relativePos);
            _transform.localRotation = Quaternion.Lerp(_transform.rotation, rotation, Time.deltaTime * points.speed * 0.25f);
        }
        else
            _transform.LookAt(_target);
        _transform.position = Vector3.MoveTowards(_transform.position, _target, Time.deltaTime * points.speed);

        if ((_target - _transform.position).magnitude > points.radius)
        {
            return;
        }
        else
        {
            ChangePoint();
        }
	}

    private void ChangePoint()
    {
        if (_pointIndex < points.cords.Length - 1)
        {
            _pointIndex++;
        }
        else
        {
            _pointIndex = 0;
            if (!points.loop)
                points.RespawnObject(_gameObject);
        }
        _target = points.cords[_pointIndex];
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 5.5f);
    }
}
