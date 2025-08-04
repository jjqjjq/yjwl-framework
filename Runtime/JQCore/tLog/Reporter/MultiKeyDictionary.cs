using System.Collections.Generic;

public class MultiKeyDictionary<T1, T2, T3> : Dictionary<T1, Dictionary<T2, T3>>
{
    public new Dictionary<T2, T3> this[T1 key]
    {
        get
        {
            if (!ContainsKey(key))
                Add(key, new Dictionary<T2, T3>());

            Dictionary<T2, T3> returnObj;
            TryGetValue(key, out returnObj);

            return returnObj;
        }
    }

    public bool ContainsKey(T1 key1, T2 key2)
    {
        Dictionary<T2, T3> returnObj;
        TryGetValue(key1, out returnObj);
        if (returnObj == null)
            return false;

        return returnObj.ContainsKey(key2);
    }

    public void TryGetMultiValue(T1 key1, T2 key2, out T3 val)
    {
        Dictionary<T2, T3> returnObj;
        TryGetValue(key1, out returnObj);
        if (returnObj == null)
        {
            val = default;
            return;
        }

        T3 outVal;
        returnObj.TryGetValue(key2, out outVal);
        if (outVal != null)
            val = outVal;
        else
            val = default;
    }

    public void MultiKeyClear()
    {
        foreach (var keyValuePair in this)
        {
            var dictionary = keyValuePair.Value;
            dictionary.Clear();
        }

        Clear();
    }
}