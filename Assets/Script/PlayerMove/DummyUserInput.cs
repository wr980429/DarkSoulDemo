using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyUserInput : UserInputBase
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        while (true)
        {
            isMouse1 = true;
            yield return 0;
        }     
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDmagDvec(Dup,Dright);
    }
}
