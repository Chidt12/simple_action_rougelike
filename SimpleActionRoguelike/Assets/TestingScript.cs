using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class TestingScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var namespaces = Assembly.GetExecutingAssembly().GetTypes()
                         .Select(t => t.Namespace)
                         .Distinct();

        int num = 0;

        foreach (var item in namespaces)
        {
            var classInNamespace = (from cal in Assembly.GetExecutingAssembly().GetTypes()
                    where cal.Namespace == item && (cal.IsClass)
                    select cal).ToList().Count();

            Debug.LogError($"{item} - {classInNamespace}");

            num += classInNamespace;
        }

        Debug.LogError("Num " + num);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
