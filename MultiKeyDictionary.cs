using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tjx.MultiKeyDictionary
{
    public static class Factory
    {
              
        public static IDictionary ToMultiKeyDictionary<TSource, TSecKey>(this IEnumerable<TSource> source, params Func<TSource, TSecKey>[] func)
        {
            Dictionary<int, Type> reverseCollection = new Dictionary<int, Type>();
            int functionsCount = func.Count();
            List<IDictionary> dictTypeCollection = new List<IDictionary>();
            object instanceOfCurrentDictionaryArgument = new object();
            object dictionaryArgQueue = null;
            List<Type> dictionaryTypes = new List<Type>();
            for (var i = 0; i <= functionsCount - 1; i++)
                reverseCollection.Add(i, source.Select(func[func.Count() - (i + 1)]).FirstOrDefault().GetType());
            Type typeOfGenericDictionary;
            Type[] dictionaryTypeArgs;
            Type newConstructedType;
            IDictionary currentDictionaryInstance;
            for (int i = 0; i <= reverseCollection.Count - 1; i++)
            {
                if (reverseCollection[i] != typeof(string))
                    instanceOfCurrentDictionaryArgument = Activator.CreateInstance(reverseCollection[i]);
                else
                    instanceOfCurrentDictionaryArgument = string.Empty;

                if (instanceOfCurrentDictionaryArgument != null && dictionaryArgQueue != null)
                {
                    typeOfGenericDictionary = typeof(Dictionary<,>);
                    dictionaryTypeArgs = new[] { instanceOfCurrentDictionaryArgument.GetType(), dictionaryArgQueue.GetType() };
                    newConstructedType = typeOfGenericDictionary.MakeGenericType(dictionaryTypeArgs);
                    currentDictionaryInstance = (IDictionary)Activator.CreateInstance(newConstructedType);
                    dictionaryTypes.Add(newConstructedType);
                    dictionaryArgQueue = currentDictionaryInstance;
                }
                else
                    dictionaryArgQueue = instanceOfCurrentDictionaryArgument;
            }
            IDictionary dict = (IDictionary)Activator.CreateInstance(dictionaryTypes.Last());
            // If dict IsNot Nothing Then
            object queueObj;
            foreach (var item in source)
            {
                var currentItem = item;

                bool itemAddedFlag = false;
                IEnumerable<TSource> currentItemInCollection;
                IDictionary iterDict = dict; // Reset dictionary
                for (int i = 0; i <= func.Count() - 1; i++)
                {
                    currentItemInCollection = new [] { currentItem };
              
                    TSecKey currentKey = currentItemInCollection.Select(func[i]).FirstOrDefault();
                    if (iterDict.Contains(currentKey) && iterDict[currentKey] != null)
                        iterDict = (IDictionary) iterDict[currentKey];
                    else
                    {
                        queueObj = null;
                        for (var k = 0; k <= reverseCollection.Count - 1; k++)
                        {
                            if (queueObj == null)
                                queueObj = currentItemInCollection.Select(func[functionsCount - (k + 1)]).FirstOrDefault();
                            else
                            {
                                object curObject = currentItemInCollection.Select(func[functionsCount - (k + 1)]).FirstOrDefault();
                                IDictionary currentDictionaryFromReverse = (IDictionary) Activator.CreateInstance(dictionaryTypes[k - 1]);
                                if (iterDict.GetType() == currentDictionaryFromReverse.GetType())
                                {
                                    iterDict.Add(curObject, queueObj);
                                    itemAddedFlag = true;
                                    break;
                                }
                                currentDictionaryFromReverse.Add(curObject, queueObj);
                                queueObj = currentDictionaryFromReverse;
                            }
                        }

                        if (itemAddedFlag)
                            break;
                    }
                }
            }

            return dict;
        }


    }
}
