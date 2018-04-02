using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Collections.LowLevel.Unsafe; 

namespace Unity.Collections
{
    unsafe public static class NativeArrayEx
    {
#if CSHARP_7_OR_LATER
        public static ref T GetItemRef<T>(this NativeArray<T> array, int index) where T : struct
        {
            return ref UnsafeUtilityEx.ArrayElementAsRef<T>(array.GetUnsafeReadOnlyPtr(), index);
        }
#endif

    }
}

public class TestRef : MonoBehaviour {

	// Use this for initialization
    void Start () {
        NativeArray<SructA> array = new NativeArray<SructA>(99999999, Allocator.Temp);

        PerformanceTestJobParallel testJob=new PerformanceTestJobParallel(){array=array} ;

        float timeElpase = Time.realtimeSinceStartup;
        var fence = testJob.Schedule<PerformanceTestJobParallel>(array.Length, 64);
        fence.Complete();

        print(Time.realtimeSinceStartup - timeElpase); 
        array.Dispose();

        array = new NativeArray<SructA>(99999999, Allocator.Temp);

        PerformanceTestJob testJob2 = new PerformanceTestJob() { array = array };

        timeElpase = Time.realtimeSinceStartup;
        var fence2 = testJob2.Schedule();
        fence2.Complete();

        print(Time.realtimeSinceStartup - timeElpase);
        array.Dispose();
	}  
}

public struct SructA
{
    public int i;
}

//[ComputeJobOptimization]
public struct PerformanceTestJobParallel:IJobParallelFor
{  
    public NativeArray<SructA> array;

    public void Execute(int index)
    {
        ref SructA aaa = ref array.GetItemRef(index);
        //AAA aaa = ar[index];
        aaa.i = index;
        //ar[index] = aaa;
    }
}

public struct PerformanceTestJob : IJob
{
    public NativeArray<SructA> array;

    public void Execute()
    {
        for (int i = 0; i < array.Length; i++)
        {
            ref SructA aaa = ref array.GetItemRef(i);
            //AAA aaa = ar[i];
            aaa.i = i;
            //ar[i] = aaa;
        }
       
    }
}
