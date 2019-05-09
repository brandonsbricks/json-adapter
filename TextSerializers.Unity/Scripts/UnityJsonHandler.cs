﻿using BRM.TextSerializers.Interfaces;
using UnityEngine;

namespace BRM.TextSerializers
{
    public class UnityJsonHandler : ISerializeText
    {
        public T AsObject<T>(string json)
        {
            return JsonUtility.FromJson<T>(json);
        }

        public string AsString<T>(T serializableInstance, bool prettyPrint = true)
        {
            return JsonUtility.ToJson(serializableInstance, prettyPrint);
        }
        public SerializationType SerializationType => SerializationType.Json;
    }
}
