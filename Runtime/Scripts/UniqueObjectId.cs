using UnityEngine;
[ExecuteInEditMode]
public class UniqueObjectId : MonoBehaviour
{
    public string Id;
    // Start is called before the first frame update

    void Start(){
        Gen();
    }
    public void Gen()
    {
        if(string.IsNullOrEmpty(Id)) {
            Id = System.Guid.NewGuid().ToString();
        }
    }
}
