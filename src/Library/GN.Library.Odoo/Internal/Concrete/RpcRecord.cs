﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CookComputing.XmlRpc;

namespace GN.Library.Odoo.Internal.Concrete
{
    public class RpcRecord
    {
        private readonly RpcConnection _rpcConnection;
        private readonly string _model;
        private readonly List<RpcField> _fieldsResult;
        private int _id = -1;

        public int Id { get => this._id; set => this._id = value; }

        

        public RpcRecord(RpcConnection rpcConnection, string model, int? id, IEnumerable<RpcField> fieldsTemplate,
            XmlRpcStruct vals = null)
        {
            _model = model;
            _rpcConnection = rpcConnection;
            if (id == null)
            {
                _id = -1;
            }
            else
            {
                _id = (int) id;
            }

            if (id != null)
            {
                _fieldsResult = new List<RpcField>();
                foreach (var rpcField in fieldsTemplate)
                {
                    _fieldsResult.Add(new RpcField
                    {
                        FieldName = rpcField.FieldName,
                        Type = rpcField.Type,
                        String = rpcField.String,
                        Help = rpcField.Help,
                        Changed = false,
                        Value = vals?[rpcField.FieldName]
                    });
                }
            }
            else
            {
                _fieldsResult = fieldsTemplate.ToList();
                _fieldsResult.ForEach(x => x.Changed = false);
            }
        }

        public IEnumerable<RpcField> GetFields()
        {
            return _fieldsResult;
        }

        public void SetFieldValue(string field, object value)
        {
            var fieldAttribute = _fieldsResult.FirstOrDefault(f => f.FieldName == field);
            if (fieldAttribute == null) return;

            fieldAttribute.Changed = fieldAttribute.Changed == false;

            fieldAttribute.Value = value;
        }

        public RpcField GetField(string field)
        {
            var fieldAttribute = _fieldsResult.FirstOrDefault(f => f.FieldName == field);
            return fieldAttribute;
        }

        public void Save()
        {
            var values = new XmlRpcStruct();

            if (_id >= 0)
            {
                foreach (var field in _fieldsResult.Where(f => (bool) f.Changed))
                {

                    if (field.Value != null)
                        values[field.FieldName] = field.Value;
                    else
                        values[field.FieldName] = false;
                }

                _rpcConnection.Write(_model, new int[1] {_id}, values);
            }
            else
            {
                foreach (var field in _fieldsResult.Where(f => (bool)f.Changed))
                {
                    if (field.Value != null)
                        values[field.FieldName] = field.Value;
                    else
                        values[field.FieldName] = false;
                }

                _id = _rpcConnection.Create(_model, values);
            }
        }

        public override string ToString()
        {
            var value = "";
            foreach (var field in _fieldsResult)
            {
                value += $"{field.FieldName}: {field.Value} \n";
            }

            return value;
        }
    }
}