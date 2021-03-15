using ParallelCore.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelCore.Scheduler
{
    internal class LoadBalancedScheduler<T> : IScheduler<T>
    {
        private readonly ITaskProvider<T> _provider;
        internal LoadBalancedScheduler(ITaskProvider<T> provider)
        {
            _provider = provider;
        }

        private struct InternalTask
        {
            public T Identifier;
            public Task ClrTask;
        }

        private readonly ConcurrentBag<TaskException<T>> _exceptions = new ConcurrentBag<TaskException<T>>();
        private readonly ConcurrentQueue<T> _completeQueue = new ConcurrentQueue<T>();
        private readonly List<InternalTask> _runningTasks = new List<InternalTask>();

        public IEnumerable<TaskException<T>> Exceptions => _exceptions;
        private void CleanUp()
        {
            while (!_exceptions.IsEmpty)
                _exceptions.TryTake(out _);

            while (!_completeQueue.IsEmpty)
                _completeQueue.TryDequeue(out _);

            _runningTasks.Clear();
        }
        private void CoreLoop()
        {
            CleanUp();

            for (; ; )
            {
                while (_completeQueue.TryDequeue(out var identifier))
                {
                    _runningTasks.RemoveAll(task => task.Identifier.Equals(identifier));
                    _provider.MarkAsCompleted(identifier);
                }

                var state = _provider.TryGetNextAvailableTask(out var newId);

                switch (state)
                {
                    case TaskProviderState.Normal:
                        var task = CreateInternalTask(newId);
                        _provider.MarkAsRunning(newId);
                        _runningTasks.Add(task);
                        task.ClrTask.Start();
                        break;

                    case TaskProviderState.NoAvailableTask:
                        if (!HasRunningTask)
                        {
                            // No running task but nothing to run. Likely the computation is aborted.
                            return;
                        }
                        WaitAnyInternalTask();
                        break;

                    case TaskProviderState.AllComplete:
                    case TaskProviderState.Abort:
                        WaitAllInternalTasks();
                        return;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(state));
                }
            }
        }

        private bool HasRunningTask => _runningTasks.Count != 0;
        public void Run()
        {
            CoreLoop();
        }
        public void RunSynchronously()
        {
            CleanUp();

            for (; ; )
            {
                var state = _provider.TryGetNextAvailableTask(out var newId);

                switch (state)
                {
                    case TaskProviderState.Normal:
                        _provider.MarkAsRunning(newId);
                        try
                        {
                            _provider.RunTaskSynchronously(newId);
                        }
                        catch (Exception e)
                        {
                            _exceptions.Add(new TaskException<T>
                            {
                                Identifier = newId,
                                InnerException = e
                            });
                        }
                        finally
                        {
                            _provider.MarkAsCompleted(newId);
                        }

                        break;

                    case TaskProviderState.NoAvailableTask:
                        throw new InvalidOperationException();

                    case TaskProviderState.AllComplete:
                    case TaskProviderState.Abort:
                        return;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(state));
                }
            }
        }
        private void CompleteInternalTask(T identifier)
        {
            _completeQueue.Enqueue(identifier);
        }
        private Task[] AllClrTasks()
        {
            return _runningTasks.Select(it => it.ClrTask).ToArray();
        }
        private void WaitAnyInternalTask()
        {
            Task.WaitAny(AllClrTasks());
        }
        private void WaitAllInternalTasks()
        {
            if (!HasRunningTask)
                return;

            Task.WaitAll(AllClrTasks());
        }
        private InternalTask CreateInternalTask(T identifier)
        {
            var currentId = identifier;

            var task = new Task(() =>
            {
                try
                {
                    _provider.RunTaskSynchronously(currentId);
                }
                catch (Exception e)
                {
                    _exceptions.Add(new TaskException<T>
                    {
                        Identifier = currentId,
                        InnerException = e
                    });
                }
                finally
                {
                    CompleteInternalTask(currentId);
                }
            }, TaskCreationOptions.PreferFairness);

            return new InternalTask
            {
                ClrTask = task,
                Identifier = currentId
            };
        }
    }
}
