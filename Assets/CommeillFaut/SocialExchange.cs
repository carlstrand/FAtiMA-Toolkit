﻿using System;
using System.Collections.Generic;
using System.Linq;
using ActionLibrary;
using ActionLibrary.DTOs;
using CommeillFaut.DTOs;
using Conditions;
using Conditions.DTOs;
using KnowledgeBase;
using WellFormedNames;
using SerializationUtilities;

namespace CommeillFaut
{

    [Serializable]
    public class SocialExchange : ICustomSerialization
    {
             
        public Guid Id { get; private set; }

        public string Description { get; set; }

        public Name Name { get; set; }

        public ConditionSet Conditions { get; set; }

        /// The Social Exchange Name
        /// </summary>
        public Name Initiator { get; set; }

        /// The Social Exchange Name
        /// </summary>
        public Name Target { get; set; }


        public SocialExchange(SocialExchangeDTO s) 
        {
            Name = s.Name;
            Description = s.Description;
            Conditions = new ConditionSet(s.Conditions);
            Id = new Guid();
            Initiator = s.Initiator;
            Target = s.Target;
            
        }

      
       
        public override String ToString()
        {
            return Name + " " + Description + " " + this.Id + "\n";
        }

        public SocialExchangeDTO ToDTO()
        {
            return new SocialExchangeDTO() {Name = this.Name, Description = this.Description, Initiator = this.Initiator, Target = this.Target, Conditions = this.Conditions.ToDTO(), Id = this.Id};
        }
        
       public void GetObjectData(ISerializationData dataHolder, ISerializationContext context)
        {
            dataHolder.SetValue("Name", this.Name);
            dataHolder.SetValue("Description", this.Description);
            dataHolder.SetValue("Initiator",this.Initiator);
            dataHolder.SetValue("Target", this.Target);
            dataHolder.SetValue("Conditions", this.Conditions);


        }

        public void SetObjectData(ISerializationData dataHolder, ISerializationContext context)
        {
            Name = dataHolder.GetValue<Name>("Name");
            Description = dataHolder.GetValue<string>("Description");
            Initiator = dataHolder.GetValue<Name>("Initiator");
            Target = dataHolder.GetValue<Name>("Target");
            Conditions = dataHolder.GetValue<ConditionSet>("Conditions");
        }

        
        public void AddCondition(Condition cond)
        {
            this.Conditions.Add(cond);
        }

        public void RemoveCondition(Condition cond)
        {
            this.Conditions.Remove(cond);
        }




        public float VolitionValue(Name init, Name targ, KB m_Kb)
        {
            float toRet = 0.0f;
            var totalCertainty = 0.0f;
            int totalConds = Conditions.Count();

            if (init == targ) return 0;

            var targetSub = new Substitution(Target, new ComplexValue(targ));
            var initiatorSub = new Substitution(Target, new ComplexValue(init));

            var constraints = new SubstitutionSet();
            constraints.AddSubstitution(targetSub);
            constraints.AddSubstitution(initiatorSub);


            foreach (var c in Conditions) // For instance SI([x]) >= 40
            {

               var resultingConstraints = c.Unify(m_Kb, init, new[] { constraints } );  // Whats the sub here [x]/John

                var total = 0.0f;
                var totalSets = resultingConstraints.Count();
                foreach (var res in resultingConstraints)
                {
                    if (resultingConstraints.Count() > 0)
                    {
                        var condition = c.ToString();

                        var certainty = res.FindMinimumCertainty();  // How do I ask SI(John) >= 40 and get its certainty

                        total += certainty;
                    }

                }

                var averageCertainty = total / totalSets;
                totalCertainty += averageCertainty;
            }
       

                toRet = totalCertainty / totalConds;

                return toRet;

            }

        }

    }

