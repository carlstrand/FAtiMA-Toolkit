﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ActionLibrary;
using ActionLibrary.DTOs;
using AssetManagerPackage;
using CommeillFaut.DTOs;
using CommeillFaut;
using Conditions.DTOs;
using EmotionalAppraisal;
using EmotionalAppraisal.DTOs;
using IntegratedAuthoringTool;
using KnowledgeBase;
using Microsoft.CSharp.RuntimeBinder;
using RolePlayCharacter;
using WellFormedNames;
using WorldModel;
using WorldModel.DTOs;

namespace CommeillFautTutorial
{
    class Program
    {
        static List<RolePlayCharacterAsset> rpcList;

        static void Main(string[] args)
        {

            var iat = IntegratedAuthoringToolAsset.LoadFromFile("../../../Examples/CiF/CiF-Scenario-IAT.iat");
            rpcList = new List<RolePlayCharacterAsset>();
        
            foreach (var source in iat.GetAllCharacterSources())
            {

                var rpc = RolePlayCharacterAsset.LoadFromFile(source.Source);


                //rpc.DynamicPropertiesRegistry.RegistDynamicProperty(Name.BuildName("Volition"),cif.VolitionPropertyCalculator);
                rpc.LoadAssociatedAssets();

                iat.BindToRegistry(rpc.DynamicPropertiesRegistry);

                rpcList.Add(rpc);

            }
          

            foreach (var actor in rpcList)
            {


                foreach (var anotherActor in rpcList)
                {
                    if (actor != anotherActor)
                    {


                        var changed = new[] { EventHelper.ActionEnd(anotherActor.CharacterName.ToString(), "Enters", "Room") };
                        actor.Perceive(changed);
                    }
                    
                }
                //         actor.SaveToFile("../../../Examples/" + actor.CharacterName + "-output1" + ".rpc");
            }


            var influenceRule = new InfluenceRule(new InfluenceRuleDTO()
            {
                Value = 2,
                Rule = new ConditionSetDTO()
                
            });

            var se = new SocialExchange(new SocialExchangeDTO(){

                Name = (Name)"Flirt",
                Initiator = (Name)"[i]",
                Target = (Name)"[t]",
                StartingConditions = new ConditionSetDTO(),
                InfluenceRules = new List<InfluenceRuleDTO>(){influenceRule.ToDTO()},
                Steps = new List<Name>(){(Name)"Initiate", (Name)"Answer", (Name)"End"}

            });

            var cif = new CommeillFautAsset();

            cif.AddOrUpdateExchange(se.ToDTO);

            cif.SaveToFile("../../../Examples/" + "cifTutorial.cif");


            List<Name> _events = new List<Name>();
            List<IAction> _actions = new List<IAction>();
/*
            IAction action = null;
            while (true)
            {
                _actions.Clear();
                foreach (var rpc in rpcList)
                {
                    rpc.Tick++;

                    Console.WriteLine("Character perceiving: " + rpc.CharacterName + " its mood: " + rpc.Mood);

                    rpc.Perceive(_events);
                    var decisions = rpc.Decide();
                    _actions.Add(decisions.FirstOrDefault());

                    foreach (var act in decisions)
                    {
                        Console.WriteLine(rpc.CharacterName + " has this action: " + act.Name);
                        
                    }
                    rpc.SaveToFile("../../../Examples/CiF/" + rpc.CharacterName + "-output" + ".rpc");


                }

                _events.Clear();
             
                    var randomGen = new Random(Guid.NewGuid().GetHashCode());

                    var pos = randomGen.Next(rpcList.Count);
                int i = 0;
                  
                    var initiator = rpcList[pos];
                
               
                    action = _actions.ElementAt(pos);
              
                Console.WriteLine();
                if(action == null)
                Console.WriteLine(initiator.CharacterName + " does not have any action to do ");

                if (action != null)
                {

                    var initiatorName = initiator.CharacterName.ToString();
                    var targetName = action.Target.ToString();
                    var nextState = action.Parameters[1].ToString();
                    var currentState = action.Parameters[0].ToString();

                    Console.WriteLine("Action: " + initiatorName + " does " + action.Name + " to " +
                                     targetName + "\n" + action.Parameters[1]);

                    _events.Add(EventHelper.ActionEnd(initiatorName, action.Name.ToString(),
                        action.Target.ToString()));

                 

                    Console.WriteLine();
                    Console.WriteLine("Dialogue:");
                    Console.WriteLine("Current State: " + currentState);
                    if (iat.GetDialogueActions(action.Parameters[0], action.Parameters[1], action.Parameters[2], action.Parameters[3]).FirstOrDefault() != null)
                        Console.WriteLine(initiator.CharacterName + " says: ''" +
                                         iat.GetDialogueActions(action.Parameters[0], action.Parameters[1], action.Parameters[2], action.Parameters[3]).FirstOrDefault().Utterance + "'' to " + targetName);
                    else Console.WriteLine(initiator.CharacterName + " says: " + "there is no dialogue suited for this action");


               
                Console.WriteLine();
                Console.ReadKey();
            }*/
        }
    }
}

    





