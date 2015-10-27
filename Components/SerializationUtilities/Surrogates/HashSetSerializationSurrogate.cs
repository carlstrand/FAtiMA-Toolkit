﻿using GAIPS.Serialization.SerializationGraph;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Utilities;

namespace GAIPS.Serialization.Surrogates
{
	//TODO improve this code

	public class HashSetSerializationSurrogate : ISerializationSurrogate
	{
		private static readonly Type DEFAULT_COMPARATOR_TYPE = Type.GetType("System.Collections.Generic.ObjectEqualityComparer`1");

		public void GetObjectData(object obj, IObjectGraphNode holder)
		{
			Type objType = obj.GetType();
			var f = objType.GetField("m_comparer", BindingFlags.NonPublic | BindingFlags.Instance);
			var comparator = f.GetValue(obj);
			Type comparatorType = comparator.GetType();

			if (!(comparatorType.IsGenericType && (comparatorType.GetGenericTypeDefinition() == DEFAULT_COMPARATOR_TYPE)))
				holder["comparer"] = holder.ParentGraph.BuildNode(comparator, null);

			Type elementType = objType.GetGenericArguments()[0];
			var nodeSequence = (obj as IEnumerable).Cast<object>().Select(o => holder.ParentGraph.BuildNode(o, elementType));
			if (!nodeSequence.IsEmpty())
			{
				ISequenceGraphNode sequence = holder.ParentGraph.BuildSequenceNode();
				sequence.AddRange(nodeSequence);
				holder["elements"] = sequence;
			}
		}

		public void SetObjectData(ref object obj, IObjectGraphNode node)
		{
			Type objType = obj.GetType();
			Type elemType = objType.GetGenericArguments()[0];

			IGraphNode comparerData = node["comparer"];
			if (comparerData != null)
			{
				Type comparerType = typeof(IEqualityComparer<>).MakeGenericType(elemType);
				var comparerObject = comparerData.RebuildObject(comparerType);
				var f = objType.GetField("m_comparer", BindingFlags.NonPublic | BindingFlags.Instance);
				f.SetValue(obj, comparerObject);
			}

			ISequenceGraphNode elements = node["elements"] as ISequenceGraphNode;
			if (elements != null)
			{
				var m = objType.GetMethod("UnionWith");
				var a = Array.CreateInstance(elemType, elements.Length);
				elements.Select(e => e.RebuildObject(elemType)).ToArray().CopyTo(a, 0);
				m.Invoke(obj, new object[] {a});
			}
		}
	}
}
