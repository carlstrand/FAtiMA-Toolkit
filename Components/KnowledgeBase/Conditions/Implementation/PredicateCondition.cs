﻿using System;
using System.Collections.Generic;
using System.Linq;
using KnowledgeBase.WellFormedNames;

namespace KnowledgeBase.Conditions
{
	public partial class Condition
	{
		private class PredicateCondition : Condition
		{
			private readonly IValueRetriver m_predicate;
			private readonly bool m_invert;

			public PredicateCondition(IValueRetriver p, bool expectedResult)
			{
				m_predicate = p;
				m_invert = !expectedResult;
			}

			protected override IEnumerable<SubstitutionSet> CheckActivation(KB kb, Name perspective, IEnumerable<SubstitutionSet> constraints)
			{
				List<SubstitutionSet> results = new List<SubstitutionSet>();
				var sets = m_predicate.Retrive(kb,perspective, constraints).ToList();
				if (sets.Count == 0 && m_invert)
				{
					results.AddRange(constraints);
				}
				else
				{
					foreach (var pair in sets)
					{
						if (pair.Item1.TypeCode != TypeCode.Boolean)
							continue;

						if (((bool)pair.Item1) != m_invert)
							results.Add(pair.Item2);
					}
				}

				return results;
			}

			public override string ToString()
			{
				return string.Format("{0} = {1}", m_predicate, !m_invert);
			}

			public override bool Equals(object obj)
			{
				PredicateCondition c = obj as PredicateCondition;
				if (c == null)
					return false;

				return m_predicate.Equals(c.m_predicate) && m_invert == c.m_invert;
			}

			public override int GetHashCode()
			{
				int h = m_predicate.GetHashCode();
				return m_invert ? ~h : h;
			}
		}
	}
}