using UnityEngine;

public class GoodsGravity : MonoBehaviour
{
    public float DownForce = 50f;
    private Rigidbody _goodRigidbody;

    void Start()
    {
        _goodRigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // z축으로 떨어지게
        _goodRigidbody.AddForce(new Vector3(0, 0, DownForce), ForceMode.Acceleration);
    }
}