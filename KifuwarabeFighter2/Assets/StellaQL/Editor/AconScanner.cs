﻿using System.Collections.Generic;
using System.Text;
using UnityEditor.Animations;
using UnityEngine;

namespace StellaQL
{
    public abstract class AbstractAconScanner
    {
        public AbstractAconScanner(bool parameterScan)
        {
            this.parameterScan = parameterScan;
        }
        bool parameterScan;

        void ScanRecursive(string path, AnimatorStateMachine stateMachine, Dictionary<string, AnimatorStateMachine> statemachineList_flat)
        {
            path += stateMachine.name + ".";
            statemachineList_flat.Add(path, stateMachine);

            foreach (ChildAnimatorStateMachine caStateMachine in stateMachine.stateMachines)
            {
                if (null == caStateMachine.stateMachine) { throw new UnityException("子ステートマシンがヌルだぜ☆（＞＿＜）！ 親stateMachine.name=[" + stateMachine.name+"]"); }
                ScanRecursive(path, caStateMachine.stateMachine, statemachineList_flat);
            }
        }

        public void ScanAnimatorController(AnimatorController ac, StringBuilder message)
        {
            message.AppendLine("Animator controller Scanning...☆（＾～＾）");

            // パラメーター
            if(parameterScan){
                AnimatorControllerParameter[] acpArray = ac.parameters;
                int num = 0;
                foreach (AnimatorControllerParameter acp in acpArray)
                {
                    OnParameter( num, acp);
                    num++;
                }
            }

            int lNum = 0;
            foreach (AnimatorControllerLayer layer in ac.layers)//レイヤー
            {
                if(OnLayer( layer, lNum))
                {
                    Dictionary<string, AnimatorStateMachine> statemachineList_flat = new Dictionary<string, AnimatorStateMachine>(); // フルパス, ステートマシン

                    // レイヤーは、ステートマシンを持っていないことがあるぜ☆（＾～＾） 他のレイヤーのステートマシンを参照してるのだろう☆（＾～＾）
                    if (null == layer.stateMachine) { continue; } // 次のレイヤーへ
                    ScanRecursive("", layer.stateMachine, statemachineList_flat);// 再帰をスキャンして、フラットにする。

                    int smNum = 0;
                    foreach (KeyValuePair<string, AnimatorStateMachine> statemachine_pair in statemachineList_flat)
                    { // ステート・マシン

                        FullpathTokens ft1 = new FullpathTokens();
                        int caret1 = 0;
                        if(!FullpathSyntaxP.Fixed_LayerName_And_StatemachineNames(statemachine_pair.Key, ref caret1, ref ft1)) {
                            throw new UnityException("[" + statemachine_pair.Key + "]パース失敗だぜ☆（＾～＾） ac=[" + ac.name + "]"); }

                        if(OnStatemachine(
                            statemachine_pair.Key,
                            ft1.StatemachinePath,// 例えばフルパスが "Base Layer.Alpaca.Bear.Cat.Dog" のとき、"Alpaca.Bear.Cat"。
                            statemachine_pair.Value, lNum, smNum))
                        {
                            int sNum = 0;
                            foreach (ChildAnimatorState caState in statemachine_pair.Value.states)
                            { //ステート（ラッパー）

                                //FullpathTokens ft2 = new FullpathTokens(ft1);
                                //int caret2 = caret1;
                                //FullpathSyntaxP.Continued_Fixed_StateName(statemachine_pair.Key, ref caret2, ft2, ref ft2);

                                if (OnState( statemachine_pair.Key, caState, lNum, smNum, sNum))//, ft2.StatemachinePath
                                {
                                    int tNum = 0; // トランジション番号
                                    foreach (AnimatorStateTransition transition in caState.state.transitions)
                                    { // トランジション
                                        if(OnTransition( transition, lNum, smNum, sNum, tNum))
                                        {
                                            int cNum = 0; // コンディション番号
                                            foreach (AnimatorCondition condition in transition.conditions)
                                            { // コンディション
                                                OnCondition( condition, lNum, smNum, sNum, tNum, cNum);
                                                cNum++;
                                            } // コンディション
                                        }
                                        tNum++;
                                    }//トランジション
                                }
                                sNum++;
                            }//ステート（ラッパー）
                        }
                        smNum++;
                    }//ステートマシン
                }
                lNum++;
            }//レイヤー

            message.AppendLine("Scanned☆（＾▽＾）");
        }

        public virtual void OnParameter(int num, AnimatorControllerParameter acp) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="layer"></param>
        /// <returns>下位を検索するなら真</returns>
        public virtual bool OnLayer(AnimatorControllerLayer layer, int lNum) { return false; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullnameEndWithDot"></param>
        /// <param name="statemachine"></param>
        /// <returns>下位を検索するなら真</returns>
        public virtual bool OnStatemachine(string fullnameEndWithDot, string statemachinePath, AnimatorStateMachine statemachine, int lNum, int smNum) { return false; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentPath"></param>
        /// <param name="caState"></param>
        /// <returns>下位を検索するなら真</returns>
        public virtual bool OnState(string parentPath, ChildAnimatorState caState, int lNum, int smNum, int sNum) { return false; }
        //, string statemachinePath
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transition"></param>
        /// <returns>下位を検索するなら真</returns>
        public virtual bool OnTransition(AnimatorStateTransition transition, int lNum, int smNum, int sNum, int tNum) { return false; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <returns>下位を検索するなら真</returns>
        public virtual bool OnCondition(AnimatorCondition condition, int lNum, int smNum, int sNum, int tNum, int cNum) { return false; }
    }

    /// <summary>
    /// 全走査。
    /// </summary>
    public class AconScanner : AbstractAconScanner
    {
        public AconScanner() : base(true)
        {
            AconData = new AconData();
        }
        public AconData AconData { get; private set; }

        public override void OnParameter( int num, AnimatorControllerParameter acp)
        {
            ParameterRecord record = new ParameterRecord(num, acp.name, acp.defaultBool, acp.defaultFloat, acp.defaultInt, acp.nameHash, acp.type);
            AconData.table_parameter.Add(record);
        }

        public override bool OnLayer( AnimatorControllerLayer layer, int lNum)
        {
            LayerRecord layerRecord = new LayerRecord(
                lNum, //AconData.table_layer.Count,
                layer);
            AconData.table_layer.Add(layerRecord); return true;
        }

        public override bool OnStatemachine(string fullnameEndWithDot, string statemachinePath, AnimatorStateMachine statemachine, int lNum, int smNum)
        {
            StatemachineRecord stateMachineRecord = new StatemachineRecord(
                lNum,
                smNum,
                //fullnameEndWithDot,
                statemachinePath,
                statemachine, AconData.table_position);
            AconData.table_statemachine.Add(stateMachineRecord); return true;
        }

        public override bool OnState( string parentPath, ChildAnimatorState caState, int lNum, int smNum, int sNum)
        {
            //, string statemachinePath
            StateRecord stateRecord = StateRecord.CreateInstance(
                lNum,
                smNum,
                sNum,
                parentPath,
                //statemachinePath,
                caState, AconData.table_position);
            AconData.table_state.Add(stateRecord); return true;
        }

        public override bool OnTransition( AnimatorStateTransition transition, int lNum, int smNum, int sNum, int tNum)
        {
            TransitionRecord transitionRecord = new TransitionRecord(
                lNum, //AconData.table_layer.Count,
                smNum, //AconData.table_statemachine.Count,
                sNum, //AconData.table_state.Count,
                tNum,//AconData.table_transition.Count,
                transition, "");
            AconData.table_transition.Add(transitionRecord); return true;
        }

        public override bool OnCondition( AnimatorCondition condition, int lNum, int smNum, int sNum, int tNum, int cNum)
        {
            ConditionRecord conditionRecord = new ConditionRecord(
                lNum, //AconData.table_layer.Count,
                smNum, //AconData.table_statemachine.Count,
                sNum, //AconData.table_state.Count,
                tNum, //AconData.table_transition.Count,
                cNum, //AconData.table_condition.Count,
                condition);
            AconData.table_condition.Add(conditionRecord); return true;
        }
    }

    /// <summary>
    /// ステートマシン、ステートを全部走査。
    /// </summary>
    public class AconStateNameScanner : AbstractAconScanner
    {
        public AconStateNameScanner() : base(false)
        {
            FullpathSet = new HashSet<string>();
        }
        public HashSet<string> FullpathSet { get; private set; }

        public override bool OnLayer( AnimatorControllerLayer layer, int lNum)
        {
            return true;
        }

        public override bool OnStatemachine(string fullnameEndWithDot, string statemachinePath, AnimatorStateMachine statemachine, int lNum, int smNum)
        {
            FullpathSet.Add(fullnameEndWithDot); return true;
        }

        public override bool OnState( string parentPath, ChildAnimatorState caState, int lNum, int smNum, int sNum)
        {
            //, string statemachinePath
            FullpathSet.Add(parentPath + caState.state.name); return false;
        }

        public string Dump()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string path in this.FullpathSet)
            {
                sb.Append(path);
                sb.Append(" To26= ");
                sb.AppendLine(FullpathConstantGenerator.String_split_toUppercaseAlphabetFigureOnly_join(path,".","_"));
            }
            return sb.ToString();
        }
    }
}