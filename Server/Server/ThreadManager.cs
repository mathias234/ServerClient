using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Server {
    public static class ThreadManager {
        private static List<Thread> RunningThreads = new List<Thread>();

        public static void StartThread<T>(Action<T> a, T parameter) {
            var thread = new Thread(() => a(parameter));
            thread.Start();

            RunningThreads.Add(thread);
        }

        public static void Stop() {
            foreach (var thread in RunningThreads) {
                thread.Abort();
            }
        }
    }
}
