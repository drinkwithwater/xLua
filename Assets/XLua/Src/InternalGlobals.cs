﻿/*
 * Tencent is pleased to support the open source community by making xLua available.
 * Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
 * Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
 * http://opensource.org/licenses/MIT
 * Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace XLua
{
    public class InternalGlobals
    {
#if !THREAD_SAFE && !HOTFIX_ENABLE
        internal static byte[] strBuff = new byte[256];
#endif

        public delegate bool TryArrayGet(Type type, RealStatePtr L, ObjectTranslator translator, object obj, int index);
        public delegate bool TryArraySet(Type type, RealStatePtr L, ObjectTranslator translator, object obj, int array_idx, int obj_idx);
        internal static volatile TryArrayGet genTryArrayGetPtr = null;
        internal static volatile TryArraySet genTryArraySetPtr = null;

        public static volatile ITranslatorPool translatorPool = null;

        internal static volatile int LUA_REGISTRYINDEX = -10000;

        internal static volatile Dictionary<string, string> supportOp = new Dictionary<string, string>()
        {
            { "op_Addition", "__add" },
            { "op_Subtraction", "__sub" },
            { "op_Multiply", "__mul" },
            { "op_Division", "__div" },
            { "op_Equality", "__eq" },
            { "op_UnaryNegation", "__unm" },
            { "op_LessThan", "__lt" },
            { "op_LessThanOrEqual", "__le" },
            { "op_Modulus", "__mod" },
            { "op_BitwiseAnd", "__band" },
            { "op_BitwiseOr", "__bor" },
            { "op_ExclusiveOr", "__bxor" },
            { "op_OnesComplement", "__bnot" },
            { "op_LeftShift", "__shl" },
            { "op_RightShift", "__shr" },
        };

        internal static volatile Dictionary<Type, IEnumerable<MethodInfo>> extensionMethodMap = null;

#if GEN_CODE_MINIMIZE
        internal static volatile LuaDLL.CSharpWrapperCaller CSharpWrapperCallerPtr = new LuaDLL.CSharpWrapperCaller(StaticLuaCallbacks.CSharpWrapperCallerImpl);
#endif

        internal static volatile LuaCSFunction LazyReflectionWrap = new LuaCSFunction(Utils.LazyReflectionCall);

        public static bool ifgen { get; }
        public static Func<int, LuaEnv, DelegateBridgeBase> ifgen_delegateBridgeMaker { get; private set; }
        public static Func<LuaEnv, RealStatePtr, ObjectTranslator> ifgen_objectTranslatorMaker { get; private set; }
        private static Func<ITranslatorPool> ifgen_translatorPoolGetter;

        static InternalGlobals()
        {
            Type InternalGlobalsGen = null;
            InternalGlobalsGen = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                select assembly.GetType($"XLua.{nameof(InternalGlobalsGen)}")).FirstOrDefault(x => x!=null);
            if (InternalGlobalsGen != null)
            {
                ifgen = true;
                Activator.CreateInstance(InternalGlobalsGen);
                translatorPool = ifgen_translatorPoolGetter();
            }
            else
            {
                ifgen = false;
                translatorPool = new ObjectTranslatorPool();
            }
        }

        public static void InitByGen(
            Dictionary<Type, IEnumerable<MethodInfo>> _extensionMethodMap, 
            TryArrayGet _genTryArrayGetPtr,
            TryArraySet _genTryArraySetPtr,
            Func<int, LuaEnv, DelegateBridgeBase> _delegateBridgeMaker,
            Func<LuaEnv, RealStatePtr, ObjectTranslator> _objectTranslatorMaker,
            Func<ITranslatorPool> _translatorPoolGetter
            )
        {
            extensionMethodMap = _extensionMethodMap;
            genTryArrayGetPtr = _genTryArrayGetPtr;
            genTryArraySetPtr = _genTryArraySetPtr;
            ifgen_delegateBridgeMaker = _delegateBridgeMaker;
            ifgen_objectTranslatorMaker = _objectTranslatorMaker;
            ifgen_translatorPoolGetter = _translatorPoolGetter;
        }

    }

}

