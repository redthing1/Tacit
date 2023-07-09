using System;
using System.Collections.Generic;
using System.Reflection;
using static Tacit.Framework.GOAP.Details.Strings;
#if UNITY_2018_1_OR_NEWER
using UnityEngine;
#endif

namespace Tacit.Framework.GOAP.Details {
    public class ActionMap : ActionHandler {

        private static readonly object[] NoArg = new object[0];
        private Dictionary<string, MethodInfo> map;
        public bool verbose;

        private int frameCount {
            get {
#if UNITY_2018_1_OR_NEWER
        return Time.frameCount;
#else
                return 0;
#endif
            }
        }

        void ActionHandler.Effect<T>(object action, GameAI<T> client) {
            switch (action) {
                case string noArg:
                    if (noArg == INITIAL_STATE) return;
                    Print($"No-arg: {noArg} @{frameCount}");
                    Map(noArg, client).Invoke(client, NoArg);
                    return;
                case Action method:
                    Print($"Delegate: {method.Method.Name} @{frameCount}");
                    method();
                    return;
                case null:
                    client.Idle();
                    return;
                default:
                    throw new ArgumentException(UNKNOWN_ARG + action);
            }
        }

        private MethodInfo Map<T>(string name, GameAI<T> client) where T : class {
            map = map ?? new Dictionary<string, MethodInfo>();
            MethodInfo method;
            map.TryGetValue(name, out method);
            if (method == null) {
                map[name] = method = client.GetType().GetMethod(name);
            }
            return method;
        }

        internal void Print(object arg) {
            if (!verbose) return;
#if UNITY_2018_1_OR_NEWER
        Debug.Log(arg);
#else
            Console.WriteLine(arg);
#endif
        }
    }
}