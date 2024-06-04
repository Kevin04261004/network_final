using UnityEngine;

public class SkyBoxHandler : MonoBehaviour
{
    [SerializeField] private Material _skyBox;
    private float _degree;
    [SerializeField] private float _rotateSpeed;
    private static readonly int Rotation = Shader.PropertyToID("_Rotation");

    void Start ()
    {
        _degree = 0;
    }
	
    void Update ()
    {
        _degree += Time.deltaTime * _rotateSpeed;
        if (_degree >= 360)
            _degree = 0;

        _skyBox.SetFloat(Rotation, _degree);
    }
}
