﻿using System.IO;
using System.Text;
using BRM.DebugAdapter.Interfaces;
using BRM.FileSerializers.Interfaces;
using BRM.DataSerializers.Interfaces;

namespace BRM.FileSerializers
{
    /// <summary>
    /// Serializes/Deserializes text for disk files
    /// </summary>
    public sealed class TextFileSerializer : IReadFiles, IWriteFiles
    {
        private ISerializeText _serializer;
        private IDebug _debugger;

        public Encoding Encoding = Encoding.UTF8;

        /// <summary>
        /// textHandler must match the document type
        /// eg: jsonHandlers must be used for Reading/Writing .json files
        /// eg: xmlHandlers must be used for Reading/Writing .xml files
        /// </summary>
        public TextFileSerializer(ISerializeText serializer, IDebug debugger)
        {
            _serializer = serializer;
            _debugger = debugger;
        }

        public TModel Read<TModel>(string filePath)
        {
            if (!IsValid(filePath, "read"))
            {
                return default;
            }
            var model = default(TModel);
            using (var stream = new FileStream(filePath, FileMode.Open))
            using (var reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();
                model = _serializer.AsObject<TModel>(json);
            }
            return model;
        }

        public void Write<TModel>(string filePath, TModel model)
        {
            if (!IsValid(filePath, "write"))
            {
                return;
            }

            FileUtilities.SafeCreateDirectory(filePath);
            using (var stream = new FileStream(filePath, FileMode.Create))
            using (var writer = new StreamWriter(stream, Encoding))
            {
                string json = _serializer.AsString(model);
                writer.Write(json);
            }
        }

        private bool IsValid(string filePath, string operation)
        {
            string extension = Path.GetExtension(filePath);
            bool isValid = FileValidator.IsValidExtensionForType(_serializer.SerializationType, extension);
            if (!isValid)
            {
                bool isExtensionNull = string.IsNullOrEmpty(extension);
                if (isExtensionNull)
                {
                    _debugger.LogWarningFormat("FileExtension is null");
                }
                else
                {
                    _debugger.LogWarningFormat("You may be attempting to {0} a file not supported by the member TextHandler. Type{1}, FileExtension:{2}", operation, _serializer.SerializationType, extension);
                }
            }
            return isValid;
        }
    }
}